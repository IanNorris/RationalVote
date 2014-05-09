function opinionAjax( pobj, cobj, type, opinion )
{
	$.post( "/VoteAjax",
		{
			Parent: pobj,
			Child: cobj,
			Type: type,
			Vote: opinion,
			__RequestVerificationToken: $('#__AjaxAntiForgeryForm input[name=__RequestVerificationToken]').val()
		} );
}

$('#fallacyDialog').on(
	'shown.bs.modal', 
	function (e)
	{
		$(this).find( '.FallacyButton' ).on( "click", { sender: e.relatedTarget },
			function(e)
			{
				var selection = $( e.target ).data( 'val' );
				var senderObj = e.data.sender;

				var selectionText = '#' + 'selected' + senderObj.id;

				$( selectionText ).text(  $( e.target ).data( 'description' ) ).prop( 'title', $( e.target ).data( 'title' ) );
				
				var splitResult = senderObj.id.split("_");

				opinionAjax( splitResult[1], splitResult[2], splitResult[3], selection );
			}
		);
	}
);

$('#fallacyDialog').on(
	'hidden.bs.modal', 
	function (e)
	{
		$(this).find( '.FallacyButton' ).unbind();
	}
);

function onAddedContent()
{
	$('.OpinionOption').unbind();
	$('.OpinionOption').click(
		function(e)
		{
			var picked = $(this).data( 'val' );
			var parentObj = $(this).data( 'pobj' );
			var childObj = $(this).data( 'cobj' );
			var type = $(this).data( 'type' );
			var title = $(this).data( 'title' );
			var message = $(this).text();

			$( '#' + 'selectedFallacyTrigger_' + parentObj + '_' + childObj + '_' + type ).text( message ).prop( 'title', title );

			opinionAjax( parentObj, childObj, type, picked );
		}
	);
}

onAddedContent();