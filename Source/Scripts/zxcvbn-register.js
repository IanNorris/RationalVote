$(document).ready(function()
{
	var checker = function()
	{
		var password = $('#RegisterPassword').val();
		var result = zxcvbn(password);
		score100 = (result.score * 25);

		if( password.length > 0 )
		{
			score100 = Math.max( score100, 5 );
		}

		// progress bar based on score
		$('#password_strength_bar').css('width', score100+'%');
		$('#password_strength_bar').removeClass("progress-bar-danger progress-bar-warning progress-bar-success").addClass(result.score < 3 ? "progress-bar-danger" : result.score === 3 ? "progress-bar-warning" : "progress-bar-success");

	};

	$('#RegisterPassword')
		.unbind(".charCounter")
		.bind("keydown", checker )
		.bind("keypress", checker )
		.bind("keyup", checker )
		.bind("focus", checker )
		.bind("mouseover", checker )
		.bind("mouseout", checker )
		.bind("paste", checker )
		.bind("click", checker );
});
