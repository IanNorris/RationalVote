// Uncomment this class to provide custom runtime policy for Glimpse

using System.Configuration;
using Glimpse.AspNet.Extensions;
using Glimpse.Core.Extensibility;

namespace RationalVote
{
	public class GlimpseSecurityPolicy:IRuntimePolicy
	{
		public RuntimePolicy Execute(IRuntimePolicyContext policyContext)
		{
			// You can perform a check like the one below to control Glimpse's permissions within your application.
			// More information about RuntimePolicies can be found at http://getglimpse.com/Help/Custom-Runtime-Policy
			var httpContext = policyContext.GetHttpContext();

			//TODO: Replace this with a role based policy system
			string[] glimpseUsers = ConfigurationManager.AppSettings.Get( "GlimpseUsers" ).ToString().ToLower().Split(';');
			if( httpContext.User.Identity.IsAuthenticated )
			{
				foreach( string user in glimpseUsers )
				{
					if( ((RationalVote.Models.UserPrincipal)httpContext.User).User.Email.ToLower().CompareTo( user ) == 0 )
					{
						return RuntimePolicy.On;
					}
				}
			}

			//if (httpContext.User.IsInRole("Administrator"))
			//{
			//	return RuntimePolicy.On;
			//}

			return RuntimePolicy.Off;
		}

		public RuntimeEvent ExecuteOn
		{
			// The RuntimeEvent.ExecuteResource is only needed in case you create a security policy
			// Have a look at http://blog.getglimpse.com/2013/12/09/protect-glimpse-axd-with-your-custom-runtime-policy/ for more details
			get { return RuntimeEvent.EndRequest | RuntimeEvent.ExecuteResource; }
		}
	}
}
