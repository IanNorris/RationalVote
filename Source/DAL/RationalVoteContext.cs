using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace RationalVote.DAL
{
	public static class RationalVoteContext
	{
		public static SqlConnection Connect()
		{
			return new SqlConnection( WebConfigurationManager.ConnectionStrings["RationalVoteContext"].ToString() );
		}
	}
}