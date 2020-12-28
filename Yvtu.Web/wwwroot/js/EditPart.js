$(document).ready(function () {
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
        document.getElementById('txtName').value = '';
        document.getElementById('txtPairMobile').value = '';
        document.getElementById('txtBrandName').value = '';
        document.getElementById('txtPersonalIdTypeName').value = '';
        document.getElementById('txtPersonalIdNo').value = '';
        document.getElementById('txtPersonalIssued').value = '';
        document.getElementById('txtPersonalIdPlace').value = '';
        document.getElementById('txtCityName').value = '';
        document.getElementById('txtDistrictName').value = '';
        document.getElementById('txtStreet').value = '';
        document.getElementById('txtZone').value = '';
        document.getElementById('txtMobileNo').value = '';
        document.getElementById('txtFixed').value = '';
        document.getElementById('txtFax').value = '';
        document.getElementById('txtEmail').value = '';
        document.getElementById('txtRefPartnerId').value = '';
        document.getElementById('txtIPAddress').value = '';
        document.getElementById('txtExtraAddressInfo').value = '';

        document.getElementById('txtName2').value = '';
        document.getElementById('txtPairMobile2').value = '';
        document.getElementById('txtBrandName2').value = '';
        //document.getElementById('selIdType').value = '';
        document.getElementById('txtPersonalIdNo2').value = '';
        document.getElementById('txtPersonalIssued2').value = '';
        document.getElementById('txtPersonalIdPlace2').value = '';
        //document.getElementById('citySelect').value = '';
        //document.getElementById('districtSelect').value = '';
        document.getElementById('txtStreet2').value = '';
        document.getElementById('txtZone2').value = '';
        document.getElementById('txtMobileNo2').value = '';
        document.getElementById('txtFixed2').value = '';
        document.getElementById('txtFax2').value = '';
        document.getElementById('txtEmail2').value = '';
        document.getElementById('txtRefPartnerId2').value = '';
        document.getElementById('txtIPAddress2').value = '';
        document.getElementById('txtExtraAddressInfo2').value = '';
    }

    function callBasicInfo(id) {
        $.ajax({
            url: "/Account/GetP",
            type: "GET",
            data: { id: id },
            traditional: true,
            success: function (result) {
                
                if (result.extra == "N/A") {
                    document.getElementById('txtName').value = result.name;
                    document.getElementById('txtPairMobile').value = result.pairMobile;
                    document.getElementById('txtBrandName').value = result.brandName;
                    document.getElementById('txtPersonalIdTypeName').value = result.personalId.idType.name;
                    document.getElementById('txtPersonalIdNo').value = result.personalId.id;
                    document.getElementById('txtPersonalIssued').value = formatDate(new Date(result.personalId.issued));
                    document.getElementById('txtPersonalIdPlace').value = result.personalId.place;
                    document.getElementById('txtCityName').value = result.address.city.name;
                    document.getElementById('txtDistrictName').value = result.address.district.name;
                    document.getElementById('txtStreet').value = result.address.street;
                    document.getElementById('txtZone').value = result.address.zone;
                    document.getElementById('txtMobileNo').value = result.contactInfo.mobile;
                    document.getElementById('txtFixed').value = result.contactInfo.fixed;
                    document.getElementById('txtFax').value = result.contactInfo.fax;
                    document.getElementById('txtEmail').value = result.contactInfo.email;
                    document.getElementById('txtRefPartnerId').value = result.refPartner.id;
                    document.getElementById('txtIPAddress').value = result.ipAddress;
                    document.getElementById('txtExtraAddressInfo').value = result.address.extraInfo;

                    if (document.getElementById('txtName2').value === '') {

                        document.getElementById('txtName2').value = result.name;
                        document.getElementById('txtPairMobile2').value = result.pairMobile;
                        document.getElementById('txtBrandName2').value = result.brandName;
                        document.getElementById('selIdType').value = result.personalId.idType.id;
                        document.getElementById('txtPersonalIdNo2').value = result.personalId.id;
                        document.getElementById('txtPersonalIssued2').value = formatDate(new Date(result.personalId.issued));
                        document.getElementById('txtPersonalIdPlace2').value = result.personalId.place;
                        document.getElementById('citySelect').value = result.address.city.id;
                        document.getElementById('districtSelect').value = result.address.district.id;
                        document.getElementById('txtStreet2').value = result.address.street;
                        document.getElementById('txtZone2').value = result.address.zone;
                        document.getElementById('txtMobileNo2').value = result.contactInfo.mobile;
                        document.getElementById('txtFixed2').value = result.contactInfo.fixed;
                        document.getElementById('txtFax2').value = result.contactInfo.fax;
                        document.getElementById('txtEmail2').value = result.contactInfo.email;
                        document.getElementById('txtRefPartnerId2').value = result.refPartner.id;
                        document.getElementById('txtIPAddress2').value = result.ipAddress;
                        document.getElementById('txtExtraAddressInfo2').value = result.address.extraInfo;
                    }
                    //FreeUpAmountControls();
                }
                else {
                    //console.log(result);
                    document.getElementById('lblError').innerHTML = result.extra;
                    $('#lblError').show("slow").delay(2000).hide("slow");
                }
            },
            error: function (err) {
                //alert("Something went wrong call the police");
                console.log("error = " + err);
            }
        });
    }


    function formatDate(date) {
        var d = new Date(date),
            month = '' + (d.getMonth() + 1),
            day = '' + d.getDate(),
            year = d.getFullYear();

        if (month.length < 2)
            month = '0' + month;
        if (day.length < 2)
            day = '0' + day;

        return [year, month, day].join('-');
    }
    $("#citySelect").on("change", function () {
        populateDistricts($("#citySelect").val());
    });
    function populateDistricts(city) {
        $list = $("#districtSelect");
        $.ajax({
            url: "/Account/GetDistrictsByCity",
            type: "GET",
            data: { id: city }, //id of the state which is used to extract cities
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
    }

    populateDistricts($("#citySelect").val());

    $("#txtName2").keyup(function () {

        var oldValue = $("#txtName").val();
        var newValue = $("#txtName2").val();
        if (oldValue != newValue) {
            document.getElementById("txtName2").style.color = "blue";
        }
        else {
            document.getElementById("txtName2").style.color = "black";
        }
    });

});