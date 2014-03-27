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
		public enum DebateType : short
		{
			Against,
			For
		}

		public enum StatusType : short
		{
			Accepted,
			Assumption,
			Open,
			Disproven,
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
				case StatusType.Assumption:
				case StatusType.Open:
					return ValidityType.Ambiguous;
				case StatusType.Disproven:
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
				case StatusType.Assumption:
				case StatusType.Open:
					return "question";
				case StatusType.Disproven:
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
						DebateLinks.Parent_Id = @Parent",
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
