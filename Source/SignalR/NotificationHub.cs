using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace RationalVote.SignalR
{
	public class NotificationHub : Hub<INotificationClient>
	{
		/*public void OnConnect( string message )
		{

		}*/

		//Functions that the client can call go here
	}

	public interface INotificationClient
	{
		void OnCountUpdated( uint newMessageCount );
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