using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace RationalVote.Models
{
	public class DebateNew
	{
		[Placeholder( "Enter your argument" )]
		[DataType( DataType.MultilineText )]
		[Required( ErrorMessage = "Your argument cannot be empty." )]
		[StringLength( 254 )]
		public string Argument { get; set; }
	}
}