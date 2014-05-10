using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace RVUtility
{
	public static class Menu
	{
		public class Item
		{
			public Item( string description, string icon )
			{
				this.description = description;
				this.icon = icon;
			}

			public string description { get; private set; }
			public string icon { get; private set; }
		}

		public class Action : Item
		{
			public Action( string description, string icon, string action, string controller, System.Web.Routing.RouteValueDictionary data )
				: base( description, icon )
			{
				this.action = action;
				this.controller = controller;
				this.data = data;
			}

			public string action { get; private set; }
			public string controller { get; private set; }
			public System.Web.Routing.RouteValueDictionary data { get; private set; }
		}

		public class Uri : Item
		{
			public Uri( string description, string icon, string uri )
				: base( description, icon )
			{
				this.uri = uri;
			}

			public string uri { get; private set; }
		}

		public class Separator : Item
		{
			public Separator()
				: base( "", "" )
			{
			}
		}

		public static string CreateMenuItem( this HtmlHelper htmlHelper, Item menuItem, out bool isActive )
		{
			string uri;
			bool separator = false;
			UrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);

			//This isn't the ideal way to do it,
			//having a virtual Render function would
			//be 'better' but would result in more code.
			if( menuItem.GetType() == typeof(Action) )
			{
				Action action = (Action)menuItem;

				uri = urlHelper.Action( action.action, action.controller, action.data );
			}
			else if( menuItem.GetType() == typeof(Uri) )
			{
				uri = ((Uri)menuItem).uri;
			}
			else if( menuItem.GetType() == typeof(Separator) )
			{
				separator = true;
				uri = "";
			}
			else
			{
				System.Diagnostics.Debug.Assert( false, "Unrecognized type" );
				uri = "";
			}

			isActive = false;

			string html;
			
			if( separator )
			{
				html = "<li class=\"divider\"></li>";
			}
			else
			{
				string className = "";

				if( urlHelper.RequestContext.HttpContext.Request.RawUrl == uri )
				{
					className = "active";
					isActive = true;
				}

				string iconString = "";
				if( menuItem.icon != null )
				{
					iconString = "<i class=\"fa fa-" + menuItem.icon + "\"></i>";
				}

				html = String.Format( "<li class=\"{0}\"><a href=\"{1}\">{2} {3}</a></li>", className, uri, iconString, menuItem.description );
			}
			
			return html;
		}

		public static MvcHtmlString CreateMenuItem( this HtmlHelper htmlHelper, Item menuItem )
		{
			bool isActive;

			return MvcHtmlString.Create( CreateMenuItem( htmlHelper, menuItem, out isActive ) );
		}

		public static MvcHtmlString CreateMenu( this HtmlHelper htmlHelper, string title, string htmlBefore, string htmlAfter, Item[] menuItems )
		{
			string result = "";

			bool parentActive = false;

			foreach( Item item in menuItems )
			{
				bool isActive;

				result = result + CreateMenuItem( htmlHelper, item, out isActive );
				parentActive |= isActive;
			}

			string menuStyle = "";
			if( parentActive )
			{
				menuStyle = "active";
			}

			string initial = String.Format( "<li class=\"dropdown {0}\"><a href=\"#\" class=\"dropdown-toggle\" data-toggle=\"dropdown\" data-hover=\"dropdown\">{1}{2}{3}</a><ul class=\"dropdown-menu\">",
				menuStyle, htmlBefore, title, htmlAfter );

			return MvcHtmlString.Create( initial + result + "</ul></li>" );
		}
	}
}