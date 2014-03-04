using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;

namespace Utility
{
	public class SendMail
	{
		/// <summary>
		/// Sends an mail message
		/// </summary>
		/// <param name="from">Sender address</param>
		/// <param name="to">Recepient address</param>
		/// <param name="bcc">Bcc recipient</param>
		/// <param name="cc">Cc recipient</param>
		/// <param name="subject">Subject of mail message</param>
		/// <param name="body">Body of mail message</param>
		/// <param name="html">Is the body HTML?</param>
		public static void SendMailMessage( string from, string to, string bcc, string cc, string subject, string body, bool html )
		{
			MailMessage message = new MailMessage();

			message.From = new MailAddress( from );
			message.To.Add( new MailAddress( to ) );
			message.Subject = subject;
			message.Body = body;
			message.IsBodyHtml = html;
			message.Priority = MailPriority.Normal;

			if( ( bcc != null ) && ( bcc != string.Empty ) )
			{
				message.Bcc.Add( new MailAddress( bcc ) );
			}

			if( ( cc != null ) && ( cc != string.Empty ) )
			{
				message.CC.Add( new MailAddress( cc ) );
			}

			SmtpClient smtp = new SmtpClient();
			smtp.Send( message );
		}
	}
}