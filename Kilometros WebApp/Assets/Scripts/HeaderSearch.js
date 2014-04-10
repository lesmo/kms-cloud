$(document).ready(function () {
    $("header > form > input").autocomplete({
        minLength: 3,
        source: function (request, response) {
            return [];
        }
    });
});
//# sourceMappingURL=HeaderSearch.js.map
