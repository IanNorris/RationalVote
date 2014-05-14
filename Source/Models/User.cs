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
	public partial class User
	{
		public enum AuthenticationMethodType : int
		{
			Local,
			Facebook,
		}

		public User()
		{
			this.AuthenticationMethod = User.AuthenticationMethodType.Local;
			this.Verified = 0;
		}

		public User( UserRegister user )
		{
			this.AuthenticationMethod = User.AuthenticationMethodType.Local;
			this.Verified = 0;

			this.Email = user.RegisterEmail.ToLower();
		}
	
		public long Id { get; set; }

		[Placeholder("E-mail address")]
		[DataType(DataType.EmailAddress)]
		[Required( ErrorMessage = "An e-mail address is required." )]
		[EmailAddress( ErrorMessage = "Invalid e-mail address." )]
		[AddOnIcon( "envelope")]
		public string Email { get; set; }

		public string PasswordSalt { get; set; }
		public string PasswordHash { get; set; }
		public AuthenticationMethodType AuthenticationMethod { get; set; }
		public int Verified { get; set; }
	}
}