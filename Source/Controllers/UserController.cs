using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RationalVote.Models;
using System.Data.Common;
using Dapper;
using RationalVote.DAL;
using Utility;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace RationalVote
{
	[RoutePrefix("User")]
	public class UserController : Controller
	{
		public static DateTime UnixTimeStampToDateTime( double unixTimeStamp )
		{
			// Unix timestamp is seconds past epoch
			System.DateTime dtDateTime = new DateTime( 1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc );
			dtDateTime = dtDateTime.AddSeconds( unixTimeStamp ).ToLocalTime();
			return dtDateTime;
		}

		public ActionResult SignIn( string returnUrl )
		{
			ViewBag.ReturnUrl = returnUrl;

			return View();
		}

		public ActionResult FacebookLogin( string returnUrl )
		{
			ViewBag.ReturnUrl = returnUrl;

			return View();
		}

		// POST: /User/Register
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost, ValidateInput(false)]
		[ValidateAntiForgeryToken]
		public ActionResult Register( UserRegister userPublic, string returnUrl )
		{
			ViewBag.ReturnUrl = returnUrl;

			if (ModelState.IsValid)
			{
				try
				{
					using( DbConnection connection = RationalVoteContext.Connect() )
					{
						using( DbTransaction transaction = connection.BeginTransaction() )
						{
							//Create user
							User user = new User( userPublic );
							string salt;
							string hash;
							Utility.Crypto.CreatePasswordHash( user.Email, userPublic.RegisterPassword, out salt, out hash );

							user.PasswordSalt = salt;
							user.PasswordHash = hash;

							user.AuthenticationMethod = Models.User.AuthenticationMethodType.Local;

#if DEBUG
							user.Verified = 1;
#endif

							//Add the user
							user.Id = connection.Insert( user, transaction );

							//Create verification token
							EmailVerificationToken verificationToken = EmailVerificationToken.CreateNew( user );
							connection.Insert( verificationToken, transaction );

							//Create profile
							Profile profile = RationalVote.Models.Profile.CreateNew( user );

							connection.Execute( @"INSERT INTO Profile 
												(UserId, DisplayName, RealName, Occupation, Location, ProfileLink, Joined, Experience) 
												VALUES 
												(@UserId, @DisplayName, @RealName, @Occupation, @Location, @ProfileLink, @Joined, @Experience)",
												profile, transaction );

#if !DEBUG
							new Controllers.MailController().VerificationEmail( user, verificationToken ).Deliver();
#endif

							transaction.Commit();

							TempData[ "WarningMessage" ] = "Thank you for registering! An e-mail has been sent to the e-mail address you provided. To log in you will need to click this link and verify your account.";
							TempData[ "MessageTitle" ] = "E-mail verification required";

							return RedirectToAction( "Index", "Home" );
						}
					}
				}
				catch( MySqlException exception )
				{
					switch( RationalVoteContext.DecodeException( exception ) )
					{
						case RationalVoteContext.Error.DuplicateIndex:
							ModelState.AddModelError( "RegisterEmail", "This e-mail address is already registered." );
							break;

						default:
							ModelState.AddModelError( string.Empty, "Unable to create account for unknown reasons." );
							break;
					}
				}
			}

			return View( "SignIn", userPublic );
		}

		// POST: /User/Login
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost, ValidateInput(false)]
		[ValidateAntiForgeryToken]
		public ActionResult Login( UserLogin userPublic, string returnUrl )
		{
			ViewBag.ReturnUrl = returnUrl;

			if( ModelState.IsValid )
			{
				using( DbConnection connection = RationalVoteContext.Connect() )
				{
					User storedUser = connection.Query<User>( "SELECT * FROM User WHERE Email = @Email AND AuthenticationMethod = 0", new { Email = userPublic.LoginEmail.ToLower() } ).FirstOrDefault();

					if( storedUser != null && storedUser.Verified != 0 && Utility.Crypto.ConfirmPasswordHash( storedUser.Email, userPublic.LoginPassword, storedUser.PasswordSalt, storedUser.PasswordHash ) )
					{
						//Create new session for the user
						RationalVote.Models.Session.CreateSession( storedUser, Request, userPublic.LoginStaySignedIn );

						if( Url.IsLocalUrl( returnUrl ) )
						{
							return Redirect( returnUrl );
						}
						else
						{
							return RedirectToAction( "Index", "Home" );
						}
					}
					else
					{
						ModelState.AddModelError( "LoginPassword", "Email and password combination is not correct." );

						userPublic.LoginPassword = "";
						return View( "SignIn", userPublic );
					}
				}
			}

			return View( "SignIn", userPublic );
		}

		// POST: /User/LoginPostFB
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost, ValidateInput(false)]
		[ValidateAntiForgeryToken]
		public ActionResult LoginPostFB( LoginFacebook login, string returnUrl )
		{
			string appSecret = ConfigurationManager.AppSettings.Get("facebookAppSecret");

			string[] signed_components = login.SignedRequest.Split( '.' );

			if( signed_components.Length != 2 )
			{
				throw new HttpException( 500, "Invalid signed request" );
			}

			string encoded_sig = signed_components[ 0 ];
			string payload = signed_components[ 1 ];

			string expected = Convert.ToBase64String( Utility.Crypto.CalculateHMAC256( appSecret, payload ) );

			//Ensure length is multiple of 4, pad with = as spec requires
			int mod4 = encoded_sig.Length % 4;
			if( mod4 > 0 )
			{
				encoded_sig += new string( '=', 4 - mod4 );
			}

			encoded_sig = encoded_sig.Replace('-', '+').Replace('_', '/');

			if( !expected.Equals( encoded_sig, StringComparison.Ordinal ) )
			{
				throw new HttpException( 500, "Invalid signed request" );
			}

			//Ensure length is multiple of 4, pad with = as spec requires
			mod4 = payload.Length % 4;
			if( mod4 > 0 )
			{
				payload += new string( '=', 4 - mod4 );
			}

			byte[] data = Convert.FromBase64String( payload );
			string decodedPayload = System.Text.Encoding.UTF8.GetString( data );

			var decoded_payload = System.Web.Helpers.Json.Decode(decodedPayload);

			DateTime issuedAt = UnixTimeStampToDateTime( decoded_payload.issued_at );

			TimeSpan timeSinceIssued = DateTime.Now - issuedAt;

			if( timeSinceIssued.TotalMinutes > 10 )
			{
				TempData[ "ErrorMessage" ] = "Unable to sign in via Facebook, the issue time is too old. Please try again.";
				TempData[ "MessageTitle" ] = "Facebook login failed";

				return RedirectToAction( "SignIn", "User" );
			}

			if( decoded_payload.user_id != login.UserID )
			{
				throw new HttpException( 500, "Signed user ID does not match unsigned user ID - Possible forgery attempt, IP address logged." );
			}

			try
			{
				var fb = new Facebook.FacebookClient();
				dynamic fb_access_token = fb.Get( "oauth/access_token", new
				{
					client_id = ConfigurationManager.AppSettings.Get("facebookAppId"),
					client_secret = appSecret,
					grant_type = "client_credentials"
				} );

				long facebookId = 0;
				if( !long.TryParse( login.UserID, out facebookId ) )
				{
					throw new Facebook.FacebookOAuthException();
				}

				var client = new Facebook.FacebookClient( fb_access_token.access_token );
				string userID = login.UserID;
				/*string email = client.Get( login.UserID + "/email" ) as string;
				string name = client.Get( login.UserID + "/name" ) as string;
				string location = client.Get( login.UserID + "/location" ) as string;
				string profile_link = client.Get( login.UserID + "/link" ) as string;*/

				dynamic userDetails = client.Get( login.UserID );
				string email = userDetails.email;
				string name = userDetails.name;
				string location = userDetails.location;
				string profile_link = userDetails.link;

				using( DbConnection connection = RationalVoteContext.Connect() )
				{
					User storedUser = connection.Query<User>( "SELECT * FROM User WHERE PasswordSalt = @UserID AND AuthenticationMethod = 1", new { UserID = userID } ).FirstOrDefault();

					if( storedUser != null )
					{
						//Create new session for the user
						RationalVote.Models.Session.CreateSession( storedUser, Request, false );

						if( Url.IsLocalUrl( returnUrl ) )
						{
							return Redirect( returnUrl );
						}
						else
						{
							return RedirectToAction( "Index", "Home" );
						}
					}
					else
					{
						using( DbTransaction transaction = connection.BeginTransaction() )
						{
							//Create user
							User user = new User();

							user.Email = email;
							user.PasswordSalt = userID;
							user.PasswordHash = "UNUSED";
							user.Verified = 1;
							user.AuthenticationMethod = Models.User.AuthenticationMethodType.Facebook;

							//Add the user
							user.Id = connection.Insert( user, transaction );

							//Create profile
							Profile profile = RationalVote.Models.Profile.CreateNew( user );
							profile.DisplayName = name;
							profile.Location = location;
							profile.RealName = name;
							profile.ProfileLink = profile_link;

							connection.Execute( @"INSERT INTO Profile 
												(UserId, DisplayName, RealName, Occupation, Location, ProfileLink, Joined, Experience) 
												VALUES 
												(@UserId, @DisplayName, @RealName, @Occupation, @Location, @ProfileLink, @Joined, @Experience)", 
												profile, transaction );

							transaction.Commit();

							TempData[ "SuccessMessage" ] = "Thank you for registering!";
							TempData[ "MessageTitle" ] = "Registration complete";

							//Create new session for the user
							RationalVote.Models.Session.CreateSession( user, Request, false );

							if( Url.IsLocalUrl( returnUrl ) )
							{
								return Redirect( returnUrl );
							}
							else
							{
								return RedirectToAction( "Index", "Home" );
							}
						}
					}
				}
			}
			catch( Facebook.FacebookOAuthException )
			{
				TempData[ "ErrorMessage" ] = "Unable to sign in via facebook, did you de-authorize the app?";
				TempData[ "MessageTitle" ] = "Facebook login failed";

				return RedirectToAction( "SignIn", "User" );
			}
		}

		// POST: /User/LogOff
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff()
		{
			if( ModelState.IsValid )
			{
				using( DbConnection connection = RationalVoteContext.Connect() )
				{
					RationalVote.Models.Session.DeleteCurrentSession();
				}
			}

			return RedirectToAction( "Index", "Home" );
		}

		// GET: /User/Verify/token
		[Route("Verify/{token?}")]
		public ActionResult Verify( string token )
		{
			using( DbConnection connection = RationalVoteContext.Connect() )
			{
				EmailVerificationToken resultToken = connection.Query<EmailVerificationToken>( "SELECT * FROM EmailVerificationToken WHERE Token = @Token", new { Token = token } ).FirstOrDefault();

				if( resultToken == null )
				{
					TempData[ "WarningMessage" ] = "This validation token no longer exists. It may have expired, already have been used or is invalid.";
					TempData[ "MessageTitle" ] = "E-mail verification failed";
				}
				else
				{
					using( DbTransaction transaction = connection.BeginTransaction() )
					{
						connection.Execute( "UPDATE User SET Verified = 1 WHERE Id = @Id", new { Id = resultToken.User }, transaction );

						connection.Delete<EmailVerificationToken>( resultToken, transaction );

						transaction.Commit();
						
						TempData[ "SuccessMessage" ] = "Thank you for verifying your email address, you may now login to your account!";
						TempData[ "MessageTitle" ] = "E-mail address verified";

						return RedirectToAction("SignIn");
					}
				}
			}

			return RedirectToAction( "Index", "Home" );
		}

		// GET: /User/ForgotPassword
		[Route("ForgotPassword")]
		public ActionResult ForgotPassword()
		{
			return View();
		}

		// GET: /User/ForgotPasswordPost
		[HttpPost, ValidateInput(false)]
		[ValidateAntiForgeryToken]
		[Route( "ForgotPasswordPost" )]
		public ActionResult ForgotPassword( ForgotPassword forgot )
		{
			if( ModelState.IsValid )
			{
				using( DbConnection connection = RationalVoteContext.Connect() )
				{
					User user = connection.Query<User>( "SELECT * FROM User WHERE Email = @Email  AND AuthenticationMethod = 0", new { Email = forgot.RegisterEmail } ).FirstOrDefault();

					if( user != null )
					{
						string token = Utility.Crypto.GenerateSaltString( 32 );

						PasswordResetToken tokenObj = new PasswordResetToken();
						tokenObj.Token = token;
						tokenObj.UserId = user.Id;

						connection.Insert( tokenObj );

						new Controllers.MailController().ResetPasswordEmail( user.Email, token ).Deliver();
					}
				}

				TempData[ "SuccessMessage" ] = "If there was an account associated with this e-mail, it should receive a password reset link shortly.";
				TempData[ "MessageTitle" ] = "Password reset e-mail sent";

				return RedirectToAction( "Index", "Home" );
			}
			else
			{
				return View( "ForgotPassword", forgot );
			}

			
		}

		// GET: /User/ResetPassword/token
		[Route( "ResetPassword/{token}" )]
		public ActionResult ResetPassword( string token )
		{
			ResetPassword reset = new ResetPassword();
			reset.Token = token;

			return View( reset );
		}

		// GET: /User/ResetPasswordPost
		[HttpPost, ValidateInput(false)]
		[ValidateAntiForgeryToken]
		[Route( "ResetPasswordPost" )]
		public ActionResult ResetPassword( ResetPassword reset )
		{
			if( ModelState.IsValid )
			{
				using( DbConnection connection = RationalVoteContext.Connect() )
				{
					PasswordResetToken token = connection.Query<PasswordResetToken>( "SELECT * FROM PasswordResetToken WHERE Token = @Token", new { Token = reset.Token } ).FirstOrDefault();

					if( token != null )
					{
						using( DbTransaction transaction = connection.BeginTransaction() )
						{
							User user = connection.Get<User>( token.UserId );

							connection.Delete<PasswordResetToken>( token, transaction );

							string salt;
							string hash;
							Utility.Crypto.CreatePasswordHash( user.Email, reset.RegisterPassword, out salt, out hash );

							user.PasswordHash = hash;
							user.PasswordSalt = salt;
							user.Verified = 1;

							connection.Update( user, transaction );

							if( reset.ClearSessions )
							{
								RationalVote.Models.Session.ClearAllSessions( user.Id, connection, transaction );
							}

							transaction.Commit();

							TempData[ "SuccessMessage" ] = "Thank you for resetting your password, you may now login to your account!";
							TempData[ "MessageTitle" ] = "Password reset";

							return RedirectToAction( "SignIn" );
						}
					}
					else
					{
						TempData[ "ErrorMessage" ] = "This token has already been used or was never valid.";
						TempData[ "MessageTitle" ] = "Invalid token";
					}
				}

				return View( "ResetPassword", reset );
			}
			else
			{
				return View( "ResetPassword", reset );
			}
		}
	}
}
