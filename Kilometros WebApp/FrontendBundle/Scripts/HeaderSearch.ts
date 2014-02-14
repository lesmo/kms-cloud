$(document).ready(() => {
    $("header > form > input").autocomplete({
        minLength: 3,
        source: (request, response) => {
            return [];
        }
    });
});