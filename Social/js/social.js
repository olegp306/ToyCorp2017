function searchNowSocial() {
    var searchtext = $("#txtSearch").val();
    if (searchtext && searchtext != $("#txtSearch").attr("placeholder")) {
        var url = String.Format("{0}social/searchsocial.aspx?name={1}", $("base").attr("href"), encodeURIComponent(searchtext));
        window.location.href = url;
    }
}

