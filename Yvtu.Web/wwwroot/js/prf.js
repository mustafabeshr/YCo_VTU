﻿$(document).ready(function () {
    
    $("#txtId").keyup(function (data) {
        if ($("#txtId").val().length == 9) {
            if (matchString($("#txtId").val()) === 1) {
                callBasicInfo($("#txtId").val());
                $(':input[type="submit"]').prop('disabled', false);
            }
        }
        else {
            $(':input[type="submit"]').prop('disabled', true);
            FreeUpControls();
        }
    });

    if ($("#txtId").val().length == 9) {
        if (matchString($("#txtId").val()) === 1) {
            callBasicInfo($("#txtId").val());
            $(':input[type="submit"]').prop('disabled', false);
        }
    }
    else {
        $(':input[type="submit"]').prop('disabled', true);
        FreeUpControls();
    }


    function matchString(value) {
        var result = value.match(/^70\d*/);
        if (result === null) {
            return 0;
        }
        else {
            return 1;
        }
    }  

    function FreeUpControls() {
        document.getElementById('lblCnt').innerHTML = '(0)';
        removeOptions(document.getElementById('selAccounts'));
        
    }

    function removeOptions(selectElement) {
        var i, L = selectElement.options.length - 1;
        for (i = L; i >= 0; i--) {
            selectElement.remove(i);
        }
    }

    function callBasicInfo(id) {
        $list = $("#selAccounts");
        $.ajax({
            url: "/Account/GetAccounts",
            type: "GET",
            data: { id: id },
            traditional: true,
            success: function (result) {
                if (result != null) {
                    $list.empty();
                    $.each(result, function (i, item) {
                        $list.append('<option value="' + item["id"] + '"> ' + item["name"] + ' </option>');
                    });
                    document.getElementById('lblCnt').innerHTML = '(' + $list.length + ')';
                }
                else {
                    $('#lblError').show("slow").delay(2000).hide("slow");
                }
            },
            error: function (err) {
                console.log("error = " + err);
            }
        });
    }

    
});