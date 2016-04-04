var tirHeirarchicalDetailReport = {
    displayPercentSymbol: function (show) {
        if (show == 'True') {
            $(".col-percentile").each(function () {
                var percentileValue = $(this).text();
                if (percentileValue != '') {
                    $(this).append('%');
                }
            });
        }
    },
    insertHeadersToReportGrid: function (headerRow, columnCount) {
        $(".tirDetailGrid").find("thead").prepend(headerRow);
        $(".tirDetailGrid").find("thead").prepend("<tr class='row-mergedcols'><td colspan='" + columnCount + "'></td></tr>");
        var ths = $(".tirDetailGrid").find(".row-report-columns").find("th");
        $(ths[0]).addClass("col-student").addClass("fixed-column"); //STUDENT
    },
    appendFooterToReportGrid: function (reportFooter) {
        $(".tirDetailGrid").append(reportFooter);
    },
    adjustColumnHeaderStyle: function () {
        var gridColumnRowIndexAccordingToStyle = 2;
        var headerCells = $(".tirDetailGrid tr:eq(" + gridColumnRowIndexAccordingToStyle + ") th");
        var firstRowCells = $(".tirDetailGrid tr:eq(" + (gridColumnRowIndexAccordingToStyle + 1) + ") td");
        $.each(firstRowCells, function (index, value) {
            var isImpactColHeader = $(firstRowCells[index]).hasClass('col-impact');
            var hasColScoreInner = $(firstRowCells[index]).hasClass('col-score-inner');
            var headerCol = $(headerCells[index]);
            headerCol.addClass($(firstRowCells[index]).attr("class"));
            if (isImpactColHeader) {
                headerCol.addClass('col-padding-right');
            }
            if (hasColScoreInner) {
                headerCol.removeClass('col-score-inner');
            }
        });
    },
    handleToggleViewScores: function (reportType, detailUrl) {
        $('#btnToggleValues').click(function () {
            if (reportType == 'MAP' || reportType == 'ISAT') {
                location.href = detailUrl;
            }
            else {
                throw 'Report type ' + reportType + ' not supported';
            }
        });
    },
    displayFotterRibbonArrows: function () {
        $('.footer-ribbon-arrow--1').attr('src', '../content/images/right_arrow_orange.png');
        $('.footer-ribbon-arrow-0').attr('src', '../content/images/right_arrow_blue.png');
        $('.footer-ribbon-arrow-1').attr('src', '../content/images/right_arrow_green.png');
        $('.footer-ribbon-arrow--2').attr('src', '../content/images/right_arrow_light-red.png');
        $('.footer-ribbon-arrow-').attr('src', '../content/images/right_arrow_blue.png');
        $('.footer-ribbon-arrow-default').attr('src', '../content/images/right_arrow_light-gray.png');
    },
    uploadHeirarachicalReportOnTermChange: function (detailUrl, assessmentTypeId) {
        $('#SchoolTermList').change(function () {
            var termId = $('#SchoolTermList').val();
            location.href = detailUrl + "&AssessmentTypeId=" + assessmentTypeId + "&InputTermId=" + termId;
        });
    },
    uploadHeirarachicalReportOnNextPrevButtonClick: function (detailUrl, prevlinkStatus, nextlinkStatus, assessmentTypeId) {
        $('#lnkNext').click(function (e) {
            if (nextlinkStatus == "link-disable") {
                e.preventDefault();
                return false;
            }
            var termId = $('#SchoolTermList').val();
            var horizontalPageIndex = 1;
            location.href = detailUrl + "&AssessmentTypeId=" + assessmentTypeId + "&InputTermId=" + termId + "&HorizontalPageIndex=" + horizontalPageIndex;
        });
        $('#lnkPrev').click(function (e) {
            if (prevlinkStatus == "link-disable") {
                e.preventDefault();
                return false;
            }
            var termId = $('#SchoolTermList').val();
            var horizontalPageIndex = 0;
            location.href = detailUrl + "&AssessmentTypeId=" + assessmentTypeId + "&InputTermId=" + termId + "&HorizontalPageIndex=" + horizontalPageIndex;
        });
    },
    getSchoolTerms: function (subjectId, schoolYearId, assessmentTypeId) {
        $.ajax({
            url: "/HeirarchicalTeacherDetailReport/GetSchoolTerms",
            data: { subjectId: subjectId, schoolYearId: schoolYearId, assessmentTypeId: assessmentTypeId },
            type: "POST",
            datatype: "json"
        })
            .then(
            function (response) {
                $('#SchoolTermList').empty();
                $.each(response, function (key, ddlData) {
                    $('select#SchoolTermList').append(
                            '<option value="' + ddlData.Value + '">'
                            + ddlData.Text +
                            '</option>');
                })
            },
            function (xhr, status) {
                alert("error: " + xhr.statusText);
            });
    },
    viewTemplateAttachment: function (sloFileName, rubricFileName) {
        $('#linkSLO').click(function () {
            $("#assessmentTempIframe").attr("src", "/HeirarchicalTeacherDetailReport/ViewPDF?fileName=" + sloFileName);
            $("#modalAssessmentTemplate").modal("show");
        });
        $('#linkRubric').click(function () {
            $("#assessmentTempIframe").attr("src", "/HeirarchicalTeacherDetailReport/ViewPDF?fileName=" + rubricFileName);
            $('#modalAssessmentTemplate').modal('show');
        });
    },
    fixHeaderAndLeftColumn: function (columnCount) {
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
            this.adjustReportHeaderWidth();
        }
    },
    getReportGridHeight: function () {
        var defaultScrollHeight = "300";
        var tableBodyHeight = $('.grid-report tbody').height();
        if (tableBodyHeight <= defaultScrollHeight) {
            defaultScrollHeight = "auto";
        }
        return defaultScrollHeight;
    },
    adjustReportHeaderWidth: function () {
        var containerWidth = $(".container").width();
        var gridReportWidth = $(".grid-report").width();
        if (gridReportWidth < containerWidth) {
            $(".report-header").css("width", gridReportWidth);
        }
    },
    // applying data type for score, target and M/E column to 'numeric-null'. 
    // This will hanldle numeric as well as null data because by default it use string.
    adjustColumnDefs: function (arrayJSONColDefs, columnsCount) {
        arrayJSONColDefs.push({ type: 'html', "targets": [0] });
        for (var i = 1; i < columnsCount; i++) {
            arrayJSONColDefs.push({ type: 'numeric-null', targets: [i] });
        }
        return arrayJSONColDefs;
    },
}

