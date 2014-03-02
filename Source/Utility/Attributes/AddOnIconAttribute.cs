using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

public class AddOnIconAttribute : Attribute, IMetadataAware
{
	private readonly string _icon;
	public AddOnIconAttribute( string icon )
	{
		_icon = icon;
	}

	public void OnMetadataCreated( ModelMetadata metadata )
	{
		metadata.AdditionalValues[ "icon" ] = _icon;
	}
}