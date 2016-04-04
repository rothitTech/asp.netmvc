var studentDetail = {
    clickSaveStudent: function () {
        $("#btnSaveStudent").click(function () {
        if (studentDetail.validateForm()) {
            $('#hdSchoolId').val($('#ddlSchool').val());
            $('#hdLineageId').val($('#ddlLineage').val());
            $('#hdGradeId').val($('#ddlGrade').val());
            $('#hdGenderId').val($('#ddlGender').val());
            $('#hdRaceId').val($('#ddlRace').val());
            $('#hdHomeLanguageId').val($('#ddlHomeLanguage').val());
            $('#hdNativeLanguageId').val($('#ddlNativeLanguage').val());
            $("form#formStudent").submit();
        }
        return false;
        });
    },
    checkNegativeAndAlphabets: function () {
        $("#LocalId").keypress(function (e) {
            if (e.which != 8 && (e.which < 48 || e.which > 57)) {
                return false;
            }
        });
    },
    backButtonClick: function () {
        $("#btnBack").click(function () {
            window.location.href = "/Students/Index";
        });
    },
    loadDatePicker: function () {
        $('.datepicker').datepicker({
            format: 'mm/dd/yy',
            endDate: '+0d',
            autoclose: true
        })
    },
    validateForm :  function()
    {
        var formIsValid = true;
        this.trimSpaces();
        if ($("#FirstName").val() == "") {
            $("#requiredFirstName").html("Required");
            formIsValid = false;
        }
        else {
            if (this.isNumeric($('#FirstName').val())) {
                formIsValid = false;
            }
            else {
                $("#requiredFirstName").html("");
            }
        }
        if ($("#LastName").val() == "") {
            $("#requiredLastName").html("Required");
            formIsValid = false;
        }
        else {
            $("#requiredLastName").html("");
        }
        if ($("#LocalId").val() == "") {
            $("#requiredLocalId").html("Required");
            formIsValid = false;
        }
        else {
            $("#requiredLocalId").html("");
        }
        return formIsValid;
    },
    trimSpaces: function ()
    {
        $('input:text').each(function () {
            $("#" + this.name).val($.trim($("#" + this.name).val()));
        });
    },
    isNumeric: function (str)
    {
        var noIsNumeric = /^\d+$/.test(str);
        if (noIsNumeric) {
            $("#requiredFirstName").html("First Name should not be a number");
        }
        return noIsNumeric;
    },
}
