using System.Configuration;
using System.Security.Cryptography;
using System.Linq;
using System;
using System.Text;

namespace Utility
{
	public class Crypto
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

		public static string CalculateMD5Hash( string input )
		{
			// step 1, calculate MD5 hash from input
			MD5 md5 = System.Security.Cryptography.MD5.Create();
			byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes( input );
			byte[] hash = md5.ComputeHash( inputBytes );

			// step 2, convert byte array to hex string
			StringBuilder sb = new StringBuilder();
			for( int i = 0; i < hash.Length; i++ )
			{
				sb.Append( hash[ i ].ToString( "X2" ) );
			}
			return sb.ToString();
		}

		public static string CalculateSHA256Hash( string input )
		{
			SHA256Managed sha = new SHA256Managed();
			string hash = String.Empty;
			byte[] crypto = sha.ComputeHash( Encoding.UTF8.GetBytes( input ), 0, Encoding.UTF8.GetByteCount( input ) );

			foreach( byte bit in crypto )
			{
				hash += bit.ToString( "x2" );
			}

			return hash;
		}

		private static byte[] StringEncode( string text )
		{
			var encoding = new ASCIIEncoding();
			return encoding.GetBytes( text );
		}

		public static byte[] CalculateHMAC256( string key, string message )
		{
			byte[] keyBytes = StringEncode( key );
			byte[] messageBytes = StringEncode( message );

			var hash = new HMACSHA256( keyBytes );
			return hash.ComputeHash( messageBytes );
		}
	}
}