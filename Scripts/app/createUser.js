var createUser = {
    clickSaveUser: function () {
        $("#btnAddUser").click(function (e) {
            e.preventDefault();
            if (createUser.validateForm()) {
                $("form#formUser").submit();
            }
        });
    },
    validateForm: function () {
        var formIsValid = true;
        var isCheckboxChecked = false;
        var schoolIdsChecked = [];
        createUser.trimSpaces();
        $('input:text').each(function () {
            if ($("#" + this.name).val() == "") {
                $("#required" + this.name).html("Required");
                formIsValid = false;
            }
            else {
                $("#required" + this.name).html("");
            }
        });

        if (($('#Password').val().length > 0) && ($('#Password').val().length < 6)) {
            $("#requiredPassword").html("Please enter password at least 6 characters.");
            formIsValid = false;
        } else if ($('#Password').val().length > 5) {
            $("#requiredPassword").html("");
        }

        $(".SCB:checked").each(function () {
            var chkBoxId = this.id.replace("userSchoolId_", "");
            var schoolId = $("#hiddenUserSchoolId_" + chkBoxId).val();
            schoolIdsChecked.push(schoolId);
            $("#SelectedSchools").val(schoolIdsChecked);
            isCheckboxChecked = true;
        });

        if (!isCheckboxChecked) {
            $("#requiredCheckbox").html("Required");
            formIsValid = false;
        } else {
            $("#requiredCheckbox").html("");
        }
        return formIsValid;
    },
    backButtonClick: function () {
        $("#btnBack").click(function () {
            window.location.href = "/User/Index";
        });
    },
    trimSpaces: function () {
        $('input:text').each(function () {
            $("#" + this.name).val($.trim($("#" + this.name).val()));
        });
    },
    checkboxValidation: function (serverChekboxMessage) {
        if (serverChekboxMessage != "") {
            $("#requiredCheckbox").html("Required");
        }
    }
}
