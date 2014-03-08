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
	public class UserRegister
	{
		[Placeholder( "E-mail address" )]
		[DataType( DataType.EmailAddress )]
		[Required( ErrorMessage = "An e-mail address is required." )]
		[EmailAddress( ErrorMessage = "Invalid e-mail address." )]
		[AddOnIcon( "envelope" )]
		[StringLength( 254 )]
		public string RegisterEmail { get; set; }

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

		[Required( ErrorMessage = "Please accept the Terms of Service." )]
		[MustBeTrue( ErrorMessage = "Please accept the Terms of Service." )]
		public bool RegisterAcceptedTermsOfService { get; set; }

		[Required( ErrorMessage = "Please read and understand the Privacy Policy." )]
		[MustBeTrue( ErrorMessage = "Please read and understand the Privacy Policy." )]
		public bool RegisterAcceptedPrivacyPolicy { get; set; }
	}

	public class UserLogin
	{
		[Placeholder( "E-mail address" )]
		[DataType( DataType.EmailAddress )]
		[Required( ErrorMessage = "An e-mail address is required." )]
		[EmailAddress( ErrorMessage = "Invalid e-mail address." )]
		[AddOnIcon( "envelope" )]
		[StringLength( 254 )]
		public string LoginEmail { get; set; }

		[Placeholder( "Password" )]
		[DataType( DataType.Password )]
		[AddOnIcon( "lock" )]
		public string LoginPassword { get; set; }

		public bool LoginStaySignedIn { get; set; }
	}
}