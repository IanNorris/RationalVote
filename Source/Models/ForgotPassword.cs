using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

//Had to use System.Web.Mvc.Compare instead of just Compare
//because Compare is buggy (not displaying the right error
//message on the client side for validation. Unfortunately
//the old one is deprecated.
#pragma warning disable 618

namespace RationalVote.Models
{
	public class ResetPassword
	{
		public string Token { get; set; }

		[Placeholder( "Password" )]
		[DataType( DataType.Password )]
		[AddOnIcon( "lock" )]
		public string RegisterPassword { get; set; }

		[Placeholder( "Re-type password" )]
		[DataType( DataType.Password )]
		[Required( ErrorMessage = "A password is required." )]
		[System.Web.Mvc.Compare( "RegisterPassword", ErrorMessage = "Passwords do not match." )]
		[AddOnIcon( "lock" )]
		public string RegisterPasswordRetype { get; set; }

		public bool ClearSessions { get; set; }
	}

	public class ForgotPassword
	{
		[Placeholder( "E-mail address" )]
		[DataType( DataType.EmailAddress )]
		[Required( ErrorMessage = "An e-mail address is required." )]
		[EmailAddress( ErrorMessage = "Invalid e-mail address." )]
		[AddOnIcon( "envelope" )]
		[StringLength( 254 )]
		public string RegisterEmail { get; set; }
	}

	public class PasswordResetToken
	{
		public long Id { get; set; }
		public long UserId { get; set; }
		public string Token { get; set; }
	}
}