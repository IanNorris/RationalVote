using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RationalVote.Models
{
	public class ErrorMessage
	{
		public enum TypeEnum
		{
			Danger,
			Info,
			Warning,
			Success
		}

		public ErrorMessage( TypeEnum type, string title, string message )
		{
			Type = type;
			Title = title;
			Message = message;
		}

		public TypeEnum Type { get; set; }
		public string Title { get; set; }
		public string Message { get; set; }
	}
}