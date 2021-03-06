﻿/// <reference path="~/Content/JS/CookieHelper.js" />
/// Common variable
var defaultTitle = "Far, anyway!";
var enableLogErr = true;
var enableLogDebug = true;
///isloop
///0: no loop
///1: loop this track
///2: loop all album
var isLoop = 2;

//cookie key
var Cookie_LoopStatus = "Cookie_LoopStatus";

//text and class
var Class_BtnPlay_Play = "control-btnCustom control-btnpause";
var Class_BtnPlay_Pause = "control-btnCustom control-btnplay";

var Class_BtnLoop = "control-btnCustom control-loop";
var Txt_BtnLoop_ROne = "Repeat one";
var Txt_BtnLoop_RAlbum = "Repeat Album";
var Txt_BtnLoop_NoR = "No repeat";
var Txt_BtnLoop_Suf = "Random";


//audio player function
function getLoopButtonClass() {
    return Class_BtnLoop + isLoop;
}
function getLoopButtonText() {
    switch (isLoop) {
        case 1:
            return Txt_BtnLoop_ROne;
        case 2:
            return Txt_BtnLoop_RAlbum;
        case 3:
            return Txt_BtnLoop_Suf;
        case 0:
            return Txt_BtnLoop_NoR;
    }
    return "";
}

function InitLoop() {
    var loopStatus = readCookie(Cookie_LoopStatus);
    if (loopStatus != null && loopStatus != "") {
        isLoop = loopStatus;
    } else {
        isLoop = 2;
        writeCookie(Cookie_LoopStatus, isLoop, 7);
    }
    $("#btnMyLoop").attr("class", getLoopButtonClass());
    $("#btnMyLoop").attr("title", getLoopButtonText());
}

function changeLoop(ele) {
    LogDebug(ele);
    switch (parseInt(isLoop)) {
        case 1:
            isLoop = 2;
            break;
        case 2:
            isLoop = 3;
            break;
        case 3:
            isLoop = 0;
            break;
        case 0:
            isLoop = 1;
            break;
    }
    writeCookie(Cookie_LoopStatus, isLoop, 7);
    $(ele).attr("class", getLoopButtonClass());
    $(ele).attr("title", getLoopButtonText());
}

// genneral page funtion
function setPageTitle(songname) {
    if ($.myInverter) {
        clearInterval($.myInverter);
    }
    $(document).prop('title', defaultTitle);
    if (songname === "") {
        $(document).prop('title', defaultTitle);
    } else {
        $.currentSong = songname == undefined ? $.currentSong : songname;
        $.myInverter = setInterval(function () {
            var currenttt = $(document).prop('title');
            if (currenttt === defaultTitle) {
                $(document).prop('title', $.currentSong);
            } else {
                $(document).prop('title', defaultTitle);
            }
        }, 3000);
    }
}

function CheckSession() {
    $.ajax({
        type: "GET",
        url: "/home/CheckSession",
        contentType: "text/html; charset=utf-8",
        success: function (data) {
            LogDebug(data);;
            if (data != "1") {
               location.reload();
           }
        }
    });
}
function LogError(msg) {
    if (enableLogErr) {
        console.log(msg);
    }
}
function LogDebug(msg) {
    if (enableLogDebug) {
        console.log(msg);
    }
}
