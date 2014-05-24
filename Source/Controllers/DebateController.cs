using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RationalVote.Models;
using System.Data.Common;
using Dapper;
using RationalVote.DAL;
using System.Text.RegularExpressions;

namespace RationalVote.Controllers
{
	public class DebateController : Controller
	{
		Regex replaceMultipleSpaces = null;

		public DebateController()
		{
			replaceMultipleSpaces = new Regex( @"[ ]{2,}", RegexOptions.None );
		}

		public static IEnumerable<DebateLink> GetDebateChildren( long debateId, long userID, long offset, DebateLink.LinkType? type )
		{
			string typeFilter = "";

			if( type != null )
			{
				typeFilter = " ON DebateLink.Type = " + (int)type.Value;
			}

			using( DbConnection connection = RationalVoteContext.Connect() )
			{
				//To limit selection, do SELECT TOP 10 etc

				IEnumerable<DebateLink> arguments = connection.Query<DebateLink, Debate, DebateLink>(
					@"SELECT
						DebateLink.Id, DebateLink.Type,
						DebateLink.Parent, DebateLinkVote.Vote,
						DebateLink.LocalFor, DebateLink.LocalAgainst,
						Debate.*
					FROM
						DebateLink
							LEFT JOIN
						Debate ON DebateLink.Child = Debate.Id
							LEFT OUTER JOIN
						DebateLinkVote ON (DebateLinkVote.Parent = DebateLink.Parent AND DebateLinkVote.Child = DebateLink.Child AND DebateLinkVote.Owner = @Owner" + typeFilter + @")
					WHERE
						DebateLink.Parent = @Parent AND DebateLink.PathLength = 1
					ORDER BY Debate.Status ASC, (Debate.WeightFor - Debate.WeightAgainst) DESC, DebateLink.Weight DESC, DebateLink.LinkTime DESC
					LIMIT @Offset, @MaxRows"
					,
					( Parent, Child ) =>
					{
						Parent.Child = Child;
						return Parent;
					},
					new { Parent = debateId, Owner = userID, Offset = offset, MaxRows = Debate.MaxChildrenPerFetch }
					);

				return arguments;
			}
		}

		public static void CreateAndInsertLinkSelf( DbConnection connection, DbTransaction transaction, long parent )
		{
			connection.Execute( "INSERT INTO DebateLink (Parent, Child, Type, LinkTime, PathLength) VALUES (@Parent, @Child, @Type, @LinkTime, @PathLength)",
				new
				{
					Parent = parent,
					Child = parent,
					Type = DebateLink.LinkType.For,
					LinkTime = DateTime.Now,
					PathLength = 0,
				},
				transaction );
		}

		public static void UpdateWeights( DbConnection connection, DbTransaction transaction, long parent )
		{
			connection.Execute( @"UPDATE DebateLink AS SummedOut
								INNER JOIN
									(
										SELECT 
											DL.Parent As Parent,
											DL.Child As Child,
											DL.Type as Type,
											-- These are the votes to be displayed next to children
											SUM( CASE WHEN DLV.Vote = 1 THEN 1 ELSE 0 END ) As LocalFor,
											SUM( CASE WHEN DLV.Vote > 1 THEN 1 ELSE 0 END ) As LocalAgainst
										FROM DebateLink DL
										INNER JOIN Debate D
											ON DL.Child = D.Id -- Join in the debate info
										INNER JOIN DebateLinkVote DLV
											ON 		DLV.Parent = DL.Parent -- Find matching rows
												AND DLV.Child = DL.Child 
												AND DLV.Type = DL.Type
												AND DLV.Vote > 0 -- Skip votes of 0, they're 'No opinion'
										WHERE
											DL.Parent = @Parent AND DL.PathLength = 1 -- Only look at immediate children
										GROUP BY DL.Child
									) AS SummedTemp
									ON SummedTemp.Parent = SummedOut.Parent AND SummedTemp.Child = SummedOut.Child AND SummedTemp.Type = SummedOut.Type
								SET
									SummedOut.LocalFor = SummedTemp.LocalFor, 
									SummedOut.LocalAgainst = SummedTemp.LocalAgainst,
									SummedOut.Weight = GREATEST( SummedTemp.LocalFor - SummedTemp.LocalAgainst, 0 )
								WHERE
									SummedOut.Parent = @Parent;

								UPDATE Debate AS D
								INNER JOIN
								(
									SELECT 
										DebateLink.Parent AS Parent,
										SUM( CASE WHEN DebateLink.Type = 0 THEN Weight ELSE 0 END ) As WeightFor,
										SUM( CASE WHEN DebateLink.Type = 1 THEN Weight ELSE 0 END ) As WeightAgainst
									FROM DebateLink
									WHERE
										DebateLink.Parent = @Parent AND DebateLink.PathLength = 1
								) AS DTemp
								ON DTemp.Parent = D.Id
								SET
									D.WeightFor = DTemp.WeightFor,
									D.WeightAgainst = DTemp.WeightAgainst
								WHERE D.Id = @Parent",
				new
				{
					Parent = parent,
				},
				transaction );
		}

