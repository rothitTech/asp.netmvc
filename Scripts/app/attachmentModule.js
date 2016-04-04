var counter = 1;
var attachmentModule = {

    addFiles: function () {
        $("#lnkAddFiles").click(function () {
            $("#tblKnowledgeBase").append("<tr valign='top'><td></td><td><input type='file' class='form-control' name='Files' id='cntFile_" + counter + "' placeholder='pdf, docx and xlxs files only'/><a class='removeCF' href='javascript:void(0);' style='margin-left:10px;'>Remove</a></td></tr>");
            counter++;
            return false;
            e.preventDefault();
        });
    },
    removeFiles: function () {
        $("#tblKnowledgeBase").on("click", ".removeCF", function () {
            if (counter > 1) {
                $(this).parent().parent().remove();
                counter--;
            }
        });
    },
    backButtonClick: function () {
        $("#btnBack").click(function () {
            window.location.href = "/KnowledgeBase/Index";
        });
    },
    initializeTinyMCE: function () {
        tinyMCE.init({
            // General options
            mode: "textareas",
            theme: "modern",
            height: 200,
            plugins: [
            "advlist autolink lists link image charmap print preview hr anchor pagebreak",
            "searchreplace wordcount visualblocks visualchars code fullscreen",
            "insertdatetime media nonbreaking save table contextmenu directionality",
            "emoticons template paste textcolor colorpicker textpattern"
            ],
            toolbar1: "insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image",
            //toolbar2: "print preview media | forecolor backcolor emoticons",
            image_advtab: true,
            templates: [
                { title: 'Test template 1', content: 'Test 1' },
                { title: 'Test template 2', content: 'Test 2' }
            ],
            // Example content CSS (should be your site CSS)
            content_css: "~/Content/Site.css",
        });
    }
}