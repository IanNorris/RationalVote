using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Security.Principal;

namespace RationalVote.Models
{
	public class UserPrincipal : IPrincipal
	{
		public UserPrincipal( UserProfile user )
		{
			this.User = user;
			this.Identity = new GenericIdentity( user.User.Email );
		}

		public IIdentity Identity { get; private set; }
		public bool IsInRole( string role ) { return false; }

		public UserProfile User { get; private set; }
	}
}