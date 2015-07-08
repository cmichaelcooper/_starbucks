try {
    $.validator.unobtrusive.parse(document);
    $.validator.defaults.highlight = function (element, errorClass, validClass) {
        $(element).closest(".grouping").addClass("has-error");
        $(".required-notice").addClass("has-error");
        $(".validation-message").addClass("has-error");
        $(".validationMessage").show();
    };

    $.validator.defaults.unhighlight = function (element, errorClass, validClass) {
        $(element).closest(".grouping").removeClass("has-error");
        $(".required-notice").removeClass("has-error");
        $(".validation-message").removeClass("has-error");
        $(".validationMessage").hide();
    };
} catch (err) {
    //console.log(err.message + $.validator + " error");
}