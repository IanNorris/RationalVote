using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace RationalVote
{
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
		}

		bool IsSiteDown()
		{
			bool isDown = System.Configuration.ConfigurationManager.AppSettings[ "siteDown" ] == "true";
			bool userCanBypass = false;

			return isDown && !userCanBypass;
		}

		void Application_BeginRequest( object sender, EventArgs e )
		{
			if( IsSiteDown() )
			{
				int lastContent = Request.RawUrl.LastIndexOf( "/Content/" );
				if( lastContent != -1 )
				{
					return;
				}

				int lastScript = Request.RawUrl.LastIndexOf( "/Scripts/" );
				if( lastScript != -1 )
				{
					return;
				}

				HttpContext.Current.RewritePath( "/Error/SiteDown" );
			}
		}
	}
}
