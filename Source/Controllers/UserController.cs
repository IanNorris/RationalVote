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
	public class UserController : Controller
	{
		// GET: /User/
		public ActionResult Index()
		{
			using( SqlConnection connection = RationalVoteContext.Connect() )
			{			
				IEnumerable<User> users = connection.Query<User>("select * from Users");

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
		public ActionResult Register( UserPublic userPublic )
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
							Utility.Crypto.CreatePasswordHash( user.Email, userPublic.Password, out salt, out hash );
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
							ModelState.AddModelError( "Email", "This e-mail address is already registered." );
							break;

						default:
							ModelState.AddModelError( string.Empty, "Unable to create account for unknown reasons." );
							break;
					}
				}
			}

			return View( "SignIn", userPublic );
		}

		// GET: /User/Edit/5
		public ActionResult Verify(long? id)
		{
			return View();
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
