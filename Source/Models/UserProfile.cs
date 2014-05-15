using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RationalVote.Models
{
	public class UserProfile
	{
		public long Id { get; set; }

		public User User { get; set; }
		public Profile Profile { get; set; }

		public string GetAvatarLink( int dimensions )
		{
			string avatarLink = "";

			if( User.AuthenticationMethod == RationalVote.Models.User.AuthenticationMethodType.Local )
			{
				string emailHash = Utility.Crypto.CalculateMD5Hash( User.Email ).ToLower();
				avatarLink = "//www.gravatar.com/avatar/" + emailHash + "?r=g&d=mm&s=" + dimensions;
			}
			else if( User.AuthenticationMethod == RationalVote.Models.User.AuthenticationMethodType.Facebook )
			{
				avatarLink = "//graph.facebook.com/" + User.PasswordSalt + "/picture?height=" + dimensions + "&type=square&width=" + dimensions;
			}

			return avatarLink;
		}
	}
}