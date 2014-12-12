var notificationCount = 0;

$(function () {

		var notifications = $.connection.notificationHub;
		var notificationCountUpdated = true;

		function init()
		{
			//notifications.server.onConnect( 'Hello World' );
		}

		notifications.client.onCountUpdated = function( newCount )
		{
			$('#NotificationCount').text( newCount );
			notification_sound.play();
			notificationCountUpdated = true;

			$('#NotificationBell').addClass( 'messages-new' );
			$('#NotificationBell').removeClass( 'messages-seen' );
		};

		var notificationProxy = $.connection.notificationHub;

		$.connection.hub.url = "/signalr";
		$.connection.hub.start().done(init);

		$('#NotificationMenu').hover(
			function()
			{
				if( notificationCountUpdated )
				{
					$('#NotificationPanelBody').html( '<div class="center-outer"><div class="center-middle"><div class="center-inner" style="text-align:center"><i class="fa fa-refresh fa-spin fa-3x"></i><br/><br/>Checking for new messages...</div></div></div>' );

					$.ajax( '/Notification/Index' )
						.done( function( data )
								{
									$('#NotificationPanelBody').html( data );
									$('#NotificationCount').text( '' );
									$('#NotificationBell').removeClass( 'messages-new' );
									$('#NotificationBell').addClass( 'messages-seen' );
								} )
						.fail( function()
								{
									$('#NotificationPanelBody').html( '<div class="center-outer"><div class="center-middle"><div class="center-inner" style="text-align:center"><i class="fa fa-ban fa-3x"></i><br/><br/>Unable to check for messages.</div></div></div>' );
									notificationCountUpdated = true;
								} );

					notificationCountUpdated = false;
				}
			} );
	});