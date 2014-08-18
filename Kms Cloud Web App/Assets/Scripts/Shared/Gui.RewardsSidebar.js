$(function() {
    $("#recompensaDashReciente .botonDorado").click(function(e) {
        var $parent = $(this).parent();
        $parent.slideUp().siblings().slideDown();

        if ($parent.data("discarded") != "1") {
            $.get(
                getKMS_ajaxUri("DiscardReward.json"),
                { id: $parent.data("id") }
            ).fail(function () {
                $parent.data("discarded", "0");
                console.log("[!] - Error: no fue posible descartar Recompensa con ID [" + $parent.data("id") + "]");
            });

            $parent.data("discarded", "1");
        }

        e.preventDefault();
        return false;
    });
    $("#recompensaDashSiguiente .botonDorado").click(function (e) {
        $(this).parent().slideUp().siblings().slideDown();
        e.preventDefault();
        return false;
    });
});