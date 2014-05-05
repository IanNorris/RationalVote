using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

public class AddOnIconAttribute : Attribute, IMetadataAware
{
	public string Icon { get; private set; }

	public AddOnIconAttribute( string icon )
	{
		Icon = icon;
	}

	public void OnMetadataCreated( ModelMetadata metadata )
	{
		metadata.AdditionalValues[ "icon" ] = Icon;
	}
}