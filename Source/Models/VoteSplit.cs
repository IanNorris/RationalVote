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

namespace RationalVote.Models
{
	public class VoteSplit
	{
		public long For			{ get; set; }
		public long Assumption	{ get; set; }
		public long Against		{ get; set; }

		public long Total()
		{
			return For - Against;
		}

		public static VoteSplit Get( long Parent )
		{
			using( DbConnection connection = RationalVoteContext.Connect() )
			{
				VoteSplit split = connection.Query<VoteSplit>(
					@"SELECT
					SUM( CASE WHEN Debate.Status = 0 THEN 1 ELSE 0 END ) AS `For`,
					SUM( CASE WHEN (Debate.Status = 1 OR Debate.Status = 2) THEN 1 ELSE 0 END ) AS `Assumption`,
					SUM( CASE WHEN Debate.Status = 3 THEN 1 ELSE 0 END ) AS `Against`
					FROM
						DebateLink
					INNER JOIN Debate
					ON DebateLink.Child = Debate.Id
					WHERE DebateLink.Parent = @Parent
					GROUP BY DebateLink.Parent", 
					new { Parent = Parent } ).FirstOrDefault();

				if( split == null )
				{
					split = new VoteSplit();
				}

				return split;
			}
		}
	}
}