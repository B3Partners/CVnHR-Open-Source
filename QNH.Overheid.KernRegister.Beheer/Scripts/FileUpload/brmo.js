﻿
$(function () {
    'use strict';

    var url = 'Backload/UploadHandler?objectContext=brmo';
    // Initialize the jQuery File Upload widget:
    $('#fileupload').fileupload({
        // Uncomment the following to send cross-domain cookies:
        //xhrFields: {withCredentials: true},
        url: url,
        acceptFileTypes: /(csv)$/i
    });

    // Enable iframe cross-domain access via redirect option:
    $('#fileupload').fileupload(
        'option',
        'redirect',
        window.location.href.replace(
            /\/[^\/]*$/,
            '/cors/result.html?%s'
        )
    );

    // Load existing files by an initial ajax request to the server after page loads up
    // This is done by a simple jQuery ajax call, not by the FIle Upload plugin.,
    // but the results are passed to the plugin with the help of the context parameter: 
    // context: $('#fileupload')[0] and the $(this)... call in the done handler. 
    // With ajax.context you can pass a JQuery object to the event handler and use "this".
    $('#fileupload').addClass('fileupload-processing');
    $.ajax({
        // Uncomment the following to send cross-domain cookies:
        //xhrFields: {withCredentials: true},
        url: url,
        dataType: 'json',
        context: $('#fileupload')[0]
    }).always(function () {
        $(this).removeClass('fileupload-processing');
    }).done(function (result) {
        $(this).fileupload('option', 'done')
            .call(this, null, { result: result });
    });
});


$("document").ready(function () {
    if ($("#processType").val() === "Csv") {
        $(".brmo-config-area").hide();
        $(".brmo-config-area-csv").show();
    } else {
        $(".brmo-config-area-csv").hide();
        $(".brmo-config-area").show();
    }

    $("#processType").on('change', function () {
        if ($("#processType").val() === "Csv") {
            $(".brmo-config-area").hide();
            $(".brmo-config-area-csv").show();
        } else {
            $(".brmo-config-area-csv").hide();
            $(".brmo-config-area").show();
        }
    });

    $(".btn-save-config").on("click", function (btn) {
        var HRDataserviceVersion = $("#Config_HRDataserviceVersion").val();
        var BrmoProcessType = $("#processType").val();
        var PostCodes;
        if ($("#processType").val() === "Csv") {
            var rows = $("#files_uploaded>tr");
            PostCodes = "";
            for (var i = 0; i < rows.length; i++) {
                PostCodes += rows[i].children[1].innerText+" ";
            }
            PostCodes = PostCodes.replace(/(\r\n|\n|\r)/gm, "");
        }
        else {
            PostCodes = $("#Brmo-PostCodes").val();
        }
        $.post(window.location.href +"/SaveConfig", { PostCodes: PostCodes, HRDataserviceVersion: HRDataserviceVersion, BrmoProcessType: BrmoProcessType })
            .fail(function () {
                alert("Error!");
            });
    });
});