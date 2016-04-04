var assessmentClassScore = {
    changedRecords: [],
    msgValidateAllFields: "Please validate all fields to proceed.",
    msgAssessmentExists: "Assessment for this Assessment type and School Term doesn't exists. Please first add assessment from Add Assessment Screen before saving new records.",

    onChangeOfSchool: function () {
        $('#ddlSchool').change(function () {
            assessmentClassScore.updateTeacherBySchool($('#ddlSchool').val(), $('#ddlYear').val());
        });
    },
    onChangeOfTeacher: function () {
        $('#ddlTeacher').change(function () {
            assessmentClassScore.updateClassDropDown();
        });
    },
    onChangeOfSchoolYear: function () {
        $('#ddlYear').change(function () {
            assessmentClassScore.updateTeacherBySchool($('#ddlSchool').val(), $('#ddlYear').val());
            assessmentClassScore.updateSchoolByYear($('#ddlYear').val());
            assessmentClassScore.updateClassDropDown();
        });
    },
    updateSchoolByYear: function (schoolYearId) {
        $.ajax({
            url: "/JsonService/RefreshSchoolByYear?schoolYearId=" + schoolYearId,
            type: "POST",
            datatype: "json"
        })
        .then(
        function (response) {
            $('#ddlSchool').empty();
            $('select#ddlSchool').append('<option value="">--SELECT--</option>');
            $.each(response, function (key, ddlData) {
                $('select#ddlSchool').append(
                    '<option value="' + ddlData.Id + '">'
                    + ddlData.Name +
                    '</option>');
            });
        },
        function (xhr, status) {
        });
    },
    updateTeacherBySchool: function (schoolId, schoolYearId) {
        $.ajax({
            url: "/JsonService/RefreshTeacherBySchool?schoolId=" + schoolId + "&schoolYearId=" + schoolYearId,
            type: "POST",
            datatype: "json"
        })
        .then(
        function (response) {
            $('#ddlTeacher').empty();
            $('select#ddlTeacher').append('<option value="">--SELECT--</option>');
            $.each(response, function (key, ddlData) {
                $('select#ddlTeacher').append(
                    '<option value="' + ddlData.Id + '">'
                    + ddlData.Name +
                    '</option>');
            });
        },
        function (xhr, status) {
        });
    },
    updateClassDropDown: function UpdateClassDropDown() {
        $("#ddlClass").prop("disabled", false);
        $.ajax({
            url: "/JsonService/RefreshClassByTeacher/",
            type: "POST",
            datatype: "json",
            data: { yearId: $('#ddlYear').val(), teacherId: $('#ddlTeacher').val(), schoolId: $('#ddlSchool').val() }
        })
        .then(
        function (response) {
            $('#ddlClass').empty();
            if (response != null && response != "") {
                if (response.length > 1) {
                    $('select#ddlClass').append('<option value="">--SELECT--</option>');
                }
                $.each(response, function (key, ddlData) {
                    $('select#ddlClass').append(
                        '<option value="' + ddlData.Id + '">'
                        + ddlData.Name +
                        '</option>');

                });
                $("#ddlClass").prop("disabled", false);
            }
            else {
                $("#ddlClass").prop("disabled", false);
                $('select#ddlClass').append('<option value="">--SELECT--</option>');
            }
        },
        function (xhr, status) {
        });
    },

    insertHeadersToReportGrid: function (headerRow, columnCount) {
        $(".assessmentClassScoreGrid").find("thead").prepend(headerRow);
        $(".assessmentClassScoreGrid").find("thead").prepend("<tr class='row-mergedcols'><td colspan='" + columnCount + "'></td></tr>");
        var ths = $(".assessmentClassScoreGrid").find(".row-report-columns").find("th");
        $(ths[0]).addClass("col-student").addClass("fixed-column"); //STUDENT
    },
    adjustColumnHeaderStyle: function (columnCount) {
        var gridColumnRowIndexAccordingToStyle = 2;
        var headerCells = $(".assessmentClassScoreGrid tr:eq(" + gridColumnRowIndexAccordingToStyle + ") th");
        var firstRowCells = $(".assessmentClassScoreGrid tr:eq(" + (gridColumnRowIndexAccordingToStyle + 1) + ") td");

        $.each(firstRowCells, function (index, value) {
            var hasColScoreInner = $(firstRowCells[index]).hasClass('col-score-inner');
            var headerCol = $(headerCells[index]);
            headerCol.addClass($(firstRowCells[index]).attr("class"));
            if (hasColScoreInner) {
                headerCol.removeClass('col-score-inner');
            }
        });
    },
    uploadHeirarachicalReportOnNextPrevButtonClick: function (detailUrl, prevlinkStatus, nextlinkStatus) {
        $('#lnkNext').click(function (e) {
            if (!$("input[type=\"text\"]").hasClass("error")) {
                if (!assessmentClassScore.changedRecords.length > 0) {
                    if (nextlinkStatus == "link-disable") {
                        e.preventDefault();
                        return false;
                    }
                    var horizontalPageIndex = 1;
                    location.href = detailUrl + "&HorizontalPageIndex=" + horizontalPageIndex;
                }
                else {
                    assessmentClassScore.displayNavigationAlertMessage("Please save edited data before navigate");
                }
            }
            else {
                assessmentClassScore.displayValidationMessage(assessmentClassScore.msgValidateAllFields);
            }
        });
        $('#lnkPrev').click(function (e) {
            if (!$("input[type=\"text\"]").hasClass("error")) {
                if (!assessmentClassScore.changedRecords.length > 0) {
                    if (prevlinkStatus == "link-disable") {
                        e.preventDefault();
                        return false;
                    }
                    var horizontalPageIndex = 0;
                    location.href = detailUrl + "&HorizontalPageIndex=" + horizontalPageIndex;
                }
                else {
                    assessmentClassScore.displayNavigationAlertMessage("Please save edited data before navigate");
                }
            }
            else {
                assessmentClassScore.displayValidationMessage(msgValidateAllFields);
            }
        });
    },

    validateScoreTargetCellValue: function (txt, isTarget) {
        var cellText = isTarget ? "Target" : "Score";
        var cellClass = isTarget ? "edited edited-target" : "edited edited-score";
        var _this = this;
        $("#divSuccessMessage").html("");
        var match = _this.isNumeric(txt.val());
        if (match == null) {
            _this.addErrorClass(txt, '' + cellText + ' must be an integer');
            return;
        }
        $(txt).removeClass("error").attr('title', '');
        var scoreMin = txt.attr('scoreMin') != "" ? parseInt(txt.attr('scoreMin')) : null;
        var scoreMax = txt.attr('scoreMax') != "" ? parseInt(txt.attr('scoreMax')) : null;
        var value = txt.val() != "" ? parseInt(txt.val()) : txt.val();
        var assessmentId = txt.attr('assessmentId');
        var assessmentScoreId = txt.attr('scoreId');
        if (assessmentId == 0) {
            if (value != "") {
                _this.addErrorClass(txt, _this.msgAssessmentExists);
            }
            return;
        }
        if (_this.validateRange(value, scoreMin, scoreMax)) {
            $(txt).removeClass("error").attr('title', '');
            $(txt).addClass(cellClass);
        }
        else {
            if (value == 0 && assessmentScoreId == 0) {
                $(txt).removeClass('error');
            }
            else {
                _this.addErrorClass(txt, '' + cellText + ' must be between ' + scoreMin + ' and ' + scoreMax + '.');
            }
        }
    },
    onEditingTextValue: function () {
        $('.cellScoreEditor').change(function (evt) {
            assessmentClassScore.validateScoreTargetCellValue($(this), false)
        });
        $('.cellTargetEditor').change(function (evt) {
            assessmentClassScore.validateScoreTargetCellValue($(this), true)
        });
    },
    createNewRecord: function (txt, isTarget) {
        //using "-999" as default value for score and target because these can have 0 and null as value.
        var defaultValue = -999;
        var score = defaultValue;
        var target = defaultValue;
        if (isTarget) {
            target = txt.val();
        }
        else {
            score = txt.val()
        }
        var newRecord = {
            assessmentScoreId: txt.attr('scoreId'),
            assessmentId: txt.attr('assessmentId'),
            studentId: txt.attr('studentId'),
            score: score,
            target: target,
            gradeLevel: txt.attr('gradeLevel'),
            schoolId: $('#ddlSchool').val()
        };
        return newRecord;
    },
    saveEditedRecords: function (detailUrl) {
        var _this = this;
        $('#btnSaveEditedRecords').click(function (e) {
            e.preventDefault();
            if ($("input[type=\"text\"]").hasClass("error")) {
                _this.displayValidationMessage(_this.msgValidateAllFields);
                return;
            }
            $('.edited-score').each(function (evt) {
                var newRecord = _this.createNewRecord($(this), false);
                _this.changedRecords.push(newRecord);
            });
            $('.edited-target').each(function (evt) {
                var newRecord = _this.createNewRecord($(this), true);
                //if score already pushed for same id, then only add target else add new record.
                var index = _this.findDuplicate(assessmentClassScore.changedRecords, newRecord);
                if (index != -1) {
                    _this.changedRecords[index].target = newRecord.target;
                }
                else {
                    _this.changedRecords.push(newRecord);
                }
            });
            if (_this.changedRecords.length > 0) {
                $("#divLoading").html("<img class='loaderImage' src='../Content/Images/loaderImg.gif' />");
                $("#divLoading").show();
                $.ajax({
                    type: 'POST',
                    url: '/AssessmentClassScore/SaveAssessmentClassScore',
                    contentType: 'application/json',
                    data: JSON.stringify({ model: _this.changedRecords }),
                    traditional: true,
                    success: function (response) {
                        _this.displayNavigationAlertMessage(response);
                        while (_this.changedRecords.length > 0) {
                            _this.changedRecords.pop();
                        }
                        $('input[type=\"text\"]').removeClass("edited edited-score edited-target");
                        location.href = detailUrl;
                    },
                    error: function (xhr, status, errorThrown) {
                        $("#divSuccessMessage").html(" Error: " + errorThrown);
                    },
                    complete: function () {
                        $("#divLoading").hide();
                    },
                });
            }
            else {
                $('input[type=\"text\"]').removeClass("edited");
                _this.displayValidationMessage("There is no edited record to save.");
            }

        });
    },

    cancelEditedRecords: function (detailUrl) {
        $('#btnCancel').click(function (e) {
            e.preventDefault();
            location.href = detailUrl;
        });
    },
    findDuplicate: function (array, obj) {
        var index = -1;
        $.map(array, function (elementOfArray, indexInArray) {
            if (obj.assessmentScoreId == 0 && elementOfArray.assessmentId == obj.assessmentId && elementOfArray.studentId == obj.studentId) {
                index = indexInArray;
            }
            else if (obj.assessmentScoreId != 0 && elementOfArray.assessmentScoreId === obj.assessmentScoreId) {
                index = indexInArray;
            }
        });
        return index;
    },
    validateRange: function (value, scoreMin, scoreMax) {
        if (value >= scoreMin && value <= scoreMax) {
            return true;
        }
        else {
            return false;
        }
    },
    isNumeric: function (value) {
        var regExp = "^[+]?[\\d ]*$";
        var match = value.match(regExp);
        return match;
    },
    displayNavigationAlertMessage: function (message) {
        $("#modalMessageBody").html(message);
        assessmentClassScore.showModalPopup("modalMessage", 5000);
    },
    addErrorClass: function (txt, message) {
        $(txt).addClass("error").attr('title', '' + message + '').focus();
    },
    displayValidationMessage: function (message) {
        $("#divSuccessMessage").html(message);
    },
    showModalPopup: function (modalId, timeOut) {
        modalMessageTimeout = setTimeout(function () {
            $("#" + modalId).modal('hide');
        }, timeOut);
        $("#" + modalId).modal('show');
    },

    showVerticalScorll: function () {
        var defaultScrollHeight = "300";
        var tableBodyHeight = $('.assessmentClassScoreGrid tbody').height();
        if (tableBodyHeight >= defaultScrollHeight) {
            $('.assessmentClassScoreGrid tbody').height(defaultScrollHeight);
            $(".assessmentClassScoreGrid tbody").css("overflow-y", "scroll");
        }
        else {
            $(".assessmentClassScoreGrid tbody").css("overflow-y", "auto");
        }
    },
    showHorizontalScroll: function () {
        var tableBodyWidth = $('.assessmentClassScoreGrid tbody').width();
        var defaultScrollWidth = $(".container").width();
        if (tableBodyWidth >= defaultScrollWidth) {
            $(".grid-container").css("overflow-x", "scroll");
        }
    },

    searchClassAssessmentScore: function () {
        $("#btnSearchAssessmentClassScore").click(function (e) {
            e.preventDefault();
            $.validator.unobtrusive.parse("form#formAssessmentScore");
            $("form#formAssessmentScore").validate();
            if ($("form#formAssessmentScore").valid()) {
                $("form#formAssessmentScore").submit();
                $("#divLoading").html("<img class='loaderImage' src='../Content/Images/loaderImg.gif' />");
                $("#divLoading").show();
            }
        });
    },
    fixHeaderAndLeftColumn: function (recordCount, columnCount) {
        if ($(".grid-report").length > 0) {
            var scrollYValue = this.getReportGridHeight();
            var arrayJSONColDefs = [];
            // Using columnDefs we can change default behaviour.
            this.adjustColumnDefs(arrayJSONColDefs, columnCount);
            var table = $('.grid-report').DataTable({
                scrollY: scrollYValue,
                scrollX: true,
                scrollCollapse: true,
                paging: false,
                columnDefs: arrayJSONColDefs,
            });
            new $.fn.dataTable.FixedColumns(table, {
                leftColumns: 1
            });
            var recordFoundHtm = "<div class='record-count'><b>Records Found: " + recordCount + "</b></div>";
            $(".dataTables_wrapper").prepend(recordFoundHtm);
        }
        $("#divLoading").hide();
    },
    getReportGridHeight: function () {
        var defaultScrollHeight = "300";
        var tableBodyHeight = $('.grid-report tbody').height();
        if (tableBodyHeight <= defaultScrollHeight) {
            defaultScrollHeight = "";
        }
        return defaultScrollHeight;
    },

   

    adjustColumnDefs: function (arrayJSONColDefs, columnsCount) {
        for (var i = 1; i < columnsCount; i++) {
            // disable sorting for all columns except first column
            arrayJSONColDefs.push({ bSortable: false, targets: [i] });
        }
        return arrayJSONColDefs;
    },
}
