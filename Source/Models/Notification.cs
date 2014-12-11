using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RationalVote.Models
{
	public class Notification
	{
		public enum Type
		{
			Vote,
			Reply
		}

		public long Id { get; set; }

		public long AssociatedPost { get; set; }
		public long AssociatedProfileId { get; set; }
		public Type NotificationType { get; set; }
		public DebateLink.LinkType Position { get; set; }
		public DateTime Time { get; set; }
		public bool Read { get; set; }

		public string Joined_ProfileName{ get; set; }
		public string Joined_ProfileImage{ get; set; }
	}
}