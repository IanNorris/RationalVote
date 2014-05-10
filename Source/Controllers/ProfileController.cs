﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RationalVote.Models;
using Dapper;
using RationalVote.DAL;
using System.Text.RegularExpressions;

namespace RationalVote
{
	public class ProfileController : Controller
	{
		// GET: /Profile
		[Route("Profile/{Id?}")]
		public ActionResult Index( long? Id )
		{
			if( Id == null )
			{
				if( HttpContext.User.Identity.IsAuthenticated )
				{
					Id = ((RationalVote.Models.UserPrincipal)HttpContext.User).User.Id;
				}
			}

			if( Id != null )
			{
				using( DbConnection connection = RationalVoteContext.Connect() )
				{
					Profile profile = connection.Get<Profile>( Id );

					if( profile != null )
					{
						return View( "Index", profile );
					}
				}
			}

			throw new HttpException( 404, "This profile does not exist." );
		}

		// POST: /Profile
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[Route("Profile/{Id?}")]
		[ValidateAntiForgeryToken]
		public ActionResult Index( [Bind(Include="DisplayName,RealName,Occupation,Location")] Profile profile )
		{
			long Id = ((RationalVote.Models.UserPrincipal)HttpContext.User).User.Id;

			using( DbConnection connection = RationalVoteContext.Connect() )
			{
				Profile profileOriginal = connection.Get<Profile>( Id );

				if( profileOriginal != null )
				{
					if( profile.DisplayName != null )
					{
						profileOriginal.DisplayName = profile.DisplayName;
					}
					
					if( profile.Location != null )
					{
						profileOriginal.Location = profile.Location;
					}

					if( profile.Occupation != null )
					{
						profileOriginal.Occupation = profile.Occupation;
					}

					if( profile.RealName != null )
					{
						profileOriginal.RealName = profile.RealName;
					}

					if( ModelState.IsValid )
					{
						connection.Update( profileOriginal );
					}

					return View( "Index", profileOriginal );
				}
			}

			throw new HttpException( 404, "This profile does not exist." );
		}
	}
}
