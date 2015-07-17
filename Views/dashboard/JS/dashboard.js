//Masonry setup for dashboards  
if (viewportWidth > 640) {
    var $container = $('.dashboard-content');
    // initialize Masonry after all images have loaded  
    $container.imagesLoaded(function () {
        $container.masonry();
        $container.masonry({
            itemSelector: '.dashboard-banner',
            columnWidth: function (containerWidth) {
                return containerWidth / 4;

            },
        });
    });
    $('.dashboard-content').masonry({
        itemSelector: '.dashboard-banner',
        columnWidth: function (containerWidth) {
            return containerWidth / 4;

        },
    });
}

// Dashboard / Profile account view switcher?
$('#SelectedAccount').on('change', function () {
    $(this).parent().submit();
});