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
			System,
			Vote,
			Reply
		}

		public enum Status
		{
			Unseen,
			Seen,
			Read,
		}

		public long Id { get; set; }
		public long Receiver { get; set; }
		public long Sender { get; set; }
		public Type Reason { get; set; }
		public string Message { get; set; }
		public Status State { get; set; }
		public DateTime Sent { get; set; }

		public string Joined_ProfileName { get; set; }
		public string Joined_ProfileImage { get; set; }
	}
}