using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RationalVote.Models
{
	public class LoginFacebook
	{
		public string AccessToken { get; set; }
		public string ExpiresIn { get; set; }
		public string SignedRequest { get; set; }
		public string UserID { get; set; }
	}
}