CURRENTVIEW_OBJECTCONTEXT = "brmo";

$("document").ready(function () {
    $("#processType").val("Csv");

    var name = getParameterByName("name");
    $("#Task-name").val(name.substr(name.indexOf(' ') + 1));

    $("#processType").on('change', function () {
        if ($("#processType").val() === "Csv") {
            $(".brmo-config-area").hide();
            $(".brmo-config-area-csv").show();
        } else {
            $(".brmo-config-area-csv").hide();
            $(".brmo-config-area").show();
        }
    });

    function getCsvRows() {
        var rows = $("#files_uploaded>tr");
        var names = $("#Brmo-PostCodes").val().split(" ");
        for (var i = 0; i < rows.length; i++) {
            for (var j = 0; j < names.length; j++) {
                if (names[j] === rows[i].children[1].innerText.replace(/(\r\n|\n|\r)/gm, "").trim()) {
                    $(rows[i].children[3]).find("input").prop("checked", true);
                }
            }
        }
        clearInterval(inter);
    };

    function getParameterByName(name, url) {
        if (!url) url = window.location.href;
        name = name.replace(/[\[\]]/g, '\\$&');
        var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
            results = regex.exec(url);
        if (!results) return null;
        if (!results[2]) return '';
        return decodeURIComponent(results[2].replace(/\+/g, ' '));
    };

    if ($("#processType").val() === "Csv") {
        $(".brmo-config-area").hide();
        $(".brmo-config-area-csv").show();
    } else {
        $(".brmo-config-area-csv").hide();
        $(".brmo-config-area").show();
    }

    $(".btn-save-config").on("click", function (btn) {
        var HRDataserviceVersion = $("#Config_HRDataserviceVersion").val();
        var BrmoProcessType = $("#processType").val();
        var PostCodes;
        var taskName = $("#Task-name").val();
        if ($("#processType").val() === "Csv") {
            var rows = $("#files_uploaded>tr");
            PostCodes = "";
            var csvCount = 0;
            for (var i = 0; i < rows.length; i++) {
                if ($(rows[i].children[3]).find("input").is(":checked")) {
                    PostCodes += rows[i].children[1].innerText.trim();
                    csvCount++;
                }
            }
            if (csvCount > 1) {
                alert("Er kan maar één CSV bestand per taak geselecteerd worden.");
                return;
            } else if (csvCount > 1) { 
                alert("Geen CSV bestand geselecteerd. Selecteer het te gebruiken CSV bestand d.m.v. de checkbox naast het CSV bestand.");
            } else if (PostCodes.indexOf(" ") != -1) {
                alert("CSV bestandsnamen mogen geen spaties bevatten");
                return;
            }
            PostCodes = PostCodes.replace(/(\r\n|\n|\r)/gm, "");
        }
        else {
            PostCodes = $("#Brmo-PostCodes").val();
        }
        $.post(window.location.href.split('?')[0] + "/SaveConfig", { PostCodes: PostCodes, taskName: taskName, HRDataserviceVersion: HRDataserviceVersion, BrmoProcessType: BrmoProcessType })
            .done(function () {
                location.replace(window.location.origin + window.location.pathname + "?name=CVnHR " + taskName);
                alert("Taak is opgeslagen");
            })
            .fail(function () {
                alert("Error!");
            });
    });

    var inter = setInterval(getCsvRows, 1000);
});