using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

public class StringLengthConfigAttribute : StringLengthAttribute
{
	public StringLengthConfigAttribute( string configParameter )
	: base( Utility.AppSettings.Get<int>( configParameter ) )
	{
	}
}