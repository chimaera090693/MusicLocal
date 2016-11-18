
///
///isloop
///0: no loop
///1: loop this track
///2: loop all album
var isLoop = 2;
$(function () {
    $.myFkingTree = $('.treeWraper').jstree();
    $.myPlayer = WaveSurfer.create({
        container: '.player-wraper',
        height: 150
    });
    $.myPlayer.on("finish", function () {
        endPlay();
    });
    $.myCrntID = "";

});

///=============Event ============
function playAudio(cls, auto) {
    $.myCrntID = cls;
    //$.myPlayer.waveform.pause();
    var newSrc = $("." + cls).first().data("src");
    var cover = $("." + cls).first().parent().parent().data("src");
    $("#cover").attr("src", cover);
    $.myPlayer.load(newSrc);
    //$.myPlayer.play();
    $.myPlayer.once('ready', function () {
        $.myPlayer.play();
    });
    if (auto != undefined) {
        $.myFkingTree.jstree("deselect_all");
        $.myFkingTree.jstree("select_node", "." + cls);
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
        console.log(next);
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
///============End Event ================

///============Common Function ================

//vjs-control vjs-control-btnCustom
function getLoopButtonClass() {
    return 'control-btnCustom control-loop' + isLoop;
}
function getLoopButtonText() {
    switch (isLoop) {
        case 1:
            return 'Repeat one';
            break;
        case 2:
            return 'Repeat Album';
            break;
        case 0:
            return 'No repeat';
            break;
    }
}
///============End Common Function ================