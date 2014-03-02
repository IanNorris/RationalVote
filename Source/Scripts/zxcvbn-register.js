$(document).ready(function(){
    $('#Password').keyup(function(event){
        var password = $('#Password').val();
        var result = zxcvbn(password);
        score100 = (result.score * 25);

        // progress bar based on score
        $('#password_strength_bar').css('width', score100+'%');
        $('#password_strength_bar').removeClass("progress-bar-danger progress-bar-warning progress-bar-success").addClass(result.score < 3 ? "progress-bar-danger" : result.score === 3 ? "progress-bar-warning" : "progress-bar-success");

    });
});
