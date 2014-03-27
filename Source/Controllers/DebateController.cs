﻿using System;
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

namespace RationalVote.Controllers
{
	[RoutePrefix("Debate")]
	public class DebateController : Controller
	{
		//
		// GET: /Debate/{Id?}
		[Route( "{Id?}" )]
		public ActionResult Index( long? Id )
		{
			if( Id == null )
			{
				Id = 8;
			}

			using( SqlConnection connection = RationalVoteContext.Connect() )
			{
				Debate test = connection.Get<Debate>( Id );

				return View( test );
			}
		}
	}
}