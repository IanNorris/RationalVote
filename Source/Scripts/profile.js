(function( $ ){
   $.fn.profilePopover = function() {
		this.popover(
		{
			html : true,
			title:	this.data('title') + ' <button type="button" id="close" class="close" onclick="$(&quot;#' + this.attr('id') + '&quot;).popover(&quot;hide&quot;);">&times;</button>',
			content: function()
			{
				return $( '#' + this.id + '-popup' ).html();
			}
		}
	);
	return this;
   }; 
})( jQuery );

$('.hasProfilePopup').each(
	function()
	{
		$(this).profilePopover();
	}
);