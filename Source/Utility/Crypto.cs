using System.Configuration;
using System.Security.Cryptography;
using System.Linq;

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

		public static void CreatePasswordHash( string email, string password, out byte[] userSpecificSalt, out byte[] passwordHash )
		{
			userSpecificSalt = new byte[ PasswordSaltSize];
			randomGenerator.GetBytes( userSpecificSalt );

			string passwordCountents = passwordSalt + email.ToLower() + password;
			Rfc2898DeriveBytes PBKDF2 = new Rfc2898DeriveBytes( passwordCountents, userSpecificSalt, IterationCount);

			passwordHash = PBKDF2.GetBytes(PasswordHashSize);
		}

		public static bool ConfirmPasswordHash( string email, string password, byte[] userSpecificSalt, byte[] passwordHash )
		{
			string passwordCountents = passwordSalt + email.ToLower() + password;
			Rfc2898DeriveBytes PBKDF2 = new Rfc2898DeriveBytes( passwordCountents, userSpecificSalt, IterationCount );

			byte[] serverPasswordHash = PBKDF2.GetBytes( PasswordHashSize );

			return serverPasswordHash.SequenceEqual( passwordHash );
		}
	}
}