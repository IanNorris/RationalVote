namespace RationalVote.Models
{
	using System;
	using System.Collections.Generic;
	
	public partial class DebateLink
	{
		public enum LinkType : byte
		{
			For,
			Against,
		}

		public DebateLink()
		{
		}
	
		public long Id { get; set; }
		public LinkType Type { get; set; }
		public long Weight { get; set; }
	
		public long Parent { get; set; }

		public Debate Child { get; set; }

		//These values are JOINed in
		public string Tags { get; set; }
		public byte Vote { get; set; }
	}
}
