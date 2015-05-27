function searchByStationName() {
    var map = new BMap.Map("container");
    var localSearch = new BMap.LocalSearch(map);
    var keyword = document.getElementById("text_").value;
    localSearch.setSearchCompleteCallback(function (searchResult) {
        var poi = searchResult.getPoi(0);
        if (poi != null && poi.point!=null) {
            document.getElementById("result_").value = poi.point.lng + "," + poi.point.lat;
        }
        else {
            document.getElementById("result_").value = "定位失败，请输入正确地址";
        }
    });
    localSearch.search(keyword);
}