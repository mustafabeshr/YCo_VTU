function confirmDelete(isDeleteClicked) {
    var deleteSpan = 'deleteSpan';
    var confirmDeleteSpan = 'deleteConfirmSpan';

    if (isDeleteClicked) {
        $('#' + deleteSpan).hide();
        $('#' + confirmDeleteSpan).show();
    } else {
        $('#' + deleteSpan).show();
        $('#' + confirmDeleteSpan).hide();
    }
}

$(document).ready(function () {
    $("#txtPartnerId").on("change", function () {
        $.ajax({
            url: "/MoneyTransfer/GetBasicInfo",
            type: "GET",
            data: { pId: $("#txtPartnerId").val() }, //id of the state which is used to extract cities
            traditional: true,
            success: function (result) {
                console.log(result);
                if (result.error == "N/A") {
                    document.getElementById('lblPartnerName').innerHTML = result.partnerName;
                    document.getElementById('lblPartnerRoleName').innerHTML = result.partnerRoleName;
                    document.getElementById('lblPartnerBalance').innerHTML = result.partnerBalance;
                    document.getElementById('lblTaxPercent').innerHTML = result.taxPercent;
                    document.getElementById('lblBonusPercent').innerHTML = result.bonusPercent;
                    document.getElementById('lblBounsTaxPercent').innerHTML = result.bounsTaxPercent;
                }
                else {
                    document.getElementById('lblError').innerHTML = result.error;
                    $('#lblError').show("slow").delay(2000).hide("slow");
                }
            },
            error: function (err) {
                //alert("Something went wrong call the police");
                console.log(err);
            }
        });
    });

    $("#txtPartnerId").on("input", function (e) {
        var input = $(this);
        var val = input.val();
        var oldValue = input.data("lastval");
        if (oldValue != val) {
            input.data("lastval", val);
            document.getElementById('lblPartnerName').innerHTML = '';
            document.getElementById('lblPartnerRoleName').innerHTML = '';
            document.getElementById('lblPartnerBalance').innerHTML = '';
            //your change action goes here 
            //console.log(oldValue + "- " +val);
        }

    });
});