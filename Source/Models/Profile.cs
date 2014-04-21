using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RationalVote.Models;
using System.Data.Common;
using Dapper;
using RationalVote.DAL;
using System.ComponentModel.DataAnnotations;

namespace RationalVote.Models
{
	public class Profile
	{
		public Profile()
		{
			this.Experience = 0;
		}

		public long UserId { get; set; }
		public string DisplayName { get; set; }
		public string RealName { get; set; }
		public string Occupation { get; set; }
		public string Location { get; set; }

		[DataType(DataType.DateTime)]
		public System.DateTime Joined { get; set; }

		public long Experience { get; set; }

		public static Profile CreateNew( User user )
		{
			Profile profile = new Profile();
			profile.UserId = user.Id;
			profile.Joined = DateTime.Now;
			profile.Experience = 0;

			return profile;
		}

		public long Level()
		{
			const double LevelScale = 1.2;

			return (long)Math.Log( Experience, LevelScale );
		}

		public static Profile GetFromUser( long? Id )
		{
			if( Id == null )
			{
				return null;
			}

			using( DbConnection connection = RationalVoteContext.Connect() )
			{
				Profile profile = connection.Query<Profile>( "SELECT * FROM Profile WHERE `UserId` = @UserId", new { UserId = Id.Value } ).FirstOrDefault();

				return profile;
			}
		}
	}
}
