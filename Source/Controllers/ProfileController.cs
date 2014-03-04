using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RationalVote.Models;

namespace RationalVote
{
	public class ProfileController : Controller
	{
		// GET: /Profile/
		public ActionResult Index()
		{
			/*var profiles = db.Profiles.Include(p => p.User);
			return View(profiles.ToList());*/

			return View();
		}

		// GET: /Profile/Details/5
		public ActionResult Details(long? id)
		{
			/*if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Profile profile = db.Profiles.Find(id);
			if (profile == null)
			{
				return HttpNotFound();
			}
			return View(profile);*/
			return View();
		}

		// GET: /Profile/Create
		public ActionResult Create()
		{
			/*ViewBag.Id = new SelectList(db.Users, "Id", "Email");
			return View();*/
			return View();
		}

		// POST: /Profile/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include="Id,DisplayName,RealName,Occupation,Location,Joined,Experience")] Profile profile)
		{
			/*if (ModelState.IsValid)
			{
				db.Profiles.Add(profile);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			ViewBag.Id = new SelectList(db.Users, "Id", "Email", profile.Id);
			return View(profile);*/
			return View();
		}

		// GET: /Profile/Edit/5
		public ActionResult Edit(long? id)
		{
			/*if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Profile profile = db.Profiles.Find(id);
			if (profile == null)
			{
				return HttpNotFound();
			}
			ViewBag.Id = new SelectList(db.Users, "Id", "Email", profile.Id);
			return View(profile);*/
			return View();
		}

		// POST: /Profile/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include="Id,DisplayName,RealName,Occupation,Location,Joined,Experience")] Profile profile)
		{
			/*if (ModelState.IsValid)
			{
				db.Entry(profile).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			ViewBag.Id = new SelectList(db.Users, "Id", "Email", profile.Id);*/
			return View(profile);
		}

		// GET: /Profile/Delete/5
		public ActionResult Delete(long? id)
		{
			/*if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Profile profile = db.Profiles.Find(id);
			if (profile == null)
			{
				return HttpNotFound();
			}
			return View(profile);*/
			return View();
		}

		// POST: /Profile/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(long id)
		{
			/*Profile profile = db.Profiles.Find(id);
			db.Profiles.Remove(profile);
			db.SaveChanges();
			return RedirectToAction("Index");*/
			return View();
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
