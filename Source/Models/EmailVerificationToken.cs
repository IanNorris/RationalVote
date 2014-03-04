namespace RationalVote.Models
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using Utility;
	
	public class EmailVerificationToken
	{
		[Key]
		public long Id { get; set; }

		[DataType(DataType.DateTime)]
		public System.DateTime Created { get; set; }

		[DataType( "VARCHAR" )]
		[MaxLength(64)]
		public string Token {  get; set; }
	
		[Required]
		[ForeignKey("Id")]
		public virtual User User { get; set; }

		public static EmailVerificationToken CreateNew( User user )
		{
			EmailVerificationToken token = new EmailVerificationToken();
			token.User = user;
			token.Token = Utility.Crypto.GenerateSaltString( 32 );
			token.Created = DateTime.Now;

			return token;
		}
	}
}
