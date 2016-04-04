var tirDetailReport = {
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
        $(ths[0]).addClass("col-id").addClass("fixed-column-ID"); //ID
        $(ths[1]).addClass("col-student").addClass("fixed-column"); //STUDENT
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

    viewTemplateAttachment: function (sloFileName, rubricFileName) {
        $('#linkSLO').click(function () {
            $("#assessmentTempIframe").attr("src", "/TeacherDetailReport/ViewPDF?fileName=" + sloFileName);
            $("#modalAssessmentTemplate").modal("show");
        });
        $('#linkRubric').click(function () {
            $("#assessmentTempIframe").attr("src", "/TeacherDetailReport/ViewPDF?fileName=" + rubricFileName);
            $('#modalAssessmentTemplate').modal('show');
        });
    }
}
