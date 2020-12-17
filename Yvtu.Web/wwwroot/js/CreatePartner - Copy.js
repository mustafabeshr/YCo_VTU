$(document).ready(function () {
    $("#citySelect").on("change", function () {
        $list = $("#districtSelect");
        $.ajax({
            url: "/Account/GetDistrictsByCity",
            type: "GET",
            data: { id: $("#citySelect").val() }, //id of the state which is used to extract cities
            traditional: true,
            success: function (result) {
                $list.empty();
                $.each(result, function (i, item) {
                    $list.append('<option value="' + item["id"] + '"> ' + item["name"] + ' </option>');
                });
            },
            error: function () {
                alert("Something went wrong call the police");
            }
        });
    });
});