using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

//Adapted from code at
//http://brad-christie.com/blog/2012/12/07/pass-html-to-extension-method/

public static class HtmlExtensions
{
	public static MvcHtmlString RegisteredScripts( this HtmlHelper htmlHelper )
	{
		var registeredScripts = htmlHelper.ViewContext.HttpContext.Items[ "_scripts_" ] as Stack< Func<Object, HelperResult> >;
		if( registeredScripts != null && registeredScripts.Count >= 1 )
		{
			foreach( var script in registeredScripts )
			{
				htmlHelper.ViewContext.Writer.Write( script( null ) );
			}
		}

		return MvcHtmlString.Empty;
	}

	public static MvcHtmlString RegisterScript( this HtmlHelper htmlHelper, Func<Object, HelperResult> script )
	{
		if( script != null )
		{
			var registeredScripts = htmlHelper.ViewContext.HttpContext.Items[ "_scripts_" ] as Stack< Func<Object, HelperResult> >;
			if( registeredScripts == null )
			{
				registeredScripts = new Stack< Func<Object, HelperResult> >();
				htmlHelper.ViewContext.HttpContext.Items[ "_scripts_" ] = registeredScripts;
			}

			registeredScripts.Push( script );
		}

		return MvcHtmlString.Empty;
	}
}