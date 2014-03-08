using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RationalVote.Controllers
{
	public class ErrorController : Controller
	{
		//
		// GET: /Error/NotFound
		public ActionResult NotFound()
		{
			Response.StatusCode = 404;
			return View();
		}

		//
		// GET: /Error/Forbidden
		public ActionResult Forbidden()
		{
			Response.StatusCode = 403;
			return View();
		}

		//
		// GET: /Error/Internal
		public ActionResult Internal()
		{
			Response.StatusCode = 500;
			return View();
		}

		//
		// GET: /Error/SiteDown
		public ActionResult SiteDown()
		{
			Response.StatusCode = 503;
			Response.AppendHeader( "Retry-After", "3600");
			return View();
		}
	}
}