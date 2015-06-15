/// <reference path="../jquery-1.10.2.js" />

$(function () {

    //$("#StartDateRecruits").datepicker();
    //$("#EndDateRecruits").datepicker();
    //$("#StartWorkDate").datepicker();
    //$("#EndWorkDate").datepicker();


    $("#RegionId").bind("change", function () {
        GetStreetByRegion();
    });
});

function GetStreetByRegion() {
    $("#StreetId").find("option").remove();
    var regionId = $("#RegionId").find("option:selected").val();
    $.ajax({
        type: "POST",
        data: "{'regionId': '" + regionId + "'}",
        dataType: "json",
        url: "/Shop/GetStreetsByRegionId",
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            $("#StreetId").append("<option value='0'>可空</option>");
            for (i = 0; i < data.length; i++) {
                $("#StreetId").append("<option value='" + data[i].RegionId + "'>" + data[i].RegionName + "</option>");
            }
        }
    });
}