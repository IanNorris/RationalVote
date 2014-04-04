using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Common;
using System.Web;
using RationalVote.DAL;
using RationalVote.Models;
using Dapper;

namespace RationalVote.Models
{    
	public partial class Session
	{
		static int LIFE_SHORT	= 360;			//1 day in seconds
		static int LIFE_LONG	= 31536000;		//1 year in seconds

		public Session()
		{
			this.KeepLoggedIn = false;
		}

		public static Session CreateSession( User user, HttpRequestBase request, bool keepLoggedIn )
		{
			Session session = new Session();
			session.Token = Utility.Crypto.GenerateSaltString( 32 );
			session.IP = Utility.Client.GetClientIP( request );
			session.KeepLoggedIn = keepLoggedIn;
			session.LastSeen = DateTime.Now;
			session.Life = keepLoggedIn ? LIFE_LONG : LIFE_SHORT;
			session.User = user.Id;

			using( DbConnection connection = RationalVoteContext.Connect() )
			{
				long Id = connection.Insert( session );
				session.Id = Id;
			}

			Utility.Cookie.SetCookie( "s_id", session.Id.ToString(), session.Life );
			Utility.Cookie.SetCookie( "s_tok", session.Token.ToString(), session.Life );

			return session;
		}

		public static User ValidateAndUpdateSession()
		{
			string id_string = Utility.Cookie.GetCookie( "s_id" );
			string token = Utility.Cookie.GetCookie( "s_tok" );

			if( token == null || id_string == null )
			{
				return null;
			}
			
			long id;
			try
			{
				id = Convert.ToInt64( id_string );
			}
			catch( OverflowException)
			{
				return null;
			}
			catch( FormatException)
			{
				return null;
			}

			using( DbConnection connection = RationalVoteContext.Connect() )
			{
				//Find the valid sessions

				//SQL Server
				//"UPDATE Session SET LastSeen = @CurrentDate OUTPUT INSERTED.* WHERE Token = @Token AND Id = @Id AND DATEADD(SECOND, Life, LastSeen) >= GETDATE()"
								
				User user = connection.Query<User>(
					@"SET @SelectedUser := NULL; 
					
					UPDATE
						RationalVote.Session 
					SET 
						LastSeen=NOW(), 
						User = (SELECT @SelectedUser := User) 
					WHERE 
						Id = @Id
						AND 
						Token = @Token
						AND (DATE_ADD(LastSeen, INTERVAL Life SECOND) >= NOW())
					LIMIT 1;

					DELETE FROM 
						RationalVote.Session 
					WHERE 
						User = @SelectedUser
						AND
						(DATE_ADD(LastSeen, INTERVAL Life SECOND) < NOW());

					SELECT * FROM User WHERE Id = @SelectedUser"
					, new { Token = token, Id = id } ).FirstOrDefault();

				return user;
			}
		}
	
		public long Id { get; set; }
		public string Token { get; set; }
		public string IP { get; set; }
		public bool KeepLoggedIn { get; set; }
		public System.DateTime LastSeen { get; set; }
		public int Life { get; set; }
		public long User { get; set; }
	}
}
