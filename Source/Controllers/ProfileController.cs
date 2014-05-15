using System;
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
					Id = ((RationalVote.Models.UserPrincipal)HttpContext.User).User.User.Id;
				}
			}

			if( Id != null )
			{
				using( DbConnection connection = RationalVoteContext.Connect() )
				{
					UserProfile profileObj = connection.Query<UserProfile,User,Profile,UserProfile>( @"SELECT User.Id As Id, User.*, Profile.* FROM Profile
																		INNER JOIN User
																		ON User.Id = Profile.UserId
																		WHERE Profile.UserId = @Id", 
																		( userProfile, user, profile ) =>
																		{
																			userProfile.User = user;
																			userProfile.Profile = profile;
																			return userProfile;
																		},
																		new { Id = Id },
																		splitOn: "Id,UserId" ).First();

					if( profileObj != null )
					{
						return View( "Index", profileObj );
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
			long Id = ((RationalVote.Models.UserPrincipal)HttpContext.User).User.User.Id;

			using( DbConnection connection = RationalVoteContext.Connect() )
			{
				UserProfile profileOriginal = connection.Query<UserProfile, User, Profile, UserProfile>( @"SELECT User.Id As Id, User.*, Profile.* FROM Profile
																		INNER JOIN User
																		ON User.Id = Profile.UserId
																		WHERE Profile.UserId = @Id",
																		( userProfile, user, nestedProfile ) =>
																		{
																			userProfile.User = user;
																			userProfile.Profile = nestedProfile;
																			return userProfile;
																		},
																		new { Id = Id },
																		splitOn: "Id,UserId" ).First();
				
				if( profileOriginal != null )
				{
					profileOriginal.Profile.DisplayName = profile.DisplayName;
					profileOriginal.Profile.Location = profile.Location;
					profileOriginal.Profile.Occupation = profile.Occupation;
					profileOriginal.Profile.RealName = profile.RealName;

					if( ModelState.IsValid )
					{
						connection.Update( profileOriginal.Profile );

						return RedirectToAction( "Index" );
					}

					return View( "Index", profileOriginal );
				}
			}

			throw new HttpException( 404, "This profile does not exist." );
		}
	}
}
