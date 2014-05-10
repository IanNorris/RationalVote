using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

public class AddOnButtonAttribute : Attribute, IMetadataAware
{
	public string Icon { get; private set; }
	public string Message { get; private set; }

	public AddOnButtonAttribute( string icon, string message )
	{
		Icon = icon;
		Message = message;
	}

	public void OnMetadataCreated( ModelMetadata metadata )
	{
		metadata.AdditionalValues[ "buttonIcon" ] = Icon;
		metadata.AdditionalValues[ "buttonMessage" ] = Message;
	}
}