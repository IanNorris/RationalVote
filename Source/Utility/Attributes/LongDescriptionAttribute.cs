using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

public class LongDescriptionAttribute : Attribute, IMetadataAware
{
	public string LongDescription { get; private set; }

	public LongDescriptionAttribute( string description )
	{
		LongDescription = description;
	}

	public void OnMetadataCreated( ModelMetadata metadata )
	{
		metadata.AdditionalValues[ "longDescription" ] = LongDescription;
	}
}