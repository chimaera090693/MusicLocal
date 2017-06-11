//content-wraper

$.myTimeoutImageSlider = 0;
$.myCurrentImageId = 0;
$(function () {
    var totalImage = parseInt($("#countImage").val());
    //background-image: url("img_tree.png
    $.myCurrentImageId = Math.floor(Math.random() * totalImage);
    var newurl = "/Home/ImageCover?id=" + $.myCurrentImageId + "";
    $("#content-wraper").css("background-image", "url(\"" + newurl + "\")");

    $.myTimeoutImageSlider = setInterval(function () {
        var totalImage = parseInt($("#countImage").val());
        //background-image: url("img_tree.png
        var randomId = Math.floor(Math.random() * totalImage);
        $.myCurrentImageId = randomId;
        var newurl = "/Home/ImageCover?id=" + $.myCurrentImageId + "";
        $("#content-wraper").css("background-image", "url(\"" + newurl + "\")");
    }, 15000)
});