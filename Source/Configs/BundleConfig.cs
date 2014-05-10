using System.Web;
using System.Web.Optimization;

namespace RationalVote
{
	public class BundleConfig
	{
		// For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles)
		{
#if !DEBUG
			BundleTable.EnableOptimizations = true;
#endif

			IItemTransform urlRewrite = new CssRewriteUrlTransform();

			bundles.Add(new ScriptBundle("~/Scripts/ThirdParty/jqbs").Include(
				//"~/Scripts/ThirdParty/modernizr-*",
				"~/Scripts/ThirdParty/jquery-{version}.js",
				"~/Scripts/ThirdParty/jquery.unobtrusive-ajax.js",
				"~/Scripts/ThirdParty/bootstrap.js",
				"~/Scripts/ThirdParty/respond.js" )
			);

			bundles.Add(new ScriptBundle("~/Scripts/ThirdParty/jqueryval").Include(
				"~/Scripts/ThirdParty/jquery.validate.js",
				"~/Scripts/ThirdParty/jquery.validate.unobtrusive.js",
				"~/Scripts/ThirdParty/jquery.validate.unobtrusive-custom.js",
				"~/Scripts/ThirdParty/jquery.charcounter.js",
				"~/Scripts/ThirdParty/jquery.nolinebreaks.js",
				"~/Scripts/ajax.debate-response.js")
			);

			bundles.Add( new ScriptBundle( "~/Scripts/ThirdParty/zxcvbn" ).Include(
				"~/Scripts/ThirdParty/zxcvbn-async.js",
				"~/Scripts/zxcvbn-register.js") 
			);

			bundles.Add( new ScriptBundle( "~/Scripts/errorscripts" ).Include(
				"~/Scripts/ThirdParty/jquery.backstretch.js",
				"~/Scripts/errorscripts.js" )
			);

			bundles.Add(new StyleBundle("~/Content/ThirdParty/css")
				.Include( "~/Content/ThirdParty/bootstrap.css", urlRewrite )
				.Include( "~/Content/ThirdParty/font-awesome.css", urlRewrite )
			);

			bundles.Add(new StyleBundle("~/Content/Theme/css")
				.Include( "~/Content/Theme/app.css", urlRewrite )
				.Include( "~/Content/Theme/fonts.css" ) //Paths get mangled if using UrlTransform here.
				.Include( "~/Content/Theme/style.css", urlRewrite )
				.Include( "~/Content/Theme/theme.css", urlRewrite )
				.Include( "~/Content/Theme/custom.css", urlRewrite )
			);

			bundles.Add( new StyleBundle( "~/Content/csserror" )
				.Include( "~/Content/Theme/error.css", urlRewrite )
			);

			bundles.Add( new StyleBundle( "~/Content/csserrorinline" )
				.Include( "~/Content/Theme/error-inline.css", urlRewrite )
			);

			bundles.Add( new StyleBundle( "~/Content/Fallacies/fallacies" )
				.Include( "~/Content/Fallacies/Fallacies.css", urlRewrite )
			);

			bundles.Add( new ScriptBundle( "~/Scripts/debate" ).Include(
				"~/Scripts/fallacy.js" )
			);

			bundles.Add( new StyleBundle( "~/Content/CSS/profile" )
				.Include( "~/Content/CSS/Profile.css", urlRewrite )
			);

			bundles.Add( new ScriptBundle( "~/Scripts/profile" ).Include(
				"~/Scripts/profile.js" )
			);
		}
	}
}
