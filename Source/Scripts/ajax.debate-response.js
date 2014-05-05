function OnAjaxResponseSuccess( data, target )
{
	$( target ).append( data );

	//fallacy.js
	onAddedContent();
}