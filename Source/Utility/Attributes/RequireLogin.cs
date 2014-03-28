using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

public class RequireLoginAttribute : ActionFilterAttribute, IAuthenticationFilter
{
	public void OnAuthentication( AuthenticationContext filterContext )
	{
		if( !filterContext.HttpContext.User.Identity.IsAuthenticated )
		{
			filterContext.Controller.TempData[ "ErrorMessage" ] = "You must be signed in to view this page. Please either log in with your account details or register for an account if you do not have one.";
			filterContext.Controller.TempData[ "MessageTitle" ] = "Sign in to view this page";

			// auth failed, redirect to login page
			filterContext.Result = new HttpUnauthorizedResult();
		}
	}

	public void OnAuthenticationChallenge( System.Web.Mvc.Filters.AuthenticationChallengeContext filterContext )
	{
		
	}
}