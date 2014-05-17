using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Utility
{
	public static class AppSettings
	{
		//Usage: AppSettings.Get<bool>("Debug");

		public static T Get<T>( string key )
		{
			var appSetting = ConfigurationManager.AppSettings[ key ];

			if( appSetting == null )
			{
				return default(T);
			}

			if (typeof(T) == typeof(string))
			{ 
				return (T)(object)ConfigurationManager.AppSettings[ key ];
			}
			else
			{
				if( string.IsNullOrWhiteSpace( appSetting ) ) throw new Exception( "Setting " + key + " was not found in the configuration file" );

				var converter = TypeDescriptor.GetConverter( typeof( T ) );
				return (T)( converter.ConvertFromInvariantString( appSetting ) );
			}
		}
	}
}