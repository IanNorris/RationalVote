using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Common;
using System.Web.Configuration;
using MySql.Data.MySqlClient;

namespace RationalVote.DAL
{
	public static class RationalVoteContext
	{
		public enum Error
		{
			DuplicateIndex,

			Unknown,
		}

		static RationalVoteContext()
		{
			//DapperExtensions.DapperExtensions.SqlDialect = new DapperExtensions.Sql.MySqlDialect();

			string context = WebConfigurationManager.ConnectionStrings["RationalVoteContext"].ToString();

			//Test the connection works
			using( DbConnection connection = new MySqlConnection( context ) )
			{
				connection.Open();
			}
		}

		public static DbConnection Connect()
		{
			string context = WebConfigurationManager.ConnectionStrings["RationalVoteContext"].ToString();

			DbConnection connection = new MySqlConnection( context );
			connection.Open();

			return connection;
		}

		public static Error DecodeException( MySqlException exception )
		{
			switch( exception.Number )
			{
				case 1062:
					return Error.DuplicateIndex;

				default:
					return Error.Unknown;
			}
		}
	}
}