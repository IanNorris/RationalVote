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
using System.ComponentModel;

namespace RationalVote.Models
{
	public class Profile
	{
		public Profile()
		{
			this.Experience = 0;
		}

		[Dapper.Key]
		public long UserId { get; set; }

		[AddOnButton( "check", null )]
		[Placeholder("Display Name")]
		[UserCanEdit(true)]
		[Description("Display Name")]
		[StringLength( 64, ErrorMessage="Maximum length is 64" )]
		public string DisplayName { get; set; }

		[AddOnButton( "check", null )]
		[UserCanEdit(true)]
		[Placeholder("Real Name")]
		[Description("Real Name")]
		[StringLength( 64, ErrorMessage="Maximum length is 64" )]
		public string RealName { get; set; }

		[AddOnButton( "check", null )]
		[UserCanEdit(true)]
		[Placeholder("Occupation")]
		[Description("Occupation")]
		[StringLength( 64, ErrorMessage="Maximum length is 64" )]
		public string Occupation { get; set; }

		[AddOnButton( "check", null )]
		[UserCanEdit(true)]
		[Placeholder("Location")]
		[Description("Location")]
		[StringLength( 128, ErrorMessage="Maximum length is 128" )]
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

		public double LevelDouble()
		{
			const double LevelScale = 1.2;

			return Math.Log( Math.Max( Experience, 1 ), LevelScale );
		}

		public long Level()
		{
			return (long)LevelDouble();
		}

		public double LevelPercent()
		{
			double current = LevelDouble();
			return current - Math.Floor(current);
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
