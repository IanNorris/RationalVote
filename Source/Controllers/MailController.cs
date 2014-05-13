using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ActionMailer.Net.Mvc;
using RationalVote.Models;
using System.Configuration;

namespace RationalVote.Controllers
{
	public class MailController : MailerBase
	{
		public static string GetFromEmail( string purpose )
		{
			return String.Format( "\"{0} {1}\" <{2}>", 
				ConfigurationManager.AppSettings.Get("siteTitle"),
				purpose,
				ConfigurationManager.AppSettings.Get("emailFrom") );
		}

		public EmailResult VerificationEmail( User model, EmailVerificationToken token )
		{
			To.Add( model.Email );
			From = GetFromEmail( "Verification" );
			Subject = "Please verify your account";
			ViewBag.Token = token.Token;

			return Email( "VerificationEmail" );
		}

		public EmailResult ResetPasswordEmail( string email, string token )
		{
			To.Add( email );
			From = GetFromEmail( "Verification" );
			Subject = "Reset your password";
			ViewBag.Token = token;

			return Email( "ResetPasswordEmail" );
		}

		public EmailResult ExceptionEmail( HttpException e, string message )
		{
			To.Add( ConfigurationManager.AppSettings.Get( "ServerAdmin" ) );
			From = GetFromEmail( "Exception" );
			Subject = ConfigurationManager.AppSettings.Get("siteTitle") + " exception - " + message;
			ViewBag.Exception = e.ToString();

			return Email( "ExceptionEmail" );
		}
	}
}