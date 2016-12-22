$(function () {
    $.currnetVID = "";
    $.myVideoPlayer = videojs("video-player");

    $.myVideoPlayer.on("keydown", function (e) {
        var crntTime;
        var crntVol;
        switch (e.keyCode) {
            case 37:
                //left
                crntTime = this.currentTime();
                crntTime = crntTime > 5 ? (crntTime - 5) : 0;
                this.currentTime(crntTime);
                $(this).focus();
                break;
            case 39:
                //right
                crntTime = this.currentTime();
                crntTime = (crntTime < (this.duration() - 5)) ? (crntTime + 5) : this.duration();
                this.currentTime(crntTime);
                $(this).focus();
                break;
            case 38:
                //vol up
                crntVol = this.volume();
                crntVol = (crntVol < 0.95) ? (crntVol + 0.05) : 1;
                this.volume(crntVol);
                $(this).focus();
                break;
            case 40:
                //vol down
                crntVol = this.volume();
                crntVol = (crntVol > 0.05) ? (crntVol - 0.05) : 0;
                this.volume(crntVol);
                $(this).focus();
                break;
            case 32:
                if (this.paused()) {
                    this.play();
                } else {
                    this.pause();
                }
                break;
        }
    });

    $.myVideoPlayer.on("ended", function () {
        endPlay();
    });
});

//id của thẻ div, đã có #
function playVideo(id) {
    $.myVideoPlayer.src("");
    var src = $(id).data("src");
    $.myVideoPlayer.src("/api/Streamming/Get?p=" + encodeURIComponent(src));
    $.currnetVID = id;
    SetActive(id);
    $.myVideoPlayer.play();
    $("#video-player").focus();
}
function endPlay() {
    var crntId = $.currnetVID;
    var arr = crntId.split("-");
    var newId = arr[0] + "-" + arr[1] + "-" + (parseInt(arr[2]) + 1);
    if ($(newId).length) {
        LogDebug("play next");
        playVideo(newId);
    } else {
        LogDebug("play 0");
        newId = arr[0] + "-" + arr[1] + "-0";
        playVideo(newId);
    }
}

function SetActive(id) {

    //child-1
    var parentid = $(id).data("parent");
    $("#" + parentid).children().each(function (index, value) {
        $(value).removeClass("actived");
    });
    //    
    $(id).addClass("actived");
    var pnum = parentid.split("-")[1];
    var cnum = id.split("-")[2];
    var width = $("#folder-" + pnum + " .playlist-wraper").width();
    var liWidth = 160;
    //liWidth = 140;
    //var liWidth = $("#folder-" + num + " .playlist-wraper").width();
    LogDebug(cnum);
    var offsetScroll = (parseInt(cnum) + 1) * liWidth - width;
    LogDebug(offsetScroll);
    $("#folder-" + pnum + " .playlist-wraper").scrollLeft(offsetScroll);

}