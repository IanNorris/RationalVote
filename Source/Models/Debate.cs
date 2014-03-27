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
	public partial class Debate
	{
		//The order of the elements in this enum
		//control the sort order
		public enum StatusType : short
		{
			Accepted,
			Open,
			Assumption,
			Rejected,
			Noise,
		}

		public enum ValidityType
		{
			Valid,
			Invalid,
			Ambiguous,
			Noise,
		}

		public Debate()
		{
			this.Status = StatusType.Open;
		}
	
		public long Id { get; set; }
		public StatusType Status { get; set; }
		public string Title { get; set; }
		public System.DateTime Posted { get; set; }
		public System.DateTime Updated { get; set; }
		public long Owner_Id { get; set; }
		public bool Locked { get; set; }

		public ValidityType Validity()
		{
			switch( Status )
			{
				case StatusType.Accepted:
					return ValidityType.Valid;
				case StatusType.Open:
				case StatusType.Assumption:
					return ValidityType.Ambiguous;
				case StatusType.Rejected:
					return ValidityType.Invalid;
				case StatusType.Noise:
					return ValidityType.Noise;
				default:
					return ValidityType.Ambiguous;
			}
		}

		public string StatusIcon()
		{
			switch( Status )
			{
				case StatusType.Accepted:
					return "check";
				case StatusType.Open:
				case StatusType.Assumption:
					return "question";
				case StatusType.Rejected:
					return "times";
				case StatusType.Noise:
					return "trash-o";
				default:
					return "question";
			}
		}

		public string StatusName()
		{
			return Status.ToString();
		}

		public IEnumerable<DebateLink> Children()
		{
			using( SqlConnection connection = RationalVoteContext.Connect() )
			{
				//To limit selection, do SELECT TOP 10 etc

				IEnumerable<DebateLink> arguments = connection.Query<DebateLink, Debate, DebateLink>( 
					@"SELECT
						DebateLinks.*, Debates.*, DebateLinkVotes.vote AS Vote
					FROM
						DebateLinks
							LEFT OUTER JOIN
						Debates ON DebateLinks.Child_Id = Debates.Id
							LEFT OUTER JOIN
						DebateLinkVotes ON (DebateLinkVotes.Link_Id = DebateLinks.Id)
					WHERE
						DebateLinks.Parent_Id = @Parent
					ORDER BY Debates.Status ASC, DebateLinks.Weight DESC",
					( parent, child) =>
					{
						parent.Child = child;
						return parent;
					},
					new { Parent = Id }
					);

				return arguments;
			}
		}
	}
}
