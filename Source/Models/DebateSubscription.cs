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
	public partial class DebateSubscription
	{
		public enum Subscription
		{
			Nothing					= 0,													//Unsubscribe

			SwingOnly				= 1<<0,													//Notify only if something changes the debate result

			ImmediatePositiveOnly	= 1<<1|SwingOnly,										//Notify of any positive changes to this debate only
			ImmediateNegativeOnly	= 1<<2|SwingOnly,										//Notify of any negative changes to this debate only

			ImmediateOnly			= ImmediatePositiveOnly|ImmediateNegativeOnly|SwingOnly,//Notify of changes only to this debate, not a child debate

			ChildPositive			= 1<<3|SwingOnly,										//Notify of a positive change to children
			ChildNegative			= 1<<4|SwingOnly,										//Notify of a negative change to children

			Children				= ChildPositive|ChildNegative,							//Notify of any change to children

			Parent					= 1<<5,													//Notify if this debate contributes to a future debate

			Everything				= ImmediateOnly|Children|Parent,						//Notify of anything related to this debate
		}

		public long Id { get; set; }
		public long Debate { get; set; }
		public long User { get; set; }
	}

	//insert into rationalvote.debatesubscription (debate,user) values (40,23);

	//delete from rationalvote.debatesubscription where  debate=40 and user=23;

	//INSERT INTO rationalvote.debatesubscription (debate,user,type) VALUES (40,23,7) ON DUPLICATE KEY UPDATE Type=VALUES(Type);
}
