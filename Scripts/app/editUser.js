var editUser = {
    clickSaveUser: function () {
        $("#btnSaveUser").click(function (e) {
            e.preventDefault();
            if (editUser.validateForm()) {
                $("form#formUser").submit();
            }
        });
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
    validateForm: function () {
        var formIsValid = true;
        var isCheckboxChecked = false;
        var schoolIdsChecked = [];
        editUser.trimSpaces();
        $('input:text').each(function () {
            if ($("#" + this.name).val() == "") {
                $("#required" + this.name).html("Required");
                formIsValid = false;
            }
            else {
                $("#required" + this.name).html("");
            }
        });

        $(".SCB:checked").each(function () {
            var chkBoxId = this.id.replace("userSchoolId_", "");
            var schoolId = $("#hiddenUserSchoolId_" + chkBoxId).val();
            schoolIdsChecked.push(schoolId);
        });

        $(".DCB:checked").each(function () {
            var chkBoxId = this.id.replace("userSchoolId_", "");
            var schoolId = $("#hiddenUserSchoolId_" + chkBoxId).val();
            schoolIdsChecked.push(schoolId);
        });


        if (schoolIdsChecked != null && schoolIdsChecked.length > 0) {
            $("#SelectedSchools").val(schoolIdsChecked);
            isCheckboxChecked = true;
        }

        if (!isCheckboxChecked) {
            $("#requiredCheckbox").html("Required");
            formIsValid = false;
        } else {
            $("#requiredCheckbox").html("");
        }
        return formIsValid;
    },
    getClassDataOnYearChange: function () {
        $('#ddlYear').change(function () {
            editUser.getClassData();
        });
    },
    getClassData: function () {
        $.ajax({
            url: "/JsonService/RefreshClassByTeacher/",
            type: "POST",
            datatype: "json",
            data: { yearId: $('#ddlYear').val(), teacherId: $('#UserId').val(), schoolId: -1 }
        })
            .then(
            function (response) {
                $('#tblClassGrid').empty();
                var htm = "";
                htm += "<thead><tr class='gridHead'><th >Class Name</th></tr></thead>";
                htm += "<tbody>";
                if (response != null && response != "") {
                    $.each(response, function (key, ddlData) {
                        htm += "<tr><td><a href='/Classes/Edit/" + ddlData.Id + "'> " + ddlData.Name + " </a></td></tr>";
                    })
                }
                else {
                    htm += "<tr><td>No Classes</td></tr>";
                }
                htm += "<tbody>";
                $("#tblClassGrid").html(htm);
            },
            function (xhr, status) {
                alert("error: " + xhr.statusText);
            });

    },
    showHideUIControls: function (schoolYearId, currentSchoolYearId, allowEdit) {
        $('input:text').addClass("form-control");

        if (schoolYearId != currentSchoolYearId) {
            $("#btnSaveUser").hide();
            if (allowEdit != "False") {
                $("#paraEditCurrentYear").show();
            }
            $('input[type="text"], input[type="radio"], input[type="checkbox"], select, input[type="datetime"]').prop("disabled", true);
        }
        else {
            if (allowEdit == "False") {
                $("#btnSaveUser").hide();
                $("#paraEditCurrentYear").hide();
                $("#RoleId").prop("disabled", "true");
                $(".SCB").each(function () {
                    $("#" + this.id).prop("disabled", true)
                });
                $(".DCB").each(function () {
                    $("#" + this.id).prop("disabled", true)
                });
                $('input:text').each(function () {
                    $("#" + this.id).prop("disabled", true)
                });
            } else {
                $(".DCB").attr("disabled", true);
                $("#btnSaveUser").show();
                $("#paraEditCurrentYear").hide();
            }
        }

    },
    checkboxValidation: function (serverChekboxMessage) {
        if (serverChekboxMessage != "") {
            $("#requiredCheckbox").html("Required");
        }
    }
}
