function opinionAjax( target, opinion )
{
	//alert( target + ' -> ' + opinion );
	$.post( "/VoteAjax",
		{
			Link: target,
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

				//$( sender ).parent( ".btn-group" ).find(".SelectedOption" ).text( selection );
				//sender.parent( ".btn-group" ).text( selection );
				
				//$( '#' + senderObj.id ).closest( '.btn-group' ).children( '.SelectedOption' ).text( selection );

				var selectionText = '#' + 'selected' + senderObj.id;

				$( selectionText ).text(  $( e.target ).data( 'description' ) ).prop( 'title', $( e.target ).data( 'title' ) );
				
				opinionAjax( senderObj.id.split("_")[1], selection );

//				sende.html( 'farce' );
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

$('.OpinionOption').click(
	function(e)
	{
		var picked = $(this).data( 'val' );
		var target = $(this).data( 'target' );
		var title = $(this).data( 'title' );
		var message = $(this).text();

		$( '#' + 'selectedFallacyTrigger_' + target ).text( message ).prop( 'title', title );

		opinionAjax( target, picked );
	}
);
