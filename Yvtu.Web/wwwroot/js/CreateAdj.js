$(document).ready(function () {

    $("#txtAmount").on("input", function (e) {
        var input = $(this);
        var val = input.val();
        CalculateAmounts(val);
        //console.log();
    });

    if ($("#txtAmount").val() > 0) {
        CalculateAmounts($("#txtAmount").val());
    }


    function CalculateAmounts(amount) {
        var taxPer = document.getElementById('txtTaxPercent').value;
        var bonusPer = document.getElementById('txtBonusPercent').value;
        var bonustaxPer = document.getElementById('txtBounsTaxPercent').value;
        var OAmount = document.getElementById('txtOAmount').value;
        var ONetAmount = document.getElementById('txtONetAmount').value;
        var OReceivedAmount = document.getElementById('txtOReceivedAmount').value;
        var OTaxAmount = document.getElementById('txtOTaxAmount').value;
        var OBounsAmount = document.getElementById('txtOBounsAmount').value;
        var OBounsTaxAmount = document.getElementById('txtOBounsTaxAmount').value;

         

        var netAmount = (amount / ((taxPer / 100) + 1));
        var taxAmount = netAmount * (taxPer / 100);
        var bounsAmount = netAmount * (bonusPer / 100);
        var bounsTaxAmount = bounsAmount * (bonustaxPer / 100);
        var recAmount = (amount - bounsAmount + bounsTaxAmount);

        var ExpAmount = Number(OAmount) - Number(amount);
        var ExpNetAmount = Number(ONetAmount) - netAmount;
        var ExpReceiveAmount = Number(OReceivedAmount) - recAmount;
        var ExpTaxAmount = Number(OTaxAmount) - taxAmount;
        var ExpBounsAmount = Number(OBounsAmount) - bounsAmount;
        var ExpBounsTaxAmount = Number(OBounsTaxAmount) - bounsTaxAmount;

        document.getElementById('lblNetAmount').innerHTML = numberWithCommas(netAmount.toFixed(2));
        document.getElementById('lblTaxAmount').innerHTML = numberWithCommas(taxAmount.toFixed(2));
        document.getElementById('lblBounsAmount').innerHTML = numberWithCommas(bounsAmount.toFixed(2));
        document.getElementById('lblBounsTaxAmount').innerHTML = numberWithCommas(bounsTaxAmount.toFixed(2));
        document.getElementById('lblReceivedAmount').innerHTML = numberWithCommas(recAmount.toFixed(2));

        document.getElementById('lblExpAmount').innerHTML = numberWithCommas(ExpAmount.toFixed(2));
        document.getElementById('lblExpTaxAmount').innerHTML = numberWithCommas(ExpTaxAmount.toFixed(2));
        document.getElementById('lblExpBounsAmount').innerHTML = numberWithCommas(ExpBounsAmount.toFixed(2));
        document.getElementById('lblExpBounsTaxAmount').innerHTML = numberWithCommas(ExpBounsTaxAmount.toFixed(2));
        document.getElementById('lblExpNetAmount').innerHTML = numberWithCommas(ExpNetAmount.toFixed(2));
        document.getElementById('lblExpReceivedAmount').innerHTML = numberWithCommas(ExpReceiveAmount.toFixed(2));
        
        if (ExpAmount < 0 || isNaN(ExpAmount)) {
            document.getElementById('lblExpAmount').style = "font-size:18px;font-weight:bold;color:white;background-color:red;";
            document.getElementById('lblExpTaxAmount').style = "font-size:18px;font-weight:bold;color:white;background-color:red;";
            document.getElementById('lblExpBounsAmount').style = "font-size:18px;font-weight:bold;color:white;background-color:red;";
            document.getElementById('lblExpBounsTaxAmount').style = "font-size:18px;font-weight:bold;color:white;background-color:red;";
            document.getElementById('lblExpNetAmount').style = "font-size:18px;font-weight:bold;color:white;background-color:red;";
            document.getElementById('lblExpReceivedAmount').style = "font-size:18px;font-weight:bold;color:white;background-color:red;";
        } else {
            document.getElementById('lblExpAmount').style = "font-size:18px;font-weight:bold;color:purple;background-color:white;";
            document.getElementById('lblExpTaxAmount').style = "font-size:18px;font-weight:bold;color:purple;background-color:white;";
            document.getElementById('lblExpBounsAmount').style = "font-size:18px;font-weight:bold;color:purple;background-color:white;";
            document.getElementById('lblExpBounsTaxAmount').style = "font-size:18px;font-weight:bold;color:purple;background-color:white;";
            document.getElementById('lblExpNetAmount').style = "font-size:18px;font-weight:bold;color:purple;background-color:white;";
            document.getElementById('lblExpReceivedAmount').style = "font-size:18px;font-weight:bold;color:purple;background-color:white;";
        }

        //document.getElementById('lblAmountName').innerHTML = tafqeet(amount);
    }

    function numberWithCommas(x) {
        return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }
});