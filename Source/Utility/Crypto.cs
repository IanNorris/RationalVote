using System.Configuration;
using System.Security.Cryptography;

namespace Utility
{
	class Crypto
	{
		const int IterationCount = 5000;
		const int PasswordSaltSize = 64;
		const int PasswordHashSize = 64;

		private static RNGCryptoServiceProvider randomGenerator = new RNGCryptoServiceProvider();
		private static string passwordSalt = ConfigurationManager.AppSettings.Get("passwordSalt");

		public static string GenerateSalt()
		{
			byte[] bytes = new byte[PasswordSaltSize];
			randomGenerator.GetBytes( bytes );

			string hex = System.BitConverter.ToString( bytes );
			return hex.Replace( "-", "" );
		}

		public static void CreatePasswordHash( string email, string password, out string userSpecificSalt, out string passwordHash )
		{
			byte[] salt = new byte[ PasswordSaltSize ];
			randomGenerator.GetBytes( salt );

			//Store the hash we generated
			userSpecificSalt = System.BitConverter.ToString( salt ).Replace( "-", "" );

			string passwordCountents = passwordSalt + email.ToLower() + password;
			Rfc2898DeriveBytes PBKDF2 = new Rfc2898DeriveBytes( passwordCountents, salt, IterationCount);

			string hex = System.BitConverter.ToString( PBKDF2.GetBytes(PasswordHashSize) );
			passwordHash = hex.Replace( "-", "" );
		}
	}
}