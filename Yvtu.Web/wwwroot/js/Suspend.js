$(document).ready(function () {
    //$("#txtId").on("change", function () {
    //    if ($("#txtId").val().length == 9) {
    //        callBasicInfo($("#txtId").val());
    //        //console.log($("#txtId").val());
            
    //    }
    //    console.log(matchString());
    //});
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
        //var string = $("#txtId").val();
        var result = value.match(/^70\d*/);
        if (result === null) {
            return 0;
        }
        else {
            return 1;
        }
    }  

    function FreeUpControls() {
        document.getElementById('lblId').innerHTML = '';
        document.getElementById('lblAccount').innerHTML = '';
        document.getElementById('lblName').innerHTML = '';
        document.getElementById('lblBalance').innerHTML = '';
        document.getElementById('lblRole').innerHTML = '';
        document.getElementById('lblStatus').innerHTML = '';
        document.getElementById('lblLastLogin').innerHTML = '';
    }

    function callBasicInfo(id) {
        $.ajax({
            url: "/Account/GetBasicInfo4S",
            type: "GET",
            data: { id: id },
            traditional: true,
            success: function (result) {
                //console.log(result);
                if (result.error == "N/A") {
                    document.getElementById('lblId').innerHTML = result.id;
                    document.getElementById('lblAccount').innerHTML = result.account;
                    document.getElementById('lblName').innerHTML = result.name;
                    document.getElementById('lblBalance').innerHTML = numberWithCommas(result.balance);
                    document.getElementById('lblRole').innerHTML = result.role.name;
                    document.getElementById('lblStatus').innerHTML = result.status.name;
                    document.getElementById('lblLastLogin').innerHTML = result.lastLoginOn;
                    //FreeUpAmountControls();
                }
                else {
                    console.log(result);
                    document.getElementById('lblError').innerHTML = result.error;
                    $('#lblError').show("slow").delay(2000).hide("slow");
                }
            },
            error: function (err) {
                //alert("Something went wrong call the police");
                console.log("error = " + err);
            }
        });
    }

    function numberWithCommas(x) {
        return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }
});