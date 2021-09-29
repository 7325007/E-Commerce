function IsValidEmailAddress(inputEmail) {
    var validRegex = /^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/;
    if (validRegex.test(inputEmail)) {
        return true;
    }
    return false;
}

function isNumberKey(evt) {
    var theEvent = evt || window.event;
    var key = theEvent.keyCode || theEvent.which;
    key = String.fromCharCode(key);
    var regex = /^[0-9\b]+$/;    // allow only numbers [0-9]
    if (!regex.test(key)) {
        return false;
    }
    else {
        return true;
    }
}