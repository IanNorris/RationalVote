using System.Security.Cryptography;

namespace Utility
{
	class Crypto
	{
		private static RNGCryptoServiceProvider randomGenerator = new RNGCryptoServiceProvider();

		public static string GenerateSalt()
		{
			byte[] bytes = new byte[64];
			randomGenerator.GetBytes( bytes );

			string hex = System.BitConverter.ToString( bytes );
			return hex.Replace( "-", "" );
		}
	}
}