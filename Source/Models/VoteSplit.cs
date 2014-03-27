using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RationalVote.Models
{
	public class VoteSplit
	{
		public int For			{ get; set; }
		public int Assumption	{ get; set; }
		public int Against		{ get; set; }
		public int Noise		{ get; set; }

		public int Total		{ get; set; }

		public static VoteSplit Get( long ChildId )
		{
			/*using( SqlConnection connection = RationalVoteContext.Connect() )
			{
				VoteSplit split = connection.Query<VoteSplit>( "SELECT * FROM Profiles WHERE [User] = @ChildId", new { ChildId = ChildId } ).FirstOrDefault();

				return profile;
			}*/

			return new VoteSplit();
		}
	}
}