using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RationalVote.Models;
using System.Data.SqlClient;
using Dapper;
using DapperExtensions;
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
			using( SqlConnection connection = RationalVoteContext.Connect() )
			{
				VoteSplit split = connection.Query<VoteSplit>(
					@"SELECT
					SUM( CASE WHEN Debates.Status = 0 THEN DebateLinks.Weight ELSE 0 END ) AS [For],
					SUM( CASE WHEN (Debates.Status = 1 OR Debates.Status = 2) THEN DebateLinks.Weight ELSE 0 END ) AS [Assumption],
					SUM( CASE WHEN Debates.Status = 3 THEN DebateLinks.Weight ELSE 0 END ) AS [Against]
					FROM
						DebateLinks
					INNER JOIN Debates
					ON DebateLinks.Child_Id = Debates.Id
					WHERE DebateLinks.Parent_Id = @Parent
					GROUP BY DebateLinks.Parent_Id", 
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