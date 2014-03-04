
//Had to use System.Web.Mvc.Compare instead of just Compare
//because Compare is buggy (not displaying the right error
//message on the client side for validation. Unfortunately
//the old one is deprecated.
#pragma warning disable 618

namespace RationalVote.Models
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data;
	
	public partial class User
	{
		public User()
		{
			this.AuthenticationMethod = 0;
			this.Verified = false;
			this.Sessions = new HashSet<Session>();
			this.Debates = new HashSet<Debate>();
			this.Votes = new HashSet<DebateLinkVote>();
			this.Permissions = new HashSet<Permission>();
		}
	
		public long Id { get; set; }

		[Placeholder("E-mail address")]
		[DataType(DataType.EmailAddress)]
		[Required( ErrorMessage = "An e-mail address is required." )]
		[EmailAddress( ErrorMessage = "Invalid e-mail address." )]
		[AddOnIcon( "envelope")]
		[StringLength(254)] 
		public string Email { get; set; }

		[MaxLength(64)]
		public byte[] PasswordSalt { get; set; }

		[MaxLength( 64 )]
		public byte[] PasswordHash { get; set; }

		public byte AuthenticationMethod { get; set; }
		public bool Verified { get; set; }

		//NotMapped fields - data that don't get stored in the DB

		[NotMapped]
		[Placeholder("Password")]
		[DataType(DataType.Password)]
		[AddOnIcon("lock")]
		public string Password { get; set; }

		[NotMapped]
		[Placeholder( "Re-type password" )]
		[DataType( DataType.Password )]
		[Required( ErrorMessage = "A password is required." )]
		[System.Web.Mvc.Compare("Password", ErrorMessage = "Passwords do not match.")]
		[AddOnIcon("lock")]
		public string PasswordRetype { get; set; }

		[NotMapped]
		[Required( ErrorMessage = "Please accept the Terms of Service." )]
		[MustBeTrue( ErrorMessage = "Please accept the Terms of Service.")]
		public bool AcceptedTermsOfService {  get; set; }

		[NotMapped]
		[Required( ErrorMessage = "Please read and understand the Privacy Policy." )]
		[MustBeTrue( ErrorMessage = "Please read and understand the Privacy Policy.")]
		public bool AcceptedPrivacyPolicy {  get; set; }
	
		public virtual ICollection<Session> Sessions { get; set; }
		public virtual Profile Profiles { get; set; }
		public virtual ICollection<Debate> Debates { get; set; }
		public virtual ICollection<DebateLinkVote> Votes { get; set; }
		public virtual EmailVerificationToken EmailVerificationTokens { get; set; }
		public virtual ICollection<Permission> Permissions { get; set; }
	}
}

#pragma warning restore 618