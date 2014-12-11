var notificationCount = 0;

$(function () {

		var notifications = $.connection.notificationHub;

		function init()
		{
			//notifications.server.onConnect( 'Hello World' );
		}

		notifications.client.onCountUpdated = function( newCount )
		{
			$('#NotificationCount').text( newCount );
			notification_sound.play();
		};

		var notificationProxy = $.connection.notificationHub;

		/*connection.start().done(function() { 
			$("#broadcast").click(function () {
				connection.send(notificationCount++);
			});
		});*/

		$.connection.hub.url = "/signalr";
		$.connection.hub.start().done(init);
	});