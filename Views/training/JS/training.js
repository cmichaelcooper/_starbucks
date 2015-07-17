TrainingFunctionality: function () {

    var viewportWidth = viewportSize.getWidth();
    var viewPort;
    if (viewportWidth > 1099) {
        var viewPort = 'desktop';
    }
    if (viewportWidth > 767 && viewportWidth < 1025) {
        var viewPort = 'tablet';
    }
    if (viewportWidth < 768) {
        var viewPort = 'mobile';
    }

    $('input:radio, input:checkbox').checkedPolyfill();
    $('input.topic-check').on('click touch', function () {
        $(this).toggleClass('checked');
        $(this).parent().toggleClass('selected');
    });

    $('input.quiz-answer-radio').on('click touch', function () {
        // $(this).toggleClass('checked');
    });

    $categoryTabs = $('.grid-tabs ul li');
    $categoryItems = $('#training-modules section .catalog-grid__item');

    function setActiveTab(selectedTab) {

        $categoryTabs.each(function () {
            $(this).removeClass('active');
            $(this).addClass('inactive');
        })
        $(selectedTab).removeClass('inactive').addClass('active');

    }

    function setActive() {
        var activeTab = $('.grid-tabs ul').find('li.active');
        var selectedCat = $(activeTab).attr('data-category');
        var selectedGroup = $('#training-modules section').find(".catalog-grid__item[data-category='" + selectedCat + "']");
        $('#training-modules section .catalog-grid__item').hide();
        $(selectedGroup).each(function () {
            $(this).show();
        });
    }

    function setInactive() {
        $('#training-modules section .catalog-grid__item').hide();
    }

    function setInactiveTabs() {
        $('.grid-tabs ul').find('li.inactive').addClass('mobile-hide');
    }

    function resetTabs() {
        $('.grid-tabs ul').find('li.active').removeClass('active').addClass('inactive');
        $('.grid-tabs ul').find('li.inactive.mobile-hide').removeClass('mobile-hide');
    }

    $categoryTabs.addClass('inactive').removeClass('active');
    $categoryItems.hide();

    if (viewPort != 'mobile') {
        setTimeout(function () {
            $categoryTabs.first().removeClass('inactive').addClass('active');
            setActive();
        }, 100);
    }

    $categoryTabs.on('click touch', function () {

        if ($(this).hasClass('active')) {
            if (viewPort == 'mobile') {
                resetTabs();
                setInactive();
            }
        } else {
            setActiveTab($(this));
            setActive();
            if (viewPort == 'mobile') {
                setInactiveTabs();
            }
        }
    });
}