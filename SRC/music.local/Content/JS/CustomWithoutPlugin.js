﻿/// không có plugin player
/// <reference path="~/Content/JS/CookieHelper.js" />
/// <reference path="~/Content/JS/Common.js" />

$(function () {
    $.myFkingTree = $('.treeWraper').jstree(
    {
        "core": {
            "multiple": false,
            "worker": false
        }
    }
    );
    $.myPlayer = document.getElementById("player");
    $.myPlayer.onended = (function () {
        endPlay();
    });
    $.myPlayer.ontimeupdate= (function() {
        //update time
        UpdateDisplay();
    });
    $.myPlayer.onerror =(function () {
        LogDebug($.myCrntID);
        nextTrack();
    });
    $.myCrntID = "";
    document.getElementById("player-display").onclick = Seek;
    $.myCrntAlbunmCover = $("#cover").attr("src");
    InitLoop();
});

///=============Event ============

function playAudio(cls, auto) {
    CheckSession();
    $.myCrntID = cls;
    if (auto != undefined) {
        BindSelectedTreeElement("." + cls);
    }
    $.myPlayer.src = "";
    //$.myPlayer.waveform.pause();
    var newSrc = $("." + cls).first().data("src");
    console.log(cls);
    var albumCover = $("." + cls).first().parent().parent().data("src");
    if (albumCover != undefined) {
        $.myCrntAlbunmCover = albumCover;
    }
    $("#playerSongInfor").text($("." + cls).attr("name"));

    var songname = $("." + cls + " a:first").text();

    setPageTitle(songname);
    var songCover = newSrc.replace("api/Streamming/Get?p=", "Home/Cover?p=");
    $("#cover").attr("src", songCover);
    CheckCover();

    //var playerimage = newSrc.replace("Home/File?p=", "Home/Demo?p=");
    var playerimage = newSrc.replace("api/Streamming/Get?p=", "Home/Demo?p=");
    playerimage = encodeURI(playerimage);
    LogDebug(playerimage);
    $("#player-display").css("background-image", "url(\"" + playerimage + "\")");
    $.myPlayer.src= newSrc;
    $.myPlayer.load();
    $.myPlayer.addEventListener("canplay",function () {
        togglePlay(1);
        $.myPlayer.removeEventListener("canplay", function (){});
    });

}

function togglePlay(isplay) {
    //$.myPlayer.playPause();
    if ($.myPlayer.paused || isplay ==1) {
        //btnPlay
        setPageTitle();
        $.myPlayer.play();
        $("#btnPlay").attr("class", Class_BtnPlay_Play);
    } else {
        setPageTitle("");
        $.myPlayer.pause();
        $("#btnPlay").attr("class", Class_BtnPlay_Pause);
    }
}

function endPlay() {
    if (isLoop == 1) {
        CheckSession();
        $.myPlayer.play();
        return;
    }
    //isloop==2 => load next track
    nextTrack();
    return;
}

function nextTrack(nextVal) {
    if (nextVal == undefined || nextVal == null) {
        nextVal = 1;
    }
    //get next track
    var next;
    if ($.myCrntID != undefined && $.myCrntID != "") {
        var current = $.myCrntID.split('-');
        var nextId = current[1];
        next = current[0] + "-" + String(parseInt(nextId) + nextVal);
        LogDebug(next);
        if ($("." + next).length) {
            playAudio(next, 1);
        } else {
            //không có bài tiếp
            if (isLoop == 2) {
                next = current[0] + "-" + 0;
                if ($("." + next).length) {
                    playAudio(next, 1);
                }
            }
        }
    }
}


function UpdateDisplay() {
    var width = $("#player-display").width();
    var totalTime = $.myPlayer.duration;
    var crntTime = $.myPlayer.currentTime;
    //audio-time
    var min = Math.floor(totalTime / 60);
    var sec = Math.floor(totalTime % 60);
    var totalDisplayTime = (min < 10 ? ("0" + min) : (min + "")) + ":" + (sec < 10 ? ("0" + sec) : (sec + ""));
    min = Math.floor(crntTime / 60);
    sec = Math.floor(crntTime % 60);
    var currentDisplayTime = (min < 10 ? ("0" + min) : (min + "")) + ":" + (sec < 10 ? ("0" + sec) : (sec + ""));
    $("#audio-time").text("-   " + currentDisplayTime + " / " + totalDisplayTime);
    $("#remain-display").css("width", (width-((crntTime / totalTime) * width).toFixed(0)) + "px ");
}

function Seek(event) {
    LogDebug($.myPlayer.currentTime);
    //player-display
    var curPos = event.clientX - $(".albumCover").width();
    LogDebug(event.clientX);
    var width = $("#player-display").width();
    var totalTime = $.myPlayer.duration;
    var crntTime = ((curPos / width) * totalTime).toFixed(0);
    LogDebug(totalTime);
    LogDebug(crntTime);
    $("#remain-display").css("width", (curPos) + "px");
    $.myPlayer.currentTime = parseInt(crntTime);
}

function CheckCover() {
    setTimeout(function () {
        var currentSrc = $.myCrntAlbunmCover;
        console.log(currentSrc);
        var coverElement = document.getElementById("cover");
        if (!coverElement.complete) {
            $("#cover").attr("src", currentSrc);
        }
        if (coverElement.naturalWidth === 0) {
            $("#cover").attr("src", currentSrc);
        }
    }, 500);
}
///============End Event ================

///============Common Function ================

function BindSelectedTreeElement(selector) {
    $.myFkingTree.each(function (index, value) {
        $(value).jstree("deselect_all");
        if ($(value).find(selector).length > 0) {
            LogDebug(value);
            $(value).jstree("select_node", selector);
        }

    });
}
///============End Common Function ================