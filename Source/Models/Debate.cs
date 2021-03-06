using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RationalVote.Models;
using Dapper;
using RationalVote.DAL;
using System.Data.Common;

namespace RationalVote.Models
{
	public partial class Debate
	{
		public const int MaxPerPage = 10;
		public const int MaxPages = 20;
		public const int MaxChildrenPerFetch = 4;

		//The order of the elements in this enum
		//control the sort order
		public enum StatusType : sbyte
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
		public System.DateTime? Updated { get; set; }
		public long? Owner { get; set; }
		public bool Locked { get; set; }
		public long WeightFor { get; set; }
		public long WeightAgainst { get; set; }

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

		public string ValidityExplanation()
		{
			switch( Status )
			{
				case StatusType.Accepted:
					return "This argument has been accepted.";
				case StatusType.Open:
					return "No consensus has been reached on this debate.";
				case StatusType.Assumption:
					return "This argument is an assumption or hinges on too many assumptions.";
				case StatusType.Rejected:
					return "This argument has been disproved or deemed not relevant.";
				case StatusType.Noise:
					return "This argument adds no value to the discussion or has been flagged as spam.";
				default:
					return "The status of this debate is unknown.";
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
	}
}
