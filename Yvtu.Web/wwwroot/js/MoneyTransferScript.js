
$(document).ready(function () {
    $("#txtPartnerId").keyup(function (param) {
        CheckThenGetData();
    });


    $("#txtPartnerId").keydown(function (param) {
        var code = param.keyCode || param.which;
        if (code === 9) {
            CheckThenGetData();
        }
    });

    $("#txtPartnerId").keyup(function (param) {
        var code = param.keyCode || param.which;
        console.log(code);
        CheckThenGetData();
    });

    function CheckThenGetData() {
        if ($("#txtPartnerId").val().length == 9) {
            if (matchString($("#txtPartnerId").val()) === 1) {
                callBasicInfo($("#txtPartnerId").val());
            }
        }
    }

    CheckThenGetData();

    //if ($("#txtPartnerId").val().length == 9) {
    //    callBasicInfo($("#txtPartnerId").val());
    //}

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
                    document.getElementById('lblFixedFactor').innerHTML = result.fixedFactor;
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
        document.getElementById('lblFixedFactor').innerHTML = '';
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
        var fixedFactor = document.getElementById('lblFixedFactor').innerHTML;
        var bonusPer = document.getElementById('lblBonusPercent').innerHTML;
        var bonustaxPer = document.getElementById('lblBounsTaxPercent').innerHTML;
        if (fixedFactor <= 0) fixedFactor = 1;
        var netAmount = (amount * fixedFactor);
        var taxAmount = netAmount * (taxPer / 100);
        var bounsAmount = netAmount * (bonusPer / 100);
        var bounsTaxAmount = bounsAmount * (bonustaxPer / 100);
        var recAmount = netAmount;

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
