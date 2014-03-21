using System.Web;
using System.Web.Optimization;

namespace RationalVote
{
	public class BundleConfig
	{
		// For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
						"~/Scripts/ThirdParty/jquery-{version}.js"));

			bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
						"~/Scripts/ThirdParty/jquery.validate.js",
						"~/Scripts/ThirdParty/jquery.validate.unobtrusive.js",
						"~/Scripts/ThirdParty/jquery.validate.unobtrusive-custom.js"));

			// Use the development version of Modernizr to develop with and learn from. Then, when you're
			// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
						"~/Scripts/ThirdParty/modernizr-*"));

			bundles.Add( new ScriptBundle( "~/bundles/zxcvbn" ).Include(
						"~/Scripts/ThirdParty/zxcvbn.js",
						"~/Scripts/zxcvbn-register.js") );

			bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
					  "~/Scripts/ThirdParty/bootstrap.js",
					  "~/Scripts/ThirdParty/respond.js"));

			bundles.Add( new ScriptBundle( "~/bundles/errorscripts" ).Include(
					  "~/Scripts/ThirdParty/jquery.backstretch.js",
					  "~/Scripts/errorscripts.js" ) );

			bundles.Add(new StyleBundle("~/Content/css").Include(
					  "~/Content/ThirdParty/bootstrap.css",
					  "~/Content/Theme/default.css",
					 "~/Content/Theme/custom.css",
					  "~/Content/ThirdParty/font-awesome/css/font-awesome.css"));

			bundles.Add( new StyleBundle( "~/Content/csserror" ).Include(
					  "~/Content/Theme/error.css" ) );
		}
	}
}
