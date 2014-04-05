using System.Web;
using System.Web.Mvc;

namespace RationalVote
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters( GlobalFilterCollection filters )
		{
			filters.Add( new EmailExceptionFilter() );
			filters.Add( new HandleErrorAttribute() );
		}
	}
}