		public DebateLink CreateAndInsertLink( DbConnection connection, DbTransaction transaction, long parent, Debate child, DebateLink.LinkType type )
		{
			DebateLink link = new DebateLink();
			link.Parent = parent;
			link.Type = type;
			link.Child = child;

			//Insert self reference
			CreateAndInsertLinkSelf( connection, transaction, child.Id );

			link.Id = connection.Query<long>( @"INSERT INTO DebateLink (Parent, Child, Type, LinkTime, PathLength) 
				SELECT ParentT.Parent, ChildT.Child, @Type, @LinkTime, ParentT.PathLength + ChildT.PathLength + 1
				FROM DebateLink ParentT, DebateLink ChildT
				WHERE ParentT.Child = @Parent AND ChildT.Parent = @Child;
				SELECT DebateLink.Id FROM DebateLink WHERE DebateLink.Parent = @Parent AND DebateLink.Child = @Child AND DebateLink.PathLength = 1",
				new
				{
					Parent = link.Parent,
					Child = link.Child.Id,
					Type = link.Type,
					LinkTime = DateTime.Now,
				},
				transaction ).First();

			DebateLinkVote vote = new DebateLinkVote();
			vote.Owner = child.Owner.Value;
			vote.Parent = parent;
			vote.Child = child.Id;
			vote.Type = type;
			vote.Vote = DebateLinkVote.VoteType.Agree;

			InsertVote( connection, transaction, vote );

			return link;
		}

		public bool InsertVote( DbConnection connection, DbTransaction transaction, DebateLinkVote debateVote )
		{
			//Check that the user is voting on the right link. If they're just spamming random junk at
			//the server this might catch it. We don't want junk lying around on the server that isn't visible.
			long found = connection.Query<long>( "SELECT 1 FROM DebateLink WHERE DebateLink.Parent = @Parent AND DebateLink.Child = @Child AND DebateLink.Type = @Type AND DebateLink.PathLength = 1",
									new
									{
										Parent = debateVote.Parent,
										Child = debateVote.Child,
										Type = debateVote.Type,
									} ).FirstOrDefault();

			if( found != 1 )
			{
				return false;
			}

			connection.Execute( "INSERT INTO DebateLinkVote (Parent,Child,Type,Vote,Owner) VALUES (@Parent, @Child, @Type, @Vote, @Owner) ON DUPLICATE KEY UPDATE Vote=VALUES(Vote)",
								new
								{
									Parent = debateVote.Parent,
									Child = debateVote.Child,
									Type = debateVote.Type,
									Vote = debateVote.Vote,
									Owner = debateVote.Owner
								} );

			UpdateWeights( connection, transaction, debateVote.Parent );

			return true;
		}

		//
		// GET: /New
		[RequireLogin]
		[Route( "New" )]
		public ActionResult Create()
		{
			return View();
		}

		//
		// POST: /New
		[HttpPost]
		[RequireLogin]
		[Route( "New" )]
		[ValidateAntiForgeryToken]
		public ActionResult Create( DebateNew debateInput )
		{
			if( ModelState.IsValid )
			{
				//Convert all superfluous consecutive whitespace with a single space
				debateInput.Argument = debateInput.Argument.Replace( '\r', ' ' );
				debateInput.Argument = debateInput.Argument.Replace( '\n', ' ' );
				debateInput.Argument = debateInput.Argument.Replace( '\t', ' ' );
				debateInput.Argument = replaceMultipleSpaces.Replace( debateInput.Argument, @" " );

				Debate debate = new Debate();
				debate.Owner = ((RationalVote.Models.UserPrincipal)HttpContext.User).User.User.Id;
				debate.Title = debateInput.Argument;
				debate.Locked = false;
				debate.Posted = DateTime.Now;
				debate.Status = Debate.StatusType.Open;
				debate.Updated = null;

				using( DbConnection connection = RationalVoteContext.Connect() )
				{
					using( DbTransaction transaction = connection.BeginTransaction() )
					{
						long Id = connection.Insert( debate, transaction );

						CreateAndInsertLinkSelf( connection, transaction, Id );

						transaction.Commit();

						return RedirectToAction( "Display", new { Id = Id } );
					}
				}
			}

			return View( debateInput );
		}


