using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

public class ExplanationAttribute : Attribute, IMetadataAware
{
	public string Explanation { get; private set; }

	public ExplanationAttribute( string explanation )
	{
		Explanation = explanation;
	}

	public void OnMetadataCreated( ModelMetadata metadata )
	{
		metadata.AdditionalValues[ "explanation" ] = Explanation;
	}
}