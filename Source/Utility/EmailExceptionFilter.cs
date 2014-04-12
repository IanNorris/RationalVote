using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RationalVote;

public class EmailExceptionFilter : HandleErrorAttribute
{
	public override void OnException(ExceptionContext filterContext)
	{
		if( !filterContext.HttpContext.IsCustomErrorEnabled )
		{
			return;
		}

		HttpException httpException = new HttpException( null, filterContext.Exception );

		if( httpException.GetHttpCode() != 500 )
		{
			return;
		}

		if( !ExceptionType.IsInstanceOfType( filterContext.Exception ) )
		{
			return;
		}

		new RationalVote.Controllers.MailController().ExceptionEmail( httpException, filterContext.Exception.Message ).Deliver();
	}
}