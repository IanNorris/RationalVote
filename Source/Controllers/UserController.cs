using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RationalVote.Models;
using Dapper;
using System.Data.SqlClient;

namespace RationalVote
{
	public class UserController : Controller
	{
		// GET: /User/
		public ActionResult Index()
		{
			using( var sqlConnection = new SqlConnection( "Data Source=(LocalDb)\\v11.0;Initial Catalog=RationalVote;Integrated Security=True" ) )
			{
				sqlConnection.Open();
				
				IEnumerable<User> users = sqlConnection.Query<User>("select * from Users");

				return View( users.ToList() );
			}

			//var users = db.Users.Include(u => u.EmailVerificationTokens).Include(u => u.Profiles);
			//return View(users.ToList());
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
		public ActionResult Register([Bind(Include="Email,Password,PasswordRetype,AcceptedTermsOfService,AcceptedPrivacyPolicy")] User user)
		{
			if (ModelState.IsValid)
			{
				/*//Create user
				byte[] salt;
				byte[] hash;
				Utility.Crypto.CreatePasswordHash( user.Email, user.Password, out salt, out hash );
				user.PasswordSalt = salt;
				user.PasswordHash = hash;
				db.Users.Add(user);

				//Create verification token
				EmailVerificationToken verificationToken = EmailVerificationToken.CreateNew( user );
				db.EmailVerificationTokens.Add( verificationToken );

				//Create profile
				Profile profile = RationalVote.Models.Profile.CreateNew( user );
				db.Profiles.Add( profile );

				new Controllers.MailController().VerificationEmail( user, verificationToken ).Deliver();

				db.SaveChanges();
				return RedirectToAction("Index");*/
			}

			return View( "SignIn", user );
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
