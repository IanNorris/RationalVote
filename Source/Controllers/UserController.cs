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

namespace RationalVote
{
	[RoutePrefix("User")]
	public class UserController : Controller
	{
		// GET: /User/
		[RequireLogin]
		public ActionResult Index()
		{
			using( DbConnection connection = RationalVoteContext.Connect() )
			{
				IEnumerable<User> users = connection.Query<User>( "SELECT * FROM User" );

				return View( users.ToList() );
			}
			
		}

		public ActionResult SignIn( string returnUrl )
		{
			ViewBag.ReturnUrl = returnUrl;

			return View();
		}

		// GET: /User/Details/5
		public ActionResult Details(long? id)
		{
			/*if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			User user = db.Users.Find(id);
			if (user == null)
			{
				return HttpNotFound();
			}
			return View(user);*/
			return View();
		}

		// POST: /User/Register
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
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
							connection.Insert( profile, transaction );

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
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Login( UserLogin userPublic, string returnUrl )
		{
			ViewBag.ReturnUrl = returnUrl;

			if( ModelState.IsValid )
			{
				using( DbConnection connection = RationalVoteContext.Connect() )
				{
					User storedUser = connection.Query<User>( "SELECT * FROM User WHERE Email = @Email", new { Email = userPublic.LoginEmail.ToLower() } ).FirstOrDefault();

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
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route( "ForgotPasswordPost" )]
		public ActionResult ForgotPassword( ForgotPassword forgot )
		{
			if( ModelState.IsValid )
			{
				using( DbConnection connection = RationalVoteContext.Connect() )
				{
					User user = connection.Query<User>( "SELECT * FROM User WHERE Email = @Email", new { Email = forgot.RegisterEmail } ).FirstOrDefault();

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
		[HttpPost]
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

		// GET: /User/Edit/5
		public ActionResult Edit(long? id)
		{
			/*if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			User user = db.Users.Find(id);
			if (user == null)
			{
				return HttpNotFound();
			}
			ViewBag.Id = new SelectList(db.EmailVerificationTokens, "Id", "Id", user.Id);
			ViewBag.Id = new SelectList(db.Profiles, "Id", "DisplayName", user.Id);
			return View(user);*/
			return View();
		}

		// POST: /User/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include="Id,Email,PasswordSalt,PasswordHash,AuthenticationMethod,Verified")] User user)
		{
			/*if (ModelState.IsValid)
			{
				db.Entry(user).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			ViewBag.Id = new SelectList(db.EmailVerificationTokens, "Id", "Id", user.Id);
			ViewBag.Id = new SelectList(db.Profiles, "Id", "DisplayName", user.Id);
			return View(user);*/
			return View();
		}

		// GET: /User/Delete/5
		public ActionResult Delete(long? id)
		{
			/*if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			User user = db.Users.Find(id);
			if (user == null)
			{
				return HttpNotFound();
			}
			return View(user);*/
			return View();
		}

		// POST: /User/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(long id)
		{
			/*User user = db.Users.Find(id);
			db.Users.Remove(user);
			db.SaveChanges();*/
			return RedirectToAction("Index");
		}

		protected override void Dispose(bool disposing)
		{
			/*if (disposing)
			{
				db.Dispose();
			}*/
			base.Dispose(disposing);
		}
	}
}
