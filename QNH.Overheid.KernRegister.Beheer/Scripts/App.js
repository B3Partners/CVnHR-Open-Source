/* App.js */

$(function() {
    $(".btn-showinvalid-vestigingen").on("click", function (e) {
        e.preventDefault();
        $("table.vestiging tr[data-invalid]").toggle();
    });

    $(".log-files").on("click", "a", function (e) {
        e.preventDefault();
        window.open($(this).attr("data-base-url") + "?fileName=" + $(this).attr("data-href"), "_blank");
    });
});
