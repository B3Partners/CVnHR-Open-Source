CURRENTVIEW_OBJECTCONTEXT = "mutaties";

$("document").ready(function () {
    var csvMutatieHub = $.connection.csvMutatieHub;

    $('#btnCloseProgress').click(function (event) {
        document.getElementById('procprogesssuccess').style.width = '0%';
        document.getElementById('procprogresserror').style.width = '0%';
        $('#kvknaam').text('');
        $('#processStats').text('0/0/0');
        $('#storageStats').text('0/0/0');
    });

    var names = [];
    csvMutatieHub.client.reportProgress = function (succesCount, errorCount,  perc, succesperc, inschrijvingnaam, totalNew, totalUpdated, totalAlreadyExisted) {
        if (inschrijvingnaam !== 'Klaar') {
            document.getElementById('procprogesssuccess').style.width = succesperc + '%';
            var errorperc = perc - succesperc;
            document.getElementById('procprogresserror').style.width = errorperc + '%';
            $('#kvknaam').text(inschrijvingnaam);
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

    $('#fileupload')
        .on('fileuploaddestroy', function (e, data) {
            // Event handler example. Do something if you need after file has been deleted on the server. 
            // (Refer to the client side documentatio).
        })
        .on('fileuploaddone', function (e, data) {

            var fileName = data.files[0].name;
            $('#mutatiedialog').on('shown.bs.modal', function (e) {
                // do something...
                $.connection.hub.start().done(function () {
                    var brmoChecked = $('#brmo-mutations').prop('checked');
                    var cvnhrChecked = $('#cvnhr-mutations').prop('checked');

                    //console.log(`filename: ${fileName}, brmo: ${brmoChecked}, cvnhr: ${cvnhrChecked}`);

                    csvMutatieHub.server.processCsv(fileName, brmoChecked, cvnhrChecked);
                });
           });

            $('#mutatiedialog').modal('show');
        });
});
