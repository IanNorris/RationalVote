﻿using System.Web;
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
				"~/Scripts/ThirdParty/bootstrap.js",
				"~/Scripts/ThirdParty/respond.js" )
			);

			bundles.Add(new ScriptBundle("~/Scripts/ThirdParty/jqueryval").Include(
				"~/Scripts/ThirdParty/jquery.validate.js",
				"~/Scripts/ThirdParty/jquery.validate.unobtrusive.js",
				"~/Scripts/ThirdParty/jquery.validate.unobtrusive-custom.js")
			);

			bundles.Add( new ScriptBundle( "~/Scripts/ThirdParty/zxcvbn" ).Include(
				"~/Scripts/ThirdParty/zxcvbn.js",
				"~/Scripts/zxcvbn-register.js") 
			);

			bundles.Add( new ScriptBundle( "~/Scripts/ThirdParty/errorscripts" ).Include(
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
				.Include( "~/Content/Theme/default.css", urlRewrite )
				.Include( "~/Content/Theme/custom.css", urlRewrite )
			);

			bundles.Add( new StyleBundle( "~/Content/csserror" )
				.Include( "~/Content/Theme/error.css", urlRewrite )
			);
		}
	}
}
