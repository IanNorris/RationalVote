namespace RationalVote.Models
{
	using System;
	using System.Collections.Generic;
	
	public partial class DebateLink
	{
		public enum LinkType : int
		{
			For,
			Against,
		}

		public DebateLink()
		{
		}
	
		public long Id { get; set; }
		public LinkType Type { get; set; }

		public System.DateTime LinkTime { get; set; }
	
		public long Parent { get; set; }

		public Debate Child { get; set; }

		public long LocalFor { get; set; }
		public long LocalAgainst { get; set; }
		public long Weight { get; set; }

		//These values are JOINed in
		public string Tags { get; set; }
		public DebateLinkVote.VoteType Vote { get; set; }
	}
}
