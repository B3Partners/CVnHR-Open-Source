/* App.js */

$(function() {
    $(".btn-showinvalid-vestigingen").on("click", function (e) {
        e.preventDefault();
        $("table.vestiging tr[data-invalid]").toggle();
    });
});
