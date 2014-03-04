namespace RationalVote.Models
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	
	public class Profile
	{
		public Profile()
		{
			this.Experience = 0;
		}
	
		[Key]
		public long Id { get; set; }
		public string DisplayName { get; set; }
		public string RealName { get; set; }
		public string Occupation { get; set; }
		public string Location { get; set; }

		[DataType(DataType.DateTime)]
		public System.DateTime Joined { get; set; }

		public long Experience { get; set; }
	
		[Required]
		[ForeignKey("Id")]
		public virtual User User { get; set; }

		public static Profile CreateNew( User user )
		{
			Profile profile = new Profile();
			profile.User = user;
			profile.Joined = DateTime.Now;
			profile.Experience = 0;

			return profile;
		}
	}
}
