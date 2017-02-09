/// <reference path="~/Content/JS/CookieHelper.js" />
/// <reference path="~/Content/JS/Common.js" />
//load các pdfjs vào iframe

function Read(url) {
    var newURL = encodeURIComponent("http://" + $("#HostAddress").val() + "/Home/File?p=" + encodeURIComponent(url) + "");
    $("#reader").attr("src", "http://" + $("#HostAddress").val() + "/Content/pdf.js/web/viewer.html?file=" + newURL + "");
    setPageTitle(url.split('/')[3]);
}