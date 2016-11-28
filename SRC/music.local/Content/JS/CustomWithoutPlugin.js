///không có plugin player
///
///isloop
///0: no loop
///1: loop this track
///2: loop all album
var isLoop = 2;
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
    $.myCrntID = "";
    document.getElementById("player-display").onclick = Seek;
});

///=============Event ============

function playAudio(cls, auto) {
    $.myCrntID = cls;
    if (auto != undefined) {
        BindSelectedTreeElement("." + cls);
    }
    $.myPlayer.src = "";
    //$.myPlayer.waveform.pause();
    var newSrc = $("." + cls).first().data("src");
    var cover = $("." + cls).first().parent().parent().data("src");
    $("#playerSongInfor").text($("." + cls).attr("name"));
    $("#cover").attr("src", cover);
    var playerimage = newSrc.replace("Home/File?p=", "Home/Demo?p=");
    playerimage = encodeURI(playerimage);
    //console.log(playerimage);
    $("#player-display").css("background-image", "url(\"" + playerimage + "\")");
    $.myPlayer.src= newSrc;
    //$.myPlayer.play();
    $.myPlayer.load();
    $.myPlayer.addEventListener("canplay",function () {
        togglePlay(1);
        $.myPlayer.removeEventListener("canplay", function (){});
        //$.myPlayer.play();
    });

}

function togglePlay(isplay) {
    //$.myPlayer.playPause();
    if ($.myPlayer.paused || isplay ==1) {
        //btnPlay
        $.myPlayer.play();
        $("#btnPlay").attr("class", "control-btnCustom control-btnpause");
    } else {
        $.myPlayer.pause();
        $("#btnPlay").attr("class", "control-btnCustom control-btnplay");
    }
}

function endPlay() {
    if (isLoop == 1) {
        $.myPlayer.play();
        return;
    }
    //isloop==2 => load next track
    nextTrack();
    return;
}

function nextTrack() {
    //get next track
    var next = "";
    if ($.myCrntID != undefined && $.myCrntID != "") {
        var current = $.myCrntID.split('-');
        var nextId = current[1];
        next = current[0] + "-" + String(parseInt(nextId) + 1);
        //console.log(next);
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

function changeLoop(ele) {
    switch (isLoop) {
        case 1:
            isLoop = 2;
            break;
        case 2:
            isLoop = 0;
            break;
        case 0:
            isLoop = 1;
            break;
    }
    $(ele).attr("class", getLoopButtonClass());
    $(ele).attr("title", getLoopButtonText());
}
function UpdateDisplay() {
    var width = $("#player-display").width();
    var totalTime = $.myPlayer.duration;
    var crntTime = $.myPlayer.currentTime;
    //console.log(width + "-" + totalTime + "-" + crntTime);
    $("#remain-display").css("width", (width-((crntTime / totalTime) * width).toFixed(0)) + "px ");
}

function Seek(event) {
    console.log($.myPlayer.currentTime);
    //player-display
    var curPos = event.clientX - $(".albumCover").width();
    //console.log(event.clientX);
    var width = $("#player-display").width();
    var totalTime = $.myPlayer.duration;
    var crntTime = ((curPos / width) * totalTime).toFixed(0);
    console.log(totalTime);
    console.log(crntTime);
    $("#remain-display").css("width", (curPos) + "px");
    $.myPlayer.currentTime = parseInt(crntTime);
    //$.myPlayer.addEventListener("canplay", function () {
    //    $.myPlayer.removeEventListener("canplay", function () { });
    //    //$.myPlayer.play();
    //});
    //$.myPlayer.play();
}
///============End Event ================

///============Common Function ================

function BindSelectedTreeElement(selector) {
    $.myFkingTree.each(function (index, value) {
        $(value).jstree("deselect_all");
        if ($(value).find(selector).length > 0) {
            //console.log(value);
            //$(value).jstree("open_node", selector);
            $(value).jstree("select_node", selector);
        }

    });
}
function getLoopButtonClass() {
    return 'control-btnCustom control-loop' + isLoop;
}
function getLoopButtonText() {
    switch (isLoop) {
        case 1:
            return 'Repeat one';
        case 2:
            return 'Repeat Album';
        case 0:
            return 'No repeat';
    }
}
///============End Common Function ================