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
						"~/Scripts/jquery-{version}.js"));

			bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
						"~/Scripts/jquery.validate.js",
						"~/Scripts/jquery.validate.unobtrusive.js",
						"~/Scripts/jquery.validate.unobtrusive-custom.js"));

			// Use the development version of Modernizr to develop with and learn from. Then, when you're
			// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
						"~/Scripts/modernizr-*"));

			bundles.Add( new ScriptBundle( "~/bundles/zxcvbn" ).Include(
						"~/Scripts/zxcvbn.js",
						"~/Scripts/zxcvbn-register.js") );

			bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
					  "~/Scripts/bootstrap.js",
					  "~/Scripts/respond.js"));

			bundles.Add( new ScriptBundle( "~/bundles/errorscripts" ).Include(
					  "~/Scripts/jquery.backstretch.js",
					  "~/Scripts/errorscripts.js" ) );

			bundles.Add(new StyleBundle("~/Content/css").Include(
					  "~/Content/bootstrap.css",
					  "~/Content/Theme/default.css",
					  "~/Content/font-awesome/css/font-awesome.css"));

			bundles.Add( new StyleBundle( "~/Content/csserror" ).Include(
					  "~/Content/Theme/error.css" ) );
		}
	}
}
