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

namespace RationalVote.Controllers
{
	public class DebateController : Controller
	{
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