		//
		// POST: /New
		[HttpPost]
		[RequireLogin]
		[Route( "CreateAjax" )]
		[ValidateAntiForgeryToken]
		public ActionResult CreateAjax( DebateResponse debateInput )
		{
			if( ModelState.IsValid )
			{
				//Convert all superfluous consecutive whitespace with a single space
				debateInput.Argument = debateInput.Argument.Replace( '\r', ' ' );
				debateInput.Argument = debateInput.Argument.Replace( '\n', ' ' );
				debateInput.Argument = debateInput.Argument.Replace( '\t', ' ' );
				debateInput.Argument = replaceMultipleSpaces.Replace( debateInput.Argument, @" " );

				Debate debate = new Debate();
				debate.Owner = ( (RationalVote.Models.UserPrincipal)HttpContext.User ).User.User.Id;
				debate.Title = debateInput.Argument;
				debate.Locked = false;
				debate.Posted = DateTime.Now;
				debate.Status = Debate.StatusType.Open;
				debate.Updated = null;

				using( DbConnection connection = RationalVoteContext.Connect() )
				{
					using( DbTransaction transaction = connection.BeginTransaction() )
					{
						long Id = connection.Insert( debate, transaction );
						debate.Id = Id;

						DebateLink link = CreateAndInsertLink( connection, transaction, debateInput.Parent, debate, debateInput.Type );

						

						transaction.Commit();

						link.Vote = DebateLinkVote.VoteType.Agree;

						return View( "_DebateBlockLink", link );
					}
				}
			}

			return View( "_DebateErrorInline", new ErrorMessage( ErrorMessage.TypeEnum.Danger, "Unable to add response", "Your argument was unable to be made." ) );
		}

		//
		// POST: /VoteAjax
		[HttpPost]
		[RequireLogin]
		[Route( "VoteAjax" )]
		[ValidateAntiForgeryToken]
		public ActionResult VoteAjax( DebateLinkVote debateVote )
		{
			if( ModelState.IsValid )
			{
				debateVote.Owner = ( (RationalVote.Models.UserPrincipal)HttpContext.User ).User.User.Id;

				using( DbConnection connection = RationalVoteContext.Connect() )
				{
					if( InsertVote( connection, null, debateVote ) )
					{

					}
					else
					{
				
					}
				}

				return new HttpStatusCodeResult(HttpStatusCode.OK);
			}

			return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
		}

		//
		// GET: /Debate/{Id}
		[Route( "Debate/{Id}" )]
		public ActionResult Display( long Id )
		{
			using( DbConnection connection = RationalVoteContext.Connect() )
			{
				Debate debate = connection.Get<Debate>( Id );

				DebateContainer viewObj = new DebateContainer();
				viewObj.Debate = debate;
				viewObj.Children = GetDebateChildren( debate.Id, HttpContext.User.Identity.IsAuthenticated ? ( (RationalVote.Models.UserPrincipal)HttpContext.User ).User.User.Id : 0, 0, null );
				viewObj.VoteSplit = RationalVote.Models.VoteSplit.Get( debate.Id );

				if( debate != null )
				{
					return View( "Display", viewObj );
				}
				else
				{
					throw new HttpException( 404, "This debate does not exist." );
				}
			}
		}

		//
		// GET: /Newest
		[Route( "Newest/{start?}" )]
		public ActionResult Newest( long? start )
		{
			long startValid = start.GetValueOrDefault();

			ViewBag.Title = "Newest debates";

			if( startValid != 0 )
			{
				ViewBag.PreviousIndex = Math.Max( startValid - Debate.MaxPerPage, 0 );
			}

			ViewBag.Offset = startValid;

			using( DbConnection connection = RationalVoteContext.Connect() )
			{
				IEnumerable<Debate> list = connection.Query<Debate>(
					@"SELECT Debate.* FROM Debate
					LEFT OUTER JOIN DebateLink
					ON Debate.Id = DebateLink.Child AND DebateLink.PathLength = 1
					WHERE DebateLink.Child IS null
					ORDER BY Debate.Updated DESC, Debate.Posted DESC
					LIMIT @Offset, @MaxRows",
					new { MaxRows = Debate.MaxPerPage, Offset = startValid } );

				ViewBag.NextIndex = startValid + list.Count();

				return View( "Index", list );
			}
		}

		//
		// GET: /Popular
		[Route( "Popular/{start?}" )]
		public ActionResult Popular( long? start )
		{
			long startValid = start.GetValueOrDefault();

			ViewBag.Title = "Most popular debates";

			if( startValid != 0 )
			{
				ViewBag.PreviousIndex = Math.Max( startValid - Debate.MaxPerPage, 0 );
			}

			ViewBag.Offset = startValid;

			if( startValid > Debate.MaxPerPage * Debate.MaxPages )
			{
				IEnumerable<Debate> list = new List<Debate>();

				return View( "Index", list );
			}

			using( DbConnection connection = RationalVoteContext.Connect() )
			{
				IEnumerable<Debate> list = connection.Query<Debate>(
					@"SELECT Debate.* FROM Debate
					LEFT OUTER JOIN DebateLink
					ON Debate.Id = DebateLink.Child AND DebateLink.PathLength = 1
					WHERE DebateLink.Child IS null
					ORDER BY (Debate.WeightFor - Debate.WeightAgainst) DESC, Debate.Updated DESC, Debate.Posted DESC
					LIMIT @Offset, @MaxRows",
					new { MaxRows = Debate.MaxPerPage, Offset = startValid } );

				ViewBag.NextIndex = startValid + list.Count();

				return View( "Index", list );
			}
		}

		[Route( "Fallacy" )]
		public ActionResult Fallacy()
		{
			return View();
		}
	}
}