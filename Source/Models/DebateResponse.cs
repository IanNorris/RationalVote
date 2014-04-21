namespace RationalVote.Models
{
	using System;
	using System.Collections.Generic;

	public partial class DebateResponse
	{
		public DebateResponse( DebateLink.LinkType type, long parent )
		{
			Type = type;
			Parent = parent;
		}

		public DebateResponse()
		{
		}

		public DebateLink.LinkType Type { get; set; }
		public long Parent { get; set; }
		public string Argument { get; set; }
	}
}
