var knowledgeBase = {

    backButtonClick: function () {
        $("#btnBack").click(function () {
            window.location.href = "/KnowledgeBase/Index";
        });
    },

    displayExtensionIcons: function () {
        $('.attached-file-ext-docx').attr('src', '../content/icons/doc.ico');
        $('.attached-file-ext-pdf').attr('src', '../content/icons/pdf.ico');
        $('.attached-file-ext-xlsx').attr('src', '../content/icons/excel.ico');
        $('.attached-file-ext-').attr('src', '');
    }
}