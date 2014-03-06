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
			DapperExtensions.DapperExtensions.DefaultMapper = typeof(PluralizedAutoClassMapper<>);
		}

		public static SqlConnection Connect()
		{
			SqlConnection connection = new SqlConnection( WebConfigurationManager.ConnectionStrings["RationalVoteContext"].ToString() );
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