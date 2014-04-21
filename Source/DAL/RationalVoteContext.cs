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
		}

		public static DbConnection Connect()
		{
			var context = WebConfigurationManager.ConnectionStrings[ "RationalVoteContext" ];
			var factory = DbProviderFactories.GetFactory( "MySql.Data.MySqlClient" );
			DbConnection connection = factory.CreateConnection();

			connection.ConnectionString = context.ConnectionString;

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