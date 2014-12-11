using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using RationalVote.SignalR;

namespace RationalVote.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			//GlobalHost.ConnectionManager.GetHubContext<NotificationHub,INotificationClient>().Clients.All.OnCountUpdated( 333 );
			return View();
		}

		public ActionResult About()
		{
			return View();
		}

		public ActionResult Contact()
		{
			return View();
		}

		public ActionResult ForLegislators()
		{
			return View();
		}

		public ActionResult ForBusiness()
		{
			return View();
		}

		public ActionResult ExampleUses()
		{
			return View();
		}

		public ActionResult TermsOfService()
		{
			return View();
		}

		public ActionResult PrivacyPolicy()
		{
			return View();
		}

		public ActionResult DialogTermsOfService()
		{
			return View();
		}

		public ActionResult DialogPrivacyPolicy()
		{
			return View();
		}

		public ActionResult DialogProfilePic()
		{
			return View();
		}
	}
}