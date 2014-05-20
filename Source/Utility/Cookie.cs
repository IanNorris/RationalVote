using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Collections.Specialized;

namespace Utility
{
	public class Cookie
	{
		public static void SetCookie( string name, string value, int lifeInSeconds )
		{
			//Cookie expiry
			DateTime expiry = DateTime.Now;
			expiry = expiry.Add( new TimeSpan( 0, 0, lifeInSeconds ) );

			HttpCookie cookie = new HttpCookie( name );
			cookie.HttpOnly = true;
			cookie.Expires = expiry;

			string domain = AppSettings.Get<string>( "cookieDomain" );
			if( domain != null && domain.Length != 0 )
			{
				cookie.Domain = domain;
			}
			cookie.Secure = System.Web.HttpContext.Current.Request.IsSecureConnection;
			cookie.Shareable = false;
			cookie.Value = value;

			System.Web.HttpContext.Current.Response.Cookies.Add( cookie );
		}

		public static string GetCookie( string name )
		{
			HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get( name );
			if( cookie != null )
			{
				return cookie.Value;
			}
			
			return null;
		}
	}
}