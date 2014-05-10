using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

public class UserCanEditAttribute : Attribute, IMetadataAware
{
	public bool CanEdit { get; private set; }

	public UserCanEditAttribute( bool canEdit )
	{
		CanEdit = canEdit;
	}

	public void OnMetadataCreated( ModelMetadata metadata )
	{
		metadata.AdditionalValues[ "canEdit" ] = CanEdit;
	}
}