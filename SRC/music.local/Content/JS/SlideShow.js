//content-wraper

$.myTimeoutImageSlider = 0;
$.myCurrentImageId = -1;
$(function () {
    $.myCurrentImageId = $.myCurrentImageId + 1;
    var newurl = "/Home/ImageCover?id=" + $.myCurrentImageId + "";
    $("#content-wraper").css("background-image", "url(\"" + newurl + "\")");

    $.myTimeoutImageSlider = setInterval(function () {
        //background-image: url("img_tree.png
        $.myCurrentImageId = $.myCurrentImageId + 1;
        var newurl = "/Home/ImageCover?id=" + $.myCurrentImageId + "";
        $("#content-wraper").css("background-image", "url(\"" + newurl + "\")");
    }, 15000)
});