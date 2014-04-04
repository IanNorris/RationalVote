using System.Configuration;
using System.Security.Cryptography;
using System.Linq;
using System;

namespace Utility
{
	class Crypto
	{
		const int IterationCount = 5000;
		public const int PasswordSaltSize = 64; //Numbers are actually half the number of bytes because this length is the string length (2 chars per byte)
		public const int PasswordHashSize = 64;

		private static RNGCryptoServiceProvider randomGenerator = new RNGCryptoServiceProvider();
		private static string passwordSalt = ConfigurationManager.AppSettings.Get("passwordSalt");

		public static byte[] GenerateSalt()
		{
			byte[] bytes = new byte[PasswordSaltSize];
			randomGenerator.GetBytes( bytes );

			return bytes;
		}

		public static string GenerateSaltString( uint length )
		{
			byte[] bytes = new byte[ length ];
			randomGenerator.GetBytes( bytes );

			string hex = System.BitConverter.ToString( bytes );
			return hex.Replace( "-", "" );
		}

		public static void CreatePasswordHash( string email, string password, out string userSpecificSalt, out string passwordHash )
		{
			byte[] userSpecificSaltBinary = new byte[ PasswordSaltSize];
			randomGenerator.GetBytes( userSpecificSaltBinary );

			string passwordCountents = passwordSalt + email.ToLower() + password;
			Rfc2898DeriveBytes PBKDF2 = new Rfc2898DeriveBytes( passwordCountents, userSpecificSaltBinary, IterationCount);

			byte[] passwordHashBinary = PBKDF2.GetBytes(PasswordHashSize);

			userSpecificSalt = ByteArrayToString( userSpecificSaltBinary );
			passwordHash = ByteArrayToString( passwordHashBinary );
		}

		public static bool ConfirmPasswordHash( string email, string password, string userSpecificSalt, string passwordHash )
		{
			byte[] userSpecificSaltBinary = StringToByteArray( userSpecificSalt );

			string passwordCountents = passwordSalt + email.ToLower() + password;
			Rfc2898DeriveBytes PBKDF2 = new Rfc2898DeriveBytes( passwordCountents, userSpecificSaltBinary, IterationCount );

			byte[] serverPasswordHashBinary = PBKDF2.GetBytes( PasswordHashSize );
			string serverPasswordHash = ByteArrayToString( serverPasswordHashBinary );

			return serverPasswordHash.CompareTo( passwordHash ) == 0;
		}

		static string ByteArrayToString( byte[] bytes )
		{
			char[] c = new char[ bytes.Length * 2 ];
			int b;
			for( int i = 0; i < bytes.Length; i++ )
			{
				b = bytes[ i ] >> 4;
				c[ i * 2 ] = (char)( 55 + b + ( ( ( b - 10 ) >> 31 ) & -7 ) );
				b = bytes[ i ] & 0xF;
				c[ i * 2 + 1 ] = (char)( 55 + b + ( ( ( b - 10 ) >> 31 ) & -7 ) );
			}
			return new string( c );
		}

		public static byte[] StringToByteArray( string hex )
		{
			int NumberChars = hex.Length;
			byte[] bytes = new byte[ NumberChars / 2 ];
			for( int i = 0; i < NumberChars; i += 2 )
				bytes[ i / 2 ] = Convert.ToByte( hex.Substring( i, 2 ), 16 );
			return bytes;
		}
	}
}