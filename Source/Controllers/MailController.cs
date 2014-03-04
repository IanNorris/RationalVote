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
		public EmailResult VerificationEmail( User model, EmailVerificationToken token )
		{
			To.Add( model.Email );
			From = ConfigurationManager.AppSettings.Get("emailFrom");
			Subject = "Please verify your account";
			ViewBag.Token = token.Token;

			return Email( "VerificationEmail" );
		}
	}
}