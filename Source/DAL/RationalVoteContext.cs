using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using DapperExtensions.Mapper;

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
			string context = WebConfigurationManager.ConnectionStrings["RationalVoteContext"].ToString();

			//Test the connection works
			using( SqlConnection connection = new SqlConnection( context ) )
			{
				connection.Open();
			}

			DapperExtensions.DapperExtensions.DefaultMapper = typeof(PluralizedAutoClassMapper<>);
		}

		public static SqlConnection Connect()
		{
			string context = WebConfigurationManager.ConnectionStrings["RationalVoteContext"].ToString();

			SqlConnection connection = new SqlConnection( context );
			connection.Open();

			return connection;
		}

		public static Error DecodeException( SqlException exception )
		{
			switch( exception.Number )
			{
				case 2601:
					return Error.DuplicateIndex;

				default:
					return Error.Unknown;
			}
		}
	}
}