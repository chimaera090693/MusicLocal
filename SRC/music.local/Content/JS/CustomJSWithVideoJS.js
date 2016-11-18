///
///isloop
///0: no loop
///1: loop this track
///2: loop all album
var isLoop = 2;
$(function () {
    $('.treeWraper').jstree();
    $.myPlayer = videojs("player");
    $.myCrntID = "";
    $.myPlayer.on("ended", function () {
        endPlay();
    });
    //btnNext
    var buttonNext = $("<button text='Next' id='btnMyNext' class='vjs-control vjs-control-btnCustom vjs-control-btnnext'/>");
    buttonNext.on("click", function () {
        nextTrack();
    });
    buttonNext.insertAfter($(".vjs-play-control"));
    //btnLoop
    var buttonLoop = $("<button text='Loop' title='Repeat Album' id='btnMyLoop' class='vjs-control vjs-control-btnCustom vjs-control-loop2'></button>");
    buttonLoop.on("click", function () {
        changeLoop(this);
    });
    buttonLoop.insertAfter($(".vjs-remaining-time"));

});

///=============Event ============
function playAudio(cls) {
    $.myCrntID = cls;
    $.myPlayer.pause();
    var newSrc = $("." + cls).first().data("src");
    var cover = $("." + cls).first().parent().parent().data("src");
    $("#cover").attr("src", cover);
    $.myPlayer.src(newSrc);
    $.myPlayer.play();
}
function endPlay() {
    if (isLoop == 0) return;
    if (isLoop == 1) {
        $.myPlayer.play();
        return;
    }
    //isloop==2 => load next track
    nextTrack();
    return;
}
function nextTrack() {

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
    return 'vjs-control vjs-control-btnCustom vjs-control-loop' + isLoop;
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
