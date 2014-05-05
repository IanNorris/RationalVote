﻿using System;
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

		public void CreateAndInsertLinkSelf( DbConnection connection, DbTransaction transaction, long parent )
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
			vote.Vote = DebateLinkVote.VoteType.Agree;

			InsertVote( connection, transaction, vote );

			return link;
		}

		public bool InsertVote( DbConnection connection, DbTransaction transaction, DebateLinkVote debateVote )
		{
			//Check that the user is voting on the right link. If they're just spamming random junk at
			//the server this might catch it. We don't want junk lying around on the server that isn't visible.
			long found = connection.Query<long>( "SELECT 1 FROM DebateLink WHERE DebateLink.Parent = @Parent AND DebateLink.Child = @Child AND DebateLink.PathLength = 1",
									new
									{
										Parent = debateVote.Parent,
										Child = debateVote.Child,
									} ).FirstOrDefault();

			if( found != 1 )
			{
				return false;
			}

			connection.Execute( "INSERT INTO DebateLinkVote (Parent,Child,Vote,Owner) VALUES (@Parent, @Child, @Vote, @Owner) ON DUPLICATE KEY UPDATE Vote=VALUES(Vote)",
								new
								{
									Parent = debateVote.Parent,
									Child = debateVote.Child,
									Vote = debateVote.Vote,
									Owner = debateVote.Owner
								} );

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
				debate.Owner = ((RationalVote.Models.UserPrincipal)HttpContext.User).User.Id;
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

						return RedirectToAction( "Index", new { Id = Id } );
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
				debate.Owner = ( (RationalVote.Models.UserPrincipal)HttpContext.User ).User.Id;
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
				debateVote.Owner = ( (RationalVote.Models.UserPrincipal)HttpContext.User ).User.Id;

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
		// GET: /Debate/{Id?}
		[Route( "Debate/{Id?}" )]
		public ActionResult Index( long? Id )
		{
			if( Id != null )
			{
				using( DbConnection connection = RationalVoteContext.Connect() )
				{
					Debate debate = connection.Get<Debate>( Id );

					if( debate != null )
					{
						return View( "Display", debate );
					}
					else
					{
						throw new HttpException( 404, "This debate does not exist." );
					}
				}
			}
			else
			{
				using( DbConnection connection = RationalVoteContext.Connect() )
				{
					IEnumerable<Debate> list = connection.Query<Debate>(
						@"SELECT Debate.* FROM Debate
						LEFT OUTER JOIN DebateLink
						ON Debate.Id = DebateLink.Child AND DebateLink.PathLength = 1
						WHERE DebateLink.Child IS null
						ORDER BY Debate.Updated DESC, Debate.Posted DESC" );

					return View( "Index", list );
				}
			}
		}

		[Route( "Fallacy" )]
		public ActionResult Fallacy()
		{
			return View();
		}
	}
}