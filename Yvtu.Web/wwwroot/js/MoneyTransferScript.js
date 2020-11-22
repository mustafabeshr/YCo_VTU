
$(document).ready(function () {
    $("#txtPartnerId").on("change", function () {
        callBasicInfo($("#txtPartnerId").val());
    });

    if ($("#txtPartnerId").val().length == 9) {
        callBasicInfo($("#txtPartnerId").val());
    }

    function callBasicInfo(id) {
        $.ajax({
            url: "/MoneyTransfer/GetBasicInfo",
            type: "GET",
            data: { pId:  id}, 
            traditional: true,
            success: function (result) {
                //console.log(result);
                if (result.error == "N/A") {
                    document.getElementById('lblPartnerName').innerHTML = result.partnerName;
                    document.getElementById('lblPartnerRoleName').innerHTML = result.partnerRoleName;
                    document.getElementById('lblPartnerBalance').innerHTML = numberWithCommas(result.partnerBalance);
                    document.getElementById('lblTaxPercent').innerHTML = result.taxPercent;
                    document.getElementById('lblBonusPercent').innerHTML = result.bonusPercent;
                    document.getElementById('lblBounsTaxPercent').innerHTML = result.bounsTaxPercent;
                    //FreeUpAmountControls();
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
    }

    $("#txtPartnerId").on("input", function (e) {
        var input = $(this);
        var val = input.val();
        var oldValue = input.data("lastval");
        if (oldValue != val) {
            input.data("lastval", val);
            FreeUpControls();
            //console.log(oldValue + "- " +val);
        }
    });

    $("#txtAmount").on("input", function (e) {
        var input = $(this);
        var val = input.val();
        CalculateAmounts(val);

        //console.log(val + '   ' + val / ((taxPer / 100) + 1));

    });

    if ($("#txtAmount").val() > 0) {
        CalculateAmounts($("#txtAmount").val());
    }

    function FreeUpControls() {
        document.getElementById('lblPartnerName').innerHTML = '';
        document.getElementById('lblPartnerRoleName').innerHTML = '';
        document.getElementById('lblPartnerBalance').innerHTML = '';
        document.getElementById('lblTaxPercent').innerHTML = '';
        document.getElementById('lblBonusPercent').innerHTML = '';
        document.getElementById('lblBounsTaxPercent').innerHTML = '';
        //document.getElementById('lblNetAmount').innerHTML = '';
        //document.getElementById('lblTaxAmount').innerHTML = '';
        //document.getElementById('lblBounsAmount').innerHTML = '';
        //document.getElementById('lblBounsTaxAmount').innerHTML = '';
        //document.getElementById('lblReceivedAmount').innerHTML = '';
        //document.getElementById('lblAmountName').innerHTML = '';
    }

    function FreeUpAmountControls() {
        document.getElementById('lblNetAmount').innerHTML = '';
        document.getElementById('lblTaxAmount').innerHTML = '';
        document.getElementById('lblBounsAmount').innerHTML = '';
        document.getElementById('lblBounsTaxAmount').innerHTML = '';
        document.getElementById('lblReceivedAmount').innerHTML = '';
        document.getElementById('txtAmount').value = 0;
        document.getElementById('lblAmountName').innerHTML = '';
        document.getElementById('txtRequestAmount').value = 0;
    }

    function CalculateAmounts(amount) {
        var taxPer = document.getElementById('lblTaxPercent').innerHTML;
        var bonusPer = document.getElementById('lblBonusPercent').innerHTML;
        var bonustaxPer = document.getElementById('lblBounsTaxPercent').innerHTML;
        var netAmount = (amount / ((taxPer / 100) + 1));
        var taxAmount = netAmount * (taxPer / 100);
        var bounsAmount = netAmount * (bonusPer / 100);
        var bounsTaxAmount = bounsAmount * (bonustaxPer / 100);
        var recAmount = (amount - bounsAmount + bounsTaxAmount);

        document.getElementById('lblNetAmount').innerHTML = numberWithCommas(netAmount.toFixed(2));
        document.getElementById('lblTaxAmount').innerHTML = numberWithCommas(taxAmount.toFixed(2));
        document.getElementById('lblBounsAmount').innerHTML = numberWithCommas(bounsAmount.toFixed(2));
        document.getElementById('lblBounsTaxAmount').innerHTML = numberWithCommas(bounsTaxAmount.toFixed(2));
        document.getElementById('lblReceivedAmount').innerHTML = numberWithCommas(recAmount.toFixed(2));
        document.getElementById('lblAmountName').innerHTML = tafqeet(amount);
        document.getElementById('txtRequestAmount').value = amount;
    }

    function numberWithCommas(x) {
        return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }
    });
