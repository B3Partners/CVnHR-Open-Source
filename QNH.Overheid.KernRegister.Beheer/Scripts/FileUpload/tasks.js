CURRENTVIEW_OBJECTCONTEXT = "tasks";

$("document").ready(function () {
    $('#fileupload')
        .on('fileuploaddestroy', function (e, data) {
            // Event handler example. Do something if you need after file has been deleted on the server. 
            // (Refer to the client side documentatio).
            window.alert('File deleted! ?????');
            console.log(data);
        })
        .on('fileuploaddone', function (e, data) {

            var includeBrmo = $("#brmo-mutations").val();
            var includeCvnhr = $("#cvnhr-mutations").val();

            // alert('We zijn klaar. Dus start met processen!');
            var fileName = data.files[0].name;

            window.alert("TODO! " + fileName + ' include: ' + includeBrmo + includeCvnhr);
        });
});