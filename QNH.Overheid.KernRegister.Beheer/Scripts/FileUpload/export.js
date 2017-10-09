/*
 * jQuery File Upload Plugin JS Example 8.0.1
 * https://github.com/blueimp/jQuery-File-Upload
 *
 * Copyright 2010, Sebastian Tschan
 * https://blueimp.net
 *
 * Licensed under the MIT license:
 * http://www.opensource.org/licenses/MIT
 */

/*jslint nomen: true, regexp: true */
/*global $, window, navigator */

$(function () {
    'use strict';

    var url = 'Backload/UploadHandler?objectContext=export';

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
    
    var csvExportHub = $.connection.csvExportHub;

    $('#btnCloseProgress').click(function (event) {
        document.getElementById('procprogesssuccess').style.width = '0%';
        document.getElementById('procprogresserror').style.width = '0%';
        $('#kvknaam').text('');
        $('#processStats').text('0/0/0');
        $('#storageStats').text('0/0/0');
    });

    var names = [];
    csvExportHub.client.reportProgress = function (succesCount, errorCount, perc, succesperc, inschrijvingnaam, totalNew, totalUpdated, totalAlreadyExisted) {
        if (inschrijvingnaam != 'Klaar') {
            document.getElementById('procprogesssuccess').style.width = succesperc + '%';
            var errorperc = perc - succesperc;
            document.getElementById('procprogresserror').style.width = errorperc + '%';
            $('#kvknaam').html(inschrijvingnaam);
            names.push(inschrijvingnaam);
            var totalCount = succesCount + errorCount;
            $('#processStats').text(errorCount + '/' + succesCount + '/' + totalCount);
            $('#storageStats').text(totalNew + '/' + totalUpdated + '/' + totalAlreadyExisted);
        }
        else {
            var details = $("<a href='#'>Toon details</a>").on("click", function (e) {
                e.preventDefault();
                $(".kvk-details").toggle();
            });

            $('#kvknaam').html('Klaar met verwerken. ').append(details);

            $(".kvk-details").remove();
            $('.modal-body').append("<div class='row kvk-details' style='display:none;'></div>");
            while (names.length > 0) {
                $(".kvk-details").append(names.pop() + "<br/>");
            }

            $.connection.hub.stop();
        }
    };

    csvExportHub.client.reportItemsToInsert = function (itemsToInsert) {
        $(".rowitemstoinsert").remove();
        var row = $("<div>").addClass("row").addClass("rowitemstoinsert").append("<div class='col-sm-4'>Nieuwe kvkNummers</div><div class='itemstoinsert col-sm-8'></div>");
        $(".modal-body").append(row);
        $(itemsToInsert).each(function () {
            $(".itemstoinsert", row).append(this + "<br/>");
        });
        $(".itemstoinsert").append($("<button>").html("Toevoegen aan DocBase").on("click", function () {
            $.connection.hub.start().done(function () {
                csvExportHub.server.insertItems(itemsToInsert);
                $(".rowitemstoinsert").remove();
            });
        }));
    };

    $('#fileupload')
        .bind('fileuploaddestroy', function (e, data) {
            // Event handler example. Do something if you need after file has been deleted on the server. 
            // (Refer to the client side documentatio).
        })
        .bind('fileuploaddone', function (e, data) {
            // alert('We zijn klaar. Dus start met processen!');
            var fileName = data.files[0].name;
            $('#exportfromkvk').on('shown.bs.modal', function (e) {
                // do something...
                $.connection.hub.start().done(function () {
                    csvExportHub.server.exportCsv(fileName);
                });
                
           });

            $('#exportfromkvk').modal({
                show: true
            });
        });
});
