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
					long Id = connection.Insert( debate );

					return RedirectToAction( "Index", new { Id = Id } );
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

						DebateLink link = new DebateLink();
						link.Parent = debateInput.Parent;
						link.Type = debateInput.Type;
						link.Child = debate;

						connection.Execute( "INSERT INTO DebateLink (Parent, Child, Type, Weight) VALUES (@Parent, @Child, @Type, @Weight)", 
							new {
								Parent = link.Parent,
								Child = link.Child.Id,
								Type = link.Type,
								Weight = link.Weight,
							},
							transaction );

						transaction.Commit();

						return View( "_DebateBlockLink", link );
					}
				}
			}

			return View( "_DebateErrorInline", new ErrorMessage( ErrorMessage.TypeEnum.Danger, "Unable to add response", "Your argument was unable to be made." ) );
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

					return View( "Display", debate );
				}
			}
			else
			{
				using( DbConnection connection = RationalVoteContext.Connect() )
				{
					IEnumerable<Debate> list = connection.Query<Debate>(
						@"SELECT * FROM Debate
						LEFT OUTER JOIN DebateLink
						ON Debate.Id = DebateLink.Child
						WHERE DebateLink.Child IS null
						ORDER BY Debate.Updated DESC, Debate.Posted DESC" );

					return View( "Index", list );
				}
			}
		}
	}
}