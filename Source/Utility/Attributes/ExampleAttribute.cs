using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

public class ExampleAttribute : Attribute, IMetadataAware
{
	public string Example { get; private set; }

	public ExampleAttribute( string example )
	{
		Example = example;
	}

	public void OnMetadataCreated( ModelMetadata metadata )
	{
		metadata.AdditionalValues[ "example" ] = Example;
	}
}