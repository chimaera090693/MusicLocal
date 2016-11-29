function writeCookie(name, value, days) {
    try {
        var date, expires;
        if (!days) {
            days = 7;
        }
        date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toGMTString();
        document.cookie = name + "=" + value + expires + "; path=/";
    } catch (e) {
        console.log("Error write cookie: ");
        console.log("       name: " + name);
        console.log("       value: " + value);
        console.log("       days: " + days);
    }
}

function readCookie(name) {
    try {
        var i, c, ca, nameEQ = name + "=";
        ca = document.cookie.split(";");
        for (i = 0; i < ca.length; i++) {
            c = ca[i];
            while (c.charAt(0) == " ") {
                c = c.substring(1, c.length);
            }
            if (c.indexOf(nameEQ) == 0) {
                return c.substring(nameEQ.length, c.length);
            }
        }
        return "";
    } catch (e) {
        console.log("Error Read Cookie: "+name);
        return "";
    }
}

function eraseCookie(name) {
    writeCookie(name, "", -1);
}