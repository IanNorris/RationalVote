using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RationalVote.Models
{
	public class DebateContainer
	{
		public Debate Debate { get; set; }
		public IEnumerable<DebateLink> Children { get; set; }
		public VoteSplit VoteSplit { get; set; }
	}
}