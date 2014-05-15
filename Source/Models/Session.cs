using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Common;
using System.Web;
using RationalVote.DAL;
using RationalVote.Models;
using Dapper;
using System.Data;

namespace RationalVote.Models
{    
	public partial class Session
	{
		static int LIFE_SHORT	= 7200;			//2 hours in seconds
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

		public static UserProfile ValidateAndUpdateSession()
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
								
				UserProfile userObj = connection.Query<UserProfile,User,Profile,UserProfile>(
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

					SELECT User.Id as Id, User.*, '' as Split1, Profile.*, '' as Split2 FROM User 
					INNER JOIN Profile
						ON Profile.UserId = User.Id
					WHERE Id = @SelectedUser",
					(userProfile, user, profile) =>
					{
						userProfile.User = user;
						userProfile.Profile = profile;
						return userProfile;
					},
					new { Token = token, Id = id },
					commandTimeout: null,
					commandType: CommandType.Text, 
					splitOn: "Id,UserId" ).FirstOrDefault();

				return userObj;
			}
		}

		public static void DeleteCurrentSession()
		{
			string id_string = Utility.Cookie.GetCookie( "s_id" );
			string token = Utility.Cookie.GetCookie( "s_tok" );

			using( DbConnection connection = RationalVoteContext.Connect() )
			{
				connection.Execute( "DELETE FROM Session WHERE Id = @Id AND Token = @Token", new { Id = id_string, Token = token } );
			}
		}

		public static void ClearAllSessions( long Id, DbConnection connection, DbTransaction transaction )
		{
			connection.Execute( "DELETE FROM Session WHERE User = @User", new { User = Id }, transaction );
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
