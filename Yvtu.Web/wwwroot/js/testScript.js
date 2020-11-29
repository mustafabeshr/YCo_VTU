$(document).ready(function () {

//    function () {
//    $("#btnModalSave").on("click", function (model) {

//        var rule = {};
//        rule.role = $("#selRole").val();
//        rule.minValue = $("#txtMinValue").val();
//        rule.maxValue = $("#txtMaxValue").val()
//        rule.taxPer = $("#txtTaxPer").val();

//        var tr = $("<tr></tr>");
//        tr.append("<td>" + rule.role + "</td>");
//        tr.append("<td>" + rule.minValue + "</td>");
//        tr.append("<td>" + rule.maxValue + "</td>");
//        tr.append("<td>" + rule.taxPer + "</td>");

//        var tbody = $("#tbl tbody");
//        tbody.append(tr);

//        console.log(rule);
//        $("#pActDetailsModal").modal('hide');
//    });
//};


});

function SaveModal(model) {
    console.log("model = " + model);
    var rule = {};
    rule.role = $("#selRole").val();
    rule.minValue = $("#txtMinValue").val();
    rule.maxValue = $("#txtMaxValue").val()
    rule.taxPer = $("#txtTaxPer").val();

    var tr = $("<tr></tr>");
    tr.append("<td>" + rule.role + "</td>");
    tr.append("<td>" + rule.minValue + "</td>");
    tr.append("<td>" + rule.maxValue + "</td>");
    tr.append("<td>" + rule.taxPer + "</td>");
    $("#txtDetailMaxValue").val = rule.maxValue;
    var tbody = $("#tbl tbody");
    tbody.append(tr);

    console.log(rule);
    $("#pActDetailsModal").modal('hide');
};