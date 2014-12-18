using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using RationalVote.DAL;
using RationalVote.Models;
using Dapper;
using System.Threading.Tasks;
using System.Data.Common;

namespace RationalVote.SignalR
{
	public class NotificationHub : Hub<INotificationClient>
	{
		public override Task OnConnected()
		{
			using( DbConnection connection = RationalVoteContext.Connect() )
			{
				int count = connection.Query<int>(
					@"SELECT COUNT(*) AS Count FROM rationalvote.notification WHERE Receiver = @User GROUP BY Receiver;",
					new { User = ((RationalVote.Models.UserPrincipal)this.Context.User).User.Id } ).FirstOrDefault();

				Clients.User( this.Context.User.Identity.Name ).OnCountUpdated( (uint)count, false );
			}

			

			return base.OnConnected();
		}

		/*public void OnConnect( string message )
		{

		}*/

		//Functions that the client can call go here
	}

	public interface INotificationClient
	{
		void OnCountUpdated( uint newMessageCount, bool initial );
	}

	public class NotifyContext
	{
		// Singleton instance
		private readonly static Lazy<NotifyContext> _instance = new Lazy<NotifyContext>(
		() => new NotifyContext(GlobalHost.ConnectionManager.GetHubContext<NotificationHub>()));

		private IHubContext _context;

		private NotifyContext( IHubContext context )
		{
			_context = context;
		}
	}
}