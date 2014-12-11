using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RationalVote.Startup))]
namespace RationalVote
{
	public partial class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			ConfigureAuth(app);

			ViewEngines.Engines.Clear();
			ViewEngines.Engines.Add( new RazorViewEngine() );

			HubConfiguration hubConfiguration = new HubConfiguration();
#if DEBUG
			hubConfiguration.EnableDetailedErrors = true;
#else
			hubConfiguration.EnableDetailedErrors = false;
#endif
			hubConfiguration.EnableJavaScriptProxies = true;
			//app.MapSignalR<NotificationConnection>( "/Notifications", hubConfiguration );
			app.MapSignalR( hubConfiguration );
		}
	}
}
