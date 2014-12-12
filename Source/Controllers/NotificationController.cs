using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RationalVote.DAL;
using RationalVote.Models;
using Dapper;

namespace RationalVote.Controllers
{
	public class NotificationController : Controller
	{
		// GET: Notification
		[RequireLogin]
		public ActionResult Index( int? offset )
		{
			using( DbConnection connection = RationalVoteContext.Connect() )
			{
				IEnumerable<Notification> notifications = connection.Query<Notification>(
						@"SELECT * FROM Notification WHERE Receiver = @User ORDER BY Sent DESC LIMIT @Offset, @Rows",
						new {	User = HttpContext.User.Identity.IsAuthenticated ? ( (RationalVote.Models.UserPrincipal)HttpContext.User ).User.User.Id : 0,
								Offset = offset.GetValueOrDefault(), Rows = 20 } );

				return View( notifications );
			}
		}
	}
}