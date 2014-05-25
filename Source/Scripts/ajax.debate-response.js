function OnAjaxResponseSuccess( data, target )
{
	$( target ).append( data );

	//fallacy.js
	onAddedContent();
}

function OnAjaxLoadChildren( data, target )
{
	$( target ).append( data );
	
	//fallacy.js
	onAddedContent();
}

function loadMoreChildren( sender, parent, targetContainer, type )
{
	$( sender ).hide();

	$( targetContainer ).append( '<div class="Loader' + type + ' spinner" title="Loading..."><div class="dot1"></div><div class="dot2"></div></div>' );

	var offset = $.trim( $( '.LatestOfset' + type ).last().text() );

	$( '.LatestOfset' + type ).each(
		function()
		{
			$(this).remove();
		}
	);

	$.ajax( '/GetChildrenAjax/' + parent + '/' + type + '/' + offset ).done(
		function( data )
		{
			OnAjaxLoadChildren( data, targetContainer );

			var found = false;

			$( '.LatestOfset' + type ).each(
				function()
				{
					found = true;
				}
			);

			if( found )
			{
				$( sender ).show();
			}

			$( '.Loader' + type ).each(
				function()
				{
					$(this).remove();
				}
			);
		}
	);
}