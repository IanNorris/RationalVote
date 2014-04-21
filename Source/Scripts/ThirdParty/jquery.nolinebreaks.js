$('.NoLineBreaks').keypress(function(event) {
    // Check the keyCode and if the user pressed Enter (code = 13) 
    // disable it
    if (event.keyCode == 13) {
        event.preventDefault();
    }
});