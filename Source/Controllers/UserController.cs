using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RationalVote.Models;
using System.Data.SqlClient;
using Dapper;
using DapperExtensions;
using RationalVote.DAL;

namespace RationalVote
{
	[RoutePrefix("User")]
	public class UserController : Controller
	{
		// GET: /User/
		public ActionResult Index()
		{
			using( SqlConnection connection = RationalVoteContext.Connect() )
			{			
				IEnumerable<User> users = connection.Query<User>("SELECT * FROM Users");

				return View( users.ToList() );
			}
		}

		public ActionResult SignIn()
		{
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
		public ActionResult Register( UserRegister userPublic )
		{
			if (ModelState.IsValid)
			{
				try
				{
					using( SqlConnection connection = RationalVoteContext.Connect() )
					{
						using( SqlTransaction transaction = connection.BeginTransaction() )
						{
							//Create user
							User user = new User( userPublic );
							byte[] salt;
							byte[] hash;
							Utility.Crypto.CreatePasswordHash( user.Email, userPublic.RegisterPassword, out salt, out hash );
							user.PasswordSalt = salt;
							user.PasswordHash = hash;

							//Add the user
							user.Id = connection.Insert( user, transaction );

							//Create verification token
							EmailVerificationToken verificationToken = EmailVerificationToken.CreateNew( user );
							connection.Insert( verificationToken, transaction );

							//Create profile
							Profile profile = RationalVote.Models.Profile.CreateNew( user );
							connection.Insert( profile, transaction );

							new Controllers.MailController().VerificationEmail( user, verificationToken ).Deliver();

							transaction.Commit();

							return RedirectToAction("Index");
						}
					}
				}
				catch( SqlException exception )
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
		public ActionResult Login( UserLogin userPublic )
		{
			if( ModelState.IsValid )
			{
				using( SqlConnection connection = RationalVoteContext.Connect() )
				{
					User storedUser = connection.Query<User>( "SELECT * FROM Users WHERE Email = @Email", new { Email = userPublic.LoginEmail.ToLower() } ).FirstOrDefault();

					if( storedUser != null && Utility.Crypto.ConfirmPasswordHash( storedUser.Email, userPublic.LoginPassword, storedUser.PasswordSalt, storedUser.PasswordHash ) )
					{
						return RedirectToAction( "Index", "Home" );
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

		// GET: /User/Verify/token
		[Route("Verify/{token?}")]
		public ActionResult Verify( string token )
		{
			using( SqlConnection connection = RationalVoteContext.Connect() )
			{
				EmailVerificationToken resultToken = connection.Query<EmailVerificationToken>( "SELECT * FROM EmailVerificationTokens WHERE Token = @Token", new { Token = token } ).FirstOrDefault();

				if( resultToken == null )
				{
					TempData[ "WarningMessage" ] = "This validation token no longer exists. It may have expired, already have been used or is invalid.";
					TempData[ "MessageTitle" ] = "E-mail verification failed";
				}
				else
				{
					using( SqlTransaction transaction = connection.BeginTransaction() )
					{
						connection.Execute( "UPDATE Users SET Verified = 1 WHERE Id = @Id", new { Id = resultToken.User }, transaction );

						connection.Delete<EmailVerificationToken>( resultToken, transaction );

						transaction.Commit();
						
						TempData[ "SuccessMessage" ] = "Thank you for verifying your email address, you may now login to your account!";
						TempData[ "MessageTitle" ] = "E-mail address verified";
					}
				}
			}

			return RedirectToAction("Index");
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
