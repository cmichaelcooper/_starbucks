$(document).ready(function () {
    SBS.Begin();
});

SBS = {
    Superscript: function() {
        $('body :not(script)').contents().filter(function () {
            return this.nodeType === 3;
        }).replaceWith(function () {
            return this.nodeValue.replace(/[™®©]/g, '<sup>$&</sup>');
        });
    },

    TrackEvent: function (category, action, optional_label, optional_value, optional_non_interaction) {
        //_gaq.push(['_trackEvent', category, action, optional_label, optional_value, optional_non_interaction]);
        ga('send', 'event', category, action, optional_label, optional_value);
    },
    
    Begin: function () {
        $(".ttc-select").select2({
            minimumResultsForSearch: -1
        });


        //plugin function, place inside DOM ready function
        outdatedBrowser({
            bgColor: '#f25648',
            color: '#ffffff',
            lowerThan: 'transform',
            languagePath: '/outdatedbrowser/lang/en.html'
        });


        // Set variables
        $siteContent = $('#content');
        $siteSwitcher = $('#site-switcher');
        $siteNav = $('#site-navigation');
        $navToggle = $('.nav-toggle');

        var viewportWidth = viewportSize.getWidth(),
                viewportHeight = viewportSize.getHeight();


        // Set navigation height
        setTimeout(function () {
            if (viewportWidth < 1101) {
                viewportHeight = viewportSize.getHeight();
            } else {
                viewportHeight = $siteContent.outerHeight(false);
            }
            $siteNav.height(viewportHeight);


            // automatically superscript all tm, reg and copyright symbols
            $('body :not(script, h2.ng-binding)').contents().filter(function () {
                return this.nodeType === 3;
            }).replaceWith(function () {
                return this.nodeValue.replace(/[™®©]/g, '<sup>$&</sup>');
            });
        }, 500);

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
        // Disable overscroll / viewport moving on everything but scrollable divs
        $('body').on('touchmove', function (e) {
            if (!$('.scrollable').has($(e.target)).length) e.preventDefault();
        });


        // Functions to run when the window is resized
        $(window).smartresize(function () {
            var viewportWidth = viewportSize.getWidth();

            resizeKidGridDescription();
            setFontana();

            setTimeout(function () {
                if (viewportWidth < 768) {
                    viewportHeight = viewportSize.getHeight();
                } else {
                    viewportHeight = $siteContent.outerHeight(false);
                }
                $siteNav.height(viewportHeight);
            }, 500);
        });


        // Add these element's classes or css when the navigation container is open
        function openMenu() {
            $('body').addClass('menu-open');
            $siteNav.addClass('is-open');
            $siteSwitcher.addClass('menu-open');
        }

        // Remove these elements's classes or css when the navigation container is closed
        function closeMenu() {
            $('body').removeClass('menu-open');
            $siteSwitcher.removeClass('menu-open');
            $siteNav.removeClass('is-open');
        }

        //});
        // Resize kid-grid description to accomodate tablet breakpoints
        setTimeout(function () {
            resizeKidGridDescription();
        }, 100);
        function resizeKidGridDescription() {
            var viewportWidth = viewportSize.getWidth();

            if (viewportWidth > 767) {
                var kidGridItem = $('.module-kid-grid li .inner-wrapper').width(),
                        kidGridLogo = $('.module-kid-grid li .inner-wrapper .logo-wrapper').width() + 20,
                        kidGridDescription = $('.module-kid-grid li .inner-wrapper .description-wrapper'),
                        kidGridButton = $('.module-kid-grid li .inner-wrapper > a').width() + 20,
                        targetWidth;

                targetWidth = (kidGridItem - (kidGridLogo + kidGridButton)) - 1;

                kidGridDescription.css('width', targetWidth);
            }
        }

        $('#selectlocale').on('click touch', function () {
            if (viewportWidth < 768) {
                $('#countryselect_mobile').toggle();
            }
            else {
                $('#countryselect').toggle();
            }
        });

        // Open the navigation container
        $navToggle.on('click touch', function () {
            if ($siteNav.hasClass('is-open')) {
                closeMenu();
            }
            else {
                openMenu();
            }
        });


        // On desktop: if navigation container is open, and content area is clicked, close the navigation
        $(document).mouseup(function (e) {
            var container = $("#site-navigation");
            var secondcontainer = $(".toggle");

            if ($siteNav.hasClass('is-open') && (!container.is(e.target) && container.has(e.target).length === 0) && (!secondcontainer.is(e.target) && secondcontainer.has(e.target).length === 0)) {
                closeMenu();
            }

        });


        // In mobile devices: if navigation container is open, content area is touched, close the navigation
        $(document).on({
            'touchstart': function (e) {
                var container = $("#site-navigation");
                var secondcontainer = $(".toggle");

                if ($siteNav.hasClass('is-open') && (!container.is(e.target) && container.has(e.target).length === 0) && (!secondcontainer.is(e.target) && secondcontainer.has(e.target).length === 0)) {
                    closeMenu();
                }
            }
        });

        $('#nav-sign-in').on('click touch', function () {
            closeMenu();
        })


        // Toggle .collapser element when .expander is clicked or touched
        $('.expander').unbind('click touch').bind('click touch', function () {
            $collapser = $(this).next('.collapser');
            $collapser.slideToggle(250);
            $(this).toggleClass('active');
        });


        // Toggle accordion menus within the navigation container
        $menuToggle = $('.main-nav li a.dropdown-toggle');
        $subMenu = $menuToggle.next('div.dropdown');
        $subMenu.hide();
        if (is_touch_device && viewportWidth < 1025) {
            $('a.dropdown-toggle').on("click touch", function () {
                var $this = $(this);
                $('.dropdown.current').hide();
                var toggleButton = $this.attr('id');
                var toggleSub = toggleButton.replace('toggle-', 'dropdown-');
                $('#' + toggleSub).show().addClass('current');
                return false;
            });
        } else {
            $menuToggle.hoverIntent(expandNavigation);
        }

        function expandNavigation() {
            if (!$(this).next($subMenu).is(':visible')) {
                $subMenu.slideUp(500);
                $(this).next($subMenu).slideDown(500);
            }
        }
        function collapseNavigation() {
            if ($(this).next($subMenu).is(':visible')) {
                $(this).next($subMenu).slideUp(500);
            }
        }

        function is_touch_device() {
            return !!('ontouchstart' in window);
        }

        // Hopefully add :checked support for IE8
        $('input:radio, input:checkbox').checkedPolyfill();
        $('input#RememberMe').on('click', function () {
            $('#RememberMe').toggleClass('checked');
        });



        // CSS changes to the Fontana add-on in tablet orientations
        setFontana();
        function setFontana() {
            var viewportWidth = viewportSize.getWidth();
            if (viewportWidth < 1024) {
                $('.fontana-right').hide();
                $('.fontana-left').css('width', '100%');
                $('.fontana-left img').css({ 'display': 'block', 'margin': '0 auto' });
                $('.fontana-left').css({ 'text-align': 'center' });
                $('.fontana-left a').css({ 'float': 'none', 'margin-right': '0' });
            }
            if (viewportWidth > 1023) {
                $('.fontana-right').show();
                $('.fontana-left').css('width', '60%');
                $('.fontana-left img').css({ 'display': 'block', 'margin': '0' });
                $('.fontana-left').css({ 'text-align': 'left' });
                $('.fontana-left a').css({ 'float': 'left' });
            }
        }


        // Dashboard / Profile account view switcher?
        $('#SelectedAccount').on('change', function () {
            $(this).parent().submit();
        });

        if (viewportWidth > 1100) {
            $('#sbc-blended #site-wrapper #content .body-content .bottom-edger').css('min-height', ($(window).innerHeight() * 0.25));
        }

        $('.become-customer, .std-leadform').on('click touch', function () {

            _kmq.push(['record', 'Clicked become a customer', { 'Customer form': document.URL }]);
        });

        $("#State").on('input', function () {
            this.value = this.value.replace(/[^a-zA-Z]/g, '');
        });
    },

    Profile: function () {

        var windowWidth = viewportSize.getWidth(),
                thisId,
                numberOfAccounts,
                heightValue = 167;

        $('.indicate-required').hide();
        $('.required-notice').hide();
        $('.edit-profile-btn').on('click', function () {
            $('.indicate-required').show();
            $('.required-notice').show();
        });
        $('.cancel').on('click', function () {
            $('.indicate-required').hide();
            $('.required-notice').hide();
        });


        //.show-button triggers all items to be shown (remove inactive class, add active class) and only hide the current show-button
        $(".show-button").on("click touch", function () {
            var id = $(this).attr("id"),
                submitId = id.replace("action", "button"),
                cancelId = id.replace("action", "cancel"),
                sectionId = id.replace("action", "section");

            $("#" + id).removeClass("active").addClass("inactive");
            $("#" + submitId + ", #" + cancelId + ", #" + sectionId).each(function () {
                $(this).removeClass("inactive").addClass("active");
            });
        });

        //.cancel removes the active class that was added from .show-button and adds the inactive class to hide the items
        $('.cancel').on('click touch', function () {
            var id = $(this).attr('id'),
                sectionId = id.replace("cancel", "section"),
                actionId = id.replace("cancel", "action"),
                buttonId = id.replace("cancel", "button");
            $('#' + actionId).removeClass('inactive');
            $('#' + id).removeClass('active').addClass('inactive');
            $('#' + sectionId + ', #' + sectionId + ', #' + buttonId).each(function () {
                $(this).removeClass('active').addClass('inactive');
            });

        });

        $('.edit-nickname-btn').on('click', function(){
            var selectedButton = $(this);
            toggleNicknameButtons(selectedButton);
        });

        $('.nickname-input').focusin( function(){
            var selectedButton = $(this).next('.edit-nickname-btn');
            toggleNicknameButtons(selectedButton);
        });

        $('.cancel-nickname-btn').on('click', function(){
            var editBtn = $(this).parent().find('.edit-nickname-btn'),
                saveBtn = $(this).parent().find('.save-nickname-btn'),
                nickNameInput = $(this).parent().find('.nickname-input');
            saveBtn.hide();
            $(this).hide();
            editBtn.show();
            $(nickNameInput).removeClass('visible');
        });

       function toggleNicknameButtons(selectedButton) {
            var saveBtn = $(selectedButton).next('.save-nickname-btn'),
                cancelBtn = $(selectedButton).parent().find('.cancel-nickname-btn'),
                nickNameInput = $(selectedButton).prev('.nickname-input');
            if( saveBtn.is(':visible') ) {
                saveBtn.hide();
                cancelBtn.hide();
                selectedButton.show();
                $(nickNameInput).removeClass('visible');
            } else {
                saveBtn.show();
                cancelBtn.show();
                selectedButton.hide();
                $(nickNameInput).addClass('visible');
            }
        }

        //Each user can have a variable number of accounts tied to their account
        //Adjust the height of the account heading based on the number of accounts (assigned in the id after the last dash(-)
        $('.account-listing').each(function () {
            thisId = $(this).attr('id');
            numberOfAccounts = thisId.substring(thisId.lastIndexOf("-") + 1, thisId.length);
            $(this).css('height', heightValue * numberOfAccounts);
        });

        $('.profile-input').bind('focus', function () {
            $('#edit-profile-action').trigger('click');
        });

        $('form').submit(function () {

            if (!$('#Password').val()) {
                $('.grouping').removeClass('valid');
            }

            //$('button').attr('disabled', 'disabled');
        });
    },

    PasswordStrength: function () {
        if (document.URL.indexOf('profile') > 0) {
            $('#NewPassword').keyup(function () {
                $('#strengthIndicator').html(checkStrength($('#NewPassword').val()));
            });
        } else {
            $('#Password').keyup(function () {
                $('#strengthIndicator').html(checkStrength($('#Password').val()));
            });
        }


        /*
     * 
    */
        function checkStrength(password) {
            //initial strength
            var strength = 0

            //if length is 8 characters or more, increase strength value
            if (password.length > 7 && password.match(/([a-z].*[A-Z])|([A-Z].*[a-z])/)) strength += 1;

            //if password contains both lower and uppercase characters, increase strength value
            //if (password.match(/([a-z].*[A-Z])|([A-Z].*[a-z])/)) strength += 1

            //if it has numbers and characters, increase strength value
            if (password.match(/([a-zA-Z])/) && password.match(/([0-9])/)) strength += 1;

            //if it has one special character, increase strength value
            if (password.match(/([!,%,&,@,#,$,^,*,?,_,~])/)) strength += 1;

            //if it has two special characters, increase strength value
            if (password.match(/(.*[!,%,&,@,#,$,^,*,?,_,~].*[!,%,&,@,#,$,^,*,?,_,~])/)) strength += 1;

            //Three repeating characters && length <= 8
            if (password.match(/(.)\1\1/) && password.length <= 8) {
                strength -= 1;
            }
            //now we have calculated strength value, we can return messages
            if (strength < 2) {
                $('#strengthIndicator').removeClass();
                $('#strengthIndicator').addClass('weak');
                return 'Weak';
            }
            else if (strength == 2) {
                $('#strengthIndicator').removeClass();
                $('#strengthIndicator').addClass('good');
                return 'Good';
            }
            else {
                $('#strengthIndicator').removeClass();
                $('#strengthIndicator').addClass('strong');
                return 'Strong';
            }
        }
    },

    Register: function () {
        $("#registrationForm input:text, #registrationForm textarea").first().focus();
        $('form').submit(function () {
            setTimeout(function () {
                SBS.LoadingButton();
            }, 100);

        });
    },

    LoadingButton: function () {
        if (!$(".grouping").hasClass('has-error') && $.trim($('.field-validation-error').html()).length < 1) {
            $('.register-buttons').addClass('active');
            $(':submit').attr('disabled', 'disabled');


        }
    },

    RegisterFour: function () {

        var windowWidth = viewportSize.getWidth();

        // In mobile devices: On register step 4, hide continue button when keyboard is active
        if (windowWidth < 767) {
            $(document).on('focus', 'input, textarea', function () {
                $(".register-buttons").hide();
            });

            $(document).on('blur', 'input, textarea', function () {
                $(".register-buttons").show();
            });
        }
    },

    VideoModal: function () {
        $('#videoModal').foundation('reveal', {

            opened: function () {

                var iframe = $('#video-player')[0],
                player = $f(iframe);
                player.addEvent('ready', function () {
                    player.api("play");
                    player.addEvent('finish', onFinish);
                });

            },
            closed: function () {
                var iframe = $('#video-player')[0],
                player = $f(iframe);
                player.addEvent('ready', function () {
                    player.api("pause");
                });
            }

        });

        function onFinish() {
            var cookieNameValue = "home-video-play";
            if (document.URL.indexOf('register') > 0) {
                cookieNameValue = "register-video-play";
            }
            SBS.CreateCookie(cookieNameValue, 1, 365);
            $('#videoModal').foundation('reveal', 'close');
        }

    },

    CreateCookie: function (name, value, days) {
        if (days) {
            var date = new Date();
            date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
            var expires = "; expires=" + date.toGMTString();
        }
        else var expires = "";

        var fullDomain = location.hostname;
        var partsDomain = fullDomain.split('.');
        if (partsDomain[0] == 'en' || partsDomain[0] == 'fr' || partsDomain[0] == 'ca-en') {
            var domain = partsDomain.slice(1).join('.');
            document.cookie = name + "=" + value + expires + "; domain=" + domain + "; path=/";
        }
        else document.cookie = name + "=" + value + expires + "; path=/";

    },

    ReadCookie: function (name) {
        var nameEQ = name + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') c = c.substring(1, c.length);
            if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
        }
        return null;
    },
    
    ProductList: function () {
        var document_width = $(document).width();
        var document_height = $(document).height();

        $("#site-wrapper").on("click", ".product-dropdown", function () {
            $(this).next('select').trigger('click');
        }).on("click", ".quick-add", function () {
            var el = $(this);

            var mobile = $(window).width() <= 464;
            var tablet = $(window).width() <= 1085 && $(window).width() > 464;

            if (el.hasClass('active')) {
                $('.quick-add-pushdown').removeClass('quick-add-pushdown').removeAttr('style');
                el.removeClass('active');
                $('.quick-add-dialog').slideUp(200, function () {
                    $('#site-wrapper').append($('.quick-add-dialog').removeAttr('style'));
                });
            }
            else {
                var currentQuickAddOffsetTop = 0;

                $('.quick-add').removeClass('active').parent().removeClass('active');
                el.addClass('active');

                var product = el.parents('.product-v2');

                var index = product.index();

                var rowSize = 4;
                if (mobile) {
                    rowSize = 1;
                }
                else if (tablet) {
                    rowSize = 3;
                }

                $('.quick-add-pushdown').removeClass('quick-add-pushdown').removeAttr('style');

                if (mobile) {
                    product.parent().find('.product-v2').eq(index).after($('.quick-add-dialog').slideDown(200, function () {
                        $('html, body').animate({ scrollTop: $('.quick-add-dialog').offset().top - 180 }, 600);
                    }));
                }
                else {
                    var p = product.parent().find('.product-v2');
                    index += (rowSize - 1) - (index % rowSize);
                    while (index >= p.length) {
                        index--;
                    }
                    p.eq(index).addClass('quick-add-pushdown');

                    var cellOffsetTop = el.offset().top;
                    $('.quick-add-dialog').css('top', (cellOffsetTop + el.outerHeight(true, true)) + 'px').slideDown(200, function () {
                        $('.quick-add-pushdown').css('margin-bottom', $('.quick-add-dialog').outerHeight() + 'px');
                        if (currentQuickAddOffsetTop != cellOffsetTop) {
                            //$('html, body').animate({ scrollTop: $('.quick-add-dialog').offset().top - 180 }, 600);
                            currentQuickAddOffsetTop = cellOffsetTop;
                        }
                    });
                }

                $(".quick-add-image").removeClass("clip").find("img").on("load", function () {
                    var el = $(this);
                    if (el.height() > 640) {
                        el.parent().addClass("clip");
                    }
                    el.hide();
                });

                $('.product-dropdown').each(function () {
                    var el = $(this);
                    el.find("span").text(el.data("default"));
                });

                $('.add-to-bag [name=qty]').val('');
                $('.add-to-bag [type=submit]').text('ADD TO BAG');
            }
        }).on("change", ".product-dropdown select", function () {
            var el = $(this);
            el.parent().find('span').text(el.val());

            if (/formSelect/.test(el.parent().attr("id"))) {
                $('#formSelect ~ .product-dropdown').each(function () {
                    var ddl = $(this);
                    ddl.find("span").text(ddl.data("default"));
                });

                $('#formSelect ~ .product-dropdown select').each(function () {
                    $(this).val("").find("option:selected").removeAttr("selected");
                });
            }
        }).on("click", '.filter input', function () {
            $('.quick-add.active').click();
            $('#site-wrapper').append($('.quick-add-dialog').removeAttr('style'));
        }).on("click", '.add-to-bag .btn', function () {
            SBS.AddItemToBag($(this));
        }).on("change", '#quantity, #-qty', function () {
            var el = $(this);
            if (el.val() != "")
                el.css("color", "#000")
        });

        $("#filter-toggle").on("click", function () {
            $('.quick-add.active').click();
            var $this = $(this);
            $this.toggleClass('active');
            if ($this.hasClass('active')) {
                $this.text("Hide Filters");
            }
            else {
                $this.text("Show Filters");
            }
            $('#filters-holder').slideToggle(200);
        }).click();

        $(window).on('resize', function () {
            if (document_width != $(document).width() || document_height != $(document).height()) {
                document_width = $(document).width(); document_height = $(document).height();
                $('.quick-add.active').click();
            }

            var el = $('.filter')
            var h6 = el.find('h6').off('click');

            if ($(window).width() <= 767) {
                h6.on('click', function () {
                    $(this).toggleClass('active').parent().find('div').toggle();
                });
            }
            else {
                h6.removeClass('active');
                el.find('div').removeAttr('style');
            }
        }).resize();
    },

    ProgramFunctionality: function () {

        // Important Element Variables
        $menuBox = $('.your-menu');
        $recipeTile = $('.program-tiles').find('.recipe-toggle');
        $recipe = $('article.recipe');
        $recipeRemover = $('article.recipe .recipe-inner .remover');
        $yrRemover = $('.remover.year-round-program');
        $productsRemover = $('.products-inner .remover');
        $addRecipeBtn = $('a.recipe-btn');
        $addContentBtn = $('a.content-btn');
        var viewportWidth = viewportSize.getWidth();
        //Ensure empty product qty hidden fields on load 
        $('.prod-qty .qty-input-val').val("0");


        // Smooth scroll from tile area to menu area when clicking a tile button
        $(function () {
            $('a[href*=#]:not([href=#])').click(function () {
                if (location.pathname.replace(/^\//, '') == this.pathname.replace(/^\//, '') && location.hostname == this.hostname) {
                    var target = $(this.hash);
                    target = target.length ? target : $('[name=' + this.hash.slice(1) + ']');
                    if (target.length) {
                        $('html,body').animate({
                            scrollTop: target.offset().top
                        }, 1000);
                        return false;
                    }
                }
            });
        });


        $('input[type="number"]').keypress(function (key) {
            if (key.charCode > 8 && key.charCode < 48 || key.charCode > 57) return false;
        });


        $('input.recipe-qty').slider();

        showTitleAddon();

        var qtySlider = $('article.recipe').find('.ui-slider');
        $(qtySlider).change(function () {
            var sliderParent = $(this).parents('article.recipe');
            var sliderVal = $(this).find('.ui-slider-handle').attr('aria-valuenow');
            var sliderWrapper = $(sliderParent).find('.slider-wrapper');

            calculateYield(sliderParent, sliderVal);

            if (sliderWrapper.hasClass('manually-edited')) {
                $(sliderWrapper).removeClass('manually-edited');
            }

            var handlePosition = $(this).find('.ui-slider-handle').css('left');
            var input = $(this).find('.recipe-qty');
            var inputPosition = handlePosition;


            if (input.is(':hidden')) {
                input.show();
            }
            $(input).css('left', inputPosition);

            // setDefaultQuantities($(this));
        });

        idFormDuplicates();
        function idFormDuplicates() {

            // Holiday 2014
            $('li[data-name="starbucks-holiday-blend-coffee-5oz"], li[data-name="starbucks-holiday-blend-coffee-5-oz"]').addClass('active-form');
            $('li[data-name="starbucks-holiday-blend-coffee-9oz"], li[data-name="starbucks-holiday-blend-coffee-9-oz"]').addClass('inactive-form');
            $('li[data-name="level-4-coffee-6oz"]').addClass('active-form');
            $('li[data-name="level-4-coffee-2oz"]').addClass('inactive-form');

            // Winter 2014 signature-blend-3-—-6-oz
            $('li[data-name="caffè-verona-5-oz"], li[data-name="signature-blend-3-—-6-oz"]').addClass('active-form');
            $('li[data-name="caffè-verona-9-oz"], li[data-name="signature-blend-3-—-175-oz"], li[data-name="signature-blend-3-—-2-oz"]').addClass('inactive-form');
        }

        function setSliders(selectedRecipe) {
            $recipeCalcHandle = $(selectedRecipe).find('.recipe-qty');
            $recipeGroup = $(selectedRecipe).parent();
            $recipeCalcHandle.attr('value', '50').slider("refresh");
            $recipeCalcInputs = $recipeGroup.find('.qty-input');
        }

        function calculateYield(sliderParent, sliderVal) {
            var recipeIngredient = $(sliderParent).find('.recipe-ingredient'); // Get each recipe component
            $(recipeIngredient).each(function (index, element) {
                var recipeGroup = $(this).parents('.recipe-group');
                var yield = $(this).find('.prod-name').data('yield'); // Get each component beverage yield value
                var ratio = $(this).find('.prod-name').data('ratio'); // Get each component beverage ratio value
                var recipe = $(this).find('.prod-name').data('recipe');
                if ( ratio ) {
                    var calculatedRatio = ratio / 100;
                    var calculatedOrderQty = Math.ceil( (sliderVal / yield) * calculatedRatio );
                } else {
                    var calculatedOrderQty = Math.ceil(sliderVal / yield); // Calculate order quantity based on slider value
                }
                if ($(this).hasClass('inactive-form')) {
                    var storedYield = yield;
                    var yield = 0;
                    var calculatedOrderQty = 0;
                }

                $(this).find('.prod-qty').addClass('calculated');
                $(this).find('.prod-qty .qty-input').val(calculatedOrderQty); // Get text input for component
                $(this).find('.prod-qty .qty-input-val').val(calculatedOrderQty); // Get text input for component

                calculateBeverageBreakdown(recipeGroup, recipe, sliderVal);
            });
        }

        function calculateBeverageBreakdown(recipeGroup, recipe, sliderVal) {
            var beverageBreakdown = $(recipeGroup).find('.module-expander');
            var targetRecipe = $(beverageBreakdown).find('.bundle-beverage-count[data-recipe=' + recipe + ']');
            var targetRecipeQty = $(targetRecipe).find('.recipe-quantity');
            var targetRecipeRatio = $(targetRecipe).find('span').data('ratio');
            var beverageQty = Math.floor( sliderVal * ( targetRecipeRatio / 100 ) );
            $(targetRecipeQty).text(beverageQty);
        }


        $('.recipe-ingredient .prod-qty .qty-input').bind('keyup', function () {
            if ($(this).parents('.recipe-ingredient').hasClass('inactive-form')) {
                swapFormDuplicate($(this));
            }
        });

        function swapFormDuplicate(editedField) {
            var editedDuplicate = $(editedField).parents('.recipe-ingredient'),
                swapTarget = $(editedField).parents('.recipe-ingredient-list').find('.recipe-ingredient.active-form');

            $(editedDuplicate).removeClass('inactive-form').addClass('active-form');
            $(swapTarget).removeClass('active-form').addClass('inactive-form');
            $(swapTarget).find('.prod-qty').removeClass('calculated');
            $(swapTarget).find('.prod-qty .qty-input').val(0);
            $(swapTarget).find('.prod-qty .qty-input-val').val(0); // Get text input for component
        }

        $('.qty-input').bind('focus', function () {
            $defaultValue = $(this).val();
        });
        $('.qty-input').bind('blur', function () {
            if ($(this).val() == "") {
                $(this).val(0);
            }
        });
        $('.qty-input').bind('keyup', function () {
            $newValue = $(this).val();
            $(this).next('.prod-qty .qty-input-val').val($(this).val());
            if ($newValue != $defaultValue) {
                var sliderWrapper = $(this).parents('article.recipe').find('.slider-wrapper');
                $(sliderWrapper).addClass('manually-edited');
            }
        });
        if (viewportWidth < 1101) {
            $('.qty-input').bind('focus', function () {
                $defaultValue = $(this).val();
                this.value = '';
            });
            $('.qty-input').bind('keyup', function () {
                $defaultValue = $(this).val();
            });
            $('.qty-input').bind('blur', function () {
                this.value = $defaultValue;
            });
        }

        //Force the user to enter a positive number
        $('.qty-input').change(function () {
            if (parseInt($(this).val()) == $(this).val() && $(this).val() < 0) {
                $(this).val(0);
            }

        });


        checkMenu(); // Confirm empty menu on page load

        // Determine if the empty menu messaging should be visible
        function checkMenu() {
            $menuGroups = $menuBox.find('section.recipe-group');
            if ($menuGroups.is(':visible')) {
                $('.empty-menu').hide();
                $('.program-continue').show();
                console.log('Show It!');
            } else {
                $('.empty-menu').show();
                $('.program-continue').hide();
                console.log('Hide It!');
            }
        }


        $('.content-btn').on('click', function(){
            setTimeout(function () {
                checkMenu();
            }, 100);
        });


        /*
        The user clicking on a recipe tile provides the context for the rest of the
        functionality on the page. If this function mis-fires the rest of the page
        will likely be broken.
        */

        // Get the ID of the selected tile and pass it to the function that reveals the selected recipe group in the menu
        // $recipeTile.on('click', function() {
        //     var selectedTile = $(this).attr('id');
        //     revealRecipeGroup(selectedTile);
        // });

        $addRecipeBtn.on('click', function () {
            var selectedTile = $(this).parent().parent().attr('id');
            revealRecipeGroup(selectedTile);
            $(this).text('Added').removeClass('recipe-btn');
        });

        $addContentBtn.on('click', function() {
            var selectedTile = $(this).parent().parent().attr('id');
            var selectedGroup = $('.your-menu').find("section[data-group='" + selectedTile + "']");

            $(this).text('Added').removeClass('recipe-btn');
            if (selectedGroup.is(':hidden')) {
                $(selectedGroup).addClass('active').addClass('in-menu').addClass('visible');
            }
        });

        function setTileBtnText(selectedButton, selectedTile) {
            var selectedGroup = $('.your-menu').find("section[data-group='" + selectedTile + "']");
            if (selectedGroup.is(':hidden')) {
                selectedButton.addClass('selected');
                selectedButton.text('Added to Menu');
            } else {
                selectedButton.removeClass('selected');
                selectedButton.text('Add to Menu');
            }
        }

        function revealRecipeGroup(selectedTile) {

            // Find the section with a data-group attribute that matches the ID of the tile that the user selected
            var selectedGroup = $('.your-menu').find("section[data-group='" + selectedTile + "']");
            var recipes = $(selectedGroup).find('article.recipe');

            $(recipes).each(function () {
                revealRecipe($(this));
            });

        }

        function revealRecipe(selectedRecipe) {

            var recipeGroup = $(selectedRecipe).parent();
            var marketing = $(recipeGroup).find('article.marketing');
            var recipeSlider = $(selectedRecipe).find('.recipe-qty');

            if (recipeGroup.is(':hidden')) {
                $(recipeGroup).addClass('active').addClass('is-visible');
            }

            if (selectedRecipe.is(':hidden')) {
                $(selectedRecipe).addClass('in-menu');

                setTimeout(function () {
                    if (selectedRecipe.hasClass('in-menu')) {
                        $(selectedRecipe).addClass('visible');
                    }
                }, 100);
            }

            if (marketing.is(':hidden')) {
                $(marketing).addClass('in-menu').addClass('visible');
            }

            setSliders(selectedRecipe);

            // resetSliderQty(selectedRecipe);
            additionalRecipesVisibility();
            setTimeout(function () {
                countAdditionalRecipes();
            }, 1000);
            checkMenu();
        }


        // Fire when the remove link of any recipe is clicked
        // The function will determine which remove was clicked and how to proceed
        $recipeRemover.on('click', function () {
            var selectedRemover = $(this);
            removeRecipe(selectedRemover);
            additionalRecipeClose();
        });

        $productsRemover.on('click', function() {
            var removerProductsGroup = $(this).parents('.program-support-products');
            var productsSection = $(removerProductsGroup).attr('data-group');
            var productsGroupTile = $('.program-tiles').find('div#' + productsSection);
            var productsTileBtn = $(productsGroupTile).find('.program-tile-btn');                                                                                                              
            var visibleProducts = $(removerProductsGroup).find('section.program-support-products.in-menu');
            var selectedRemover = $(this);
            if (visibleProducts.length === 0) {
                $(productsTileBtn).text('Add to menu').addClass('recipe-btn');
            }
            clearProdQty(selectedRemover);
            hideProducts(selectedRemover);
        });

        function clearProdQty(selectedRemover) {
            var productSelectors = $(selectedRemover).parent('.products-inner').find('.program-support-products-select');
            $(productSelectors).each(function() {
                $(this).find('.quantity').val('');
            });
        }

        function hideProducts(selectedRemover) {
            var selectProductSection = $(selectedRemover).parents('.content-band');
            $(selectProductSection).removeClass('active').removeClass('in-menu').removeClass('visible');
        }

        // Hide a recipe
        // It should fade out, and it's slider value should also be reset to zero
        function removeRecipe(selectedRemover) {

            var removedRecipe = $(selectedRemover).parent().parent();

            if ($(removedRecipe).hasClass('in-menu')) {

                $(removedRecipe).addClass('transparent');

                setTimeout(function () {
                    $(removedRecipe).removeClass('in-menu').addClass('is-removed').removeClass('visible');
                    countRecipes(selectedRemover);
                    resetSliderQty(removedRecipe);
                }, 375);
            }

            additionalRecipesVisibility();
            setTimeout(function () {
                countAdditionalRecipes();
            }, 1000);
        }

        function hideRecipe(selectedRecipe) {

            var removedRecipe = $(selectedRecipe).parent();

            if ($(removedRecipe).hasClass('in-menu')) {

                $(removedRecipe).addClass('transparent');

                setTimeout(function () {
                    $(removedRecipe).removeClass('in-menu').addClass('is-removed');
                    countRecipes(selectedRemover);
                    resetSliderQty(removedRecipe);
                }, 375);
            }

            additionalRecipesVisibility();
            setTimeout(function () {
                countAdditionalRecipes();
            }, 1000);
        }

        // Try to dynamically alternate row colors
        // $('.recipe-btn').on('click', function() {
        //     yrCountMenuItems();
        // });

        // $yrRemover.on('click', function(){
        //     yrCountMenuItems();
        // });

        // function yrCountMenuItems() {
        //     setTimeout(function(){
        //         var visibleItems = $('.recipe-calc-wrapper').find('section.active');
        //         // console.log(visibleItems);
        //         if( visibleItems.length > 1 ) {
        //             $(visibleItems).each( function() {
        //                 // Clear the odd/even class
        //                 if( $(this).hasClass('row-even') ) {
        //                     $(this).removeClass('row-even');
        //                 }
        //                 if( $(this).hasClass('row-odd') ) {
        //                     $(this).removeClass('row-odd');
        //                 }
        //             });
        //         }
        //         $('.recipe-calc-wrapper section.active:even').addClass('row-even').css('background-color', '#f6f7f7');
        //         $('.recipe-calc-wrapper section.active:odd').addClass('row-odd').css('background-color', '#fff');
        //     }, 250);
        // }

        //Resets slider value to zero
        function resetSliderQty(removedRecipe) {
            var sliderInput = $(removedRecipe).find('.recipe-qty');
            var sliderHandle = $(removedRecipe).find('.ui-slider-handle');
            $(sliderInput).attr('value', '0').slider("refresh");
            // $(sliderHandle).attr('aria-valuenow', '0');
        }


        // Run this when user clicks to remove a recipe from their menu
        // Count the recipes, and act when recipe count hits 0
        function countRecipes(selectedRemover) {
            var removerRecipeGroup = $(selectedRemover).parent().parent().parent();
            var recipeSection = $(removerRecipeGroup).attr('data-group');
            var recipeGroupTile = $('.program-tiles').find('div#' + recipeSection);
            var recipeTileBtn = $(recipeGroupTile).find('.program-tile-btn');
            var visibleRecipes = $(removerRecipeGroup).find('article.recipe.in-menu');
            var n = visibleRecipes.length;
            if (visibleRecipes.length === 0) {
                recipeReset(removerRecipeGroup);
                $(recipeTileBtn).text('Add to menu').addClass('recipe-btn');
            }
        }


        // Run this when all recipes in a group are removed by the user
        // Reset the selected quantities, reset classes, reset tile states to default
        function recipeReset(selectedRecipeGroup) {
            var targetRecipes = $(selectedRecipeGroup).find('article.recipe');
            $(selectedRecipeGroup).removeClass('active').removeClass('is-visible');
            $(targetRecipes).removeClass('in-menu').removeClass('transparent').removeClass('is-removed');
            marketingReset(targetRecipes);
            checkMenu();
        }


        // Deselect selected marketing materials if parent recipe group gets reset/hidden
        function marketingReset(targetRecipes) {
            var marketingList = targetRecipes.parent().find('.marketing-list li');
            $(marketingList).each(function () {
                $(this).find('input[type="checkbox"]').attr('checked', false);
            })
        }

        $additionalRecipes = $('ul.additional-recipes li');
        $additionalRecipes.on('click', function () {
            var selectedRecipe = $(this).attr('data-name');
            additionalRecipesMenu(selectedRecipe);
            additionalRecipesVisibility();
            additionalRecipeClose();
        });

        function additionalRecipesMenu(selectedRecipe) {
            var addRecipe = $('.your-menu').find("article[data-name='" + selectedRecipe + "']");
            revealRecipe(addRecipe);
        }

        additionalRecipesVisibility();

        // Populate additional recipes selector
        function additionalRecipesVisibility() {
            setTimeout(function () {
                $additionalRecipes.each(function () {
                    var recipeName = $(this).attr('data-name');
                    var menuRecipe = $("article.recipe[data-name='" + recipeName + "']");

                    if (menuRecipe.is(':visible')) {
                        $(this).hide();
                        $(this).addClass('in-menu');
                    } else {
                        $(this).show();
                        $(this).removeClass('in-menu');
                    }
                });
            }, 1000);
        }

        function additionalRecipeClose() {
            if ($('.collapser').is(':visible')) {
                $('.collapser').hide();
                $('.collapser').prev('.expander').toggleClass('active');
            }
        }

        function countAdditionalRecipes() {
            var additionalList = $('ul.additional-recipes');
            var totalRecipes = additionalList.find('li').length;
            var visibleRecipes = additionalList.find('li.in-menu').length;

            if (visibleRecipes === totalRecipes) {
                $('.module-expander').hide();
            } else {
                $('.module-expander').show();
            }
        }

        function showTitleAddon() {
            var recipeGroups = $('section.recipe-group');
            var recipeGroupTitle = $(recipeGroups).find('h3.group-title');
            $(recipeGroups).each(function () {
                var recipeCount = $(this).find('article.recipe').length;
                if (recipeCount > 1) {
                    $(this).find('.add-on-text').show();
                }
            });
        }

        if (viewportWidth < 1101) {

            $productQtyInputs = $('.prod-qty .qty-input');
            $productQtyInputs.each(function () {
                $default_value = this.value;

                $(this).bind('focus', function () {
                    if (this.value == $default_value) {
                        this.value = '';
                    }
                });
                $(this).bind('blur', function () {

                    if (this.value == '') {
                        this.value = $default_value;
                    }
                });
            });
        }


        if (viewportWidth > 1100) {
            var tour = $('#program-tour');
            $("#program-tour").joyride({
                'tipLocation': 'top',
                'nubPosition': 'auto',           // override on a per tooltip bases
                'scrollSpeed': 500,
                'tipAnimationFadeSpeed': 300,
                'postRideCallback': function () {
                    $('#help-launcher, #year-round-help-launcher').removeClass('tour-started');
                    helpBtnText();
                    helpOverlay();
                    displayTour();
                    $('body').scrollTop(0);
                }
            });
        }

        if (viewportWidth > 767 && viewportWidth < 1101) {
            var tour = $('#program-tour-tablet');
            $("#program-tour-tablet").joyride({
                'nubPosition': 'auto',           // override on a per tooltip bases
                'scrollSpeed': 500,
                'tipAnimationFadeSpeed': 300,
                'postRideCallback': function () {
                    $('#help-launcher, #year-round-help-launcher').removeClass('tour-started');
                    helpBtnText();
                    helpOverlay();
                    displayTour();
                    $('body').scrollTop(0);
                }
            });
        }

        if (viewportWidth < 768) {
            var tour = $('#program-tour-mobile');
            $("#program-tour-mobile").joyride({
                'tipLocation': 'bottom',
                'nubPosition': 'auto',           // override on a per tooltip bases
                'scrollSpeed': 500,
                'tipAnimationFadeSpeed': 300,
                'postRideCallback': function () {
                    $('#help-launcher, #year-round-help-launcher').removeClass('tour-started');
                    helpBtnText();
                    helpOverlay();
                    displayTour();
                    $('body').scrollTop(0);
                }
            });
        }

        var helpBtn = $('#help-launcher, #year-round-help-launcher'),
            overlay = $('.program-help-overlay'),
            tourRecipeGroup = $('#joyride-addbtn0').parent().parent().attr('id');

        $(helpBtn).on('click', function () {
            if ( $('body').attr('id').contains("yearround") ) {
                var isYearRound = true;
            } else {
                var isYearRound = false;
            }
            if ($(this).hasClass('tour-started')) {
                $(this).removeClass('tour-started');
            } else {
                $(this).addClass('tour-started');
            }
            helpBtnText();
            helpOverlay();
            displayTour(isYearRound);
        });

        function helpOverlay() {
            if (helpBtn.hasClass('tour-started')) {
                $(overlay).show();
            } else {
                $(overlay).hide();
            }
        }

        function displayTour(isYearRound) {
            if (helpBtn.hasClass('tour-started')) {
                showTourRecipes();
                if ( isYearRound == true ) {
                    showTourSupportProducts();
                }
                $(tour).joyride();
                // $("#program-tour").joyride('resume');
            } else {
                // $(tour).joyride('end');
                $('body').scrollTop(0);
                $('.joyride-tip-guide').hide();
                hideTourRecipes();
                // var selectedRemover = $('.products-inner .remover');
                hideProducts($productsRemover);
            }
        }

        function helpBtnText() {
            if (helpBtn.hasClass('tour-started')) {
                $(helpBtn).text('End Tour');
            } else {
                $(helpBtn).text('Take a Tour');
            }
        }

        function showTourRecipes() {
            $('.recipe-group-0 article.recipe').each(function () {
                revealRecipe($(this));
            });
        }

        function hideTourRecipes() {
            $('.recipe-group-0 article.recipe').each(function () {
                var selectedRemover = $(this).find('.remover');
                removeRecipe(selectedRemover);
            });
        }

        function showTourSupportProducts() {
            $('.content-band.program-support-products').addClass('active in-menu visible');
        }

        function hideTourSupportProducts() {
            $('.content-band.program-support-products').hide();
        }

        $(overlay).on('click', function () {
            $(helpBtn).removeClass('tour-started');
            helpBtnText();
            helpOverlay();
            displayTour();
            $('body').scrollTop(0);
            $('.joyride-tip-guide').hide();
        });


        $('article.product').each(function () {

            var overlay = $(this).find('#out-of-stock-overlay');
            if ($(this).hasClass('seasonal')) {


                $(overlay).addClass('active');
            }
        });

        if( viewportWidth < 768 ) {
            $('.support-products-qty').attr('placeholder', 'QTY');
        }

        $('#finish-prog-order').click(function () {
            SBS.AddProgramItemsToBag($(this));
        });
    },

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
    },

    EquipmentFunctionality: function (equipment) {
        $('#filter-toggle').on('click', function () {
            $(this).next('#filters-holder').slideToggle(250);
        });

        for (var item in equipment) {
            var div = $('#product-v2').clone().removeAttr('id').removeAttr('style');
            div.find('img').attr('title', equipment[item].Title);
            div.find('h3').text(equipment[item].Title);
            div.find('p').text(equipment[item].Description);

            var link = div.find('a');
            if (equipment[item].Video) {
                link.eq(0).data("reveal-id", "videoModal_" + item);
                link.eq(1).attr('href', "http://player.vimeo.com/video/" + equipment[item].Video);

                var m = div.find('.trainingVideoModal');
                m.attr('id', "videoModal_" + item);
                m.find('iframe').attr({
                    'id': "videoModal_" + item,
                    'src': '//player.vimeo.com/video/' + equipment[item].Video + '?api=1&player_id=video-player_' + item
                });
            }
            else {
                link.attr('href', equipment[item].URL);
            }

            var img = div.find('img');
            if (equipment[item].Image) {
                img.attr('src', '/img/equipment/200/' + equipment[item].Image + '.jpg');
            }
            else {
                img.attr('src', '/img/icon-play-video.png');
            }

            $('#equipment-list').append(div);
        }
    },

    Bag: function () {
        var orderCtnr = $(".bag");

        var acctSiteNumCtrl = $('#Order_AccountSiteNumber');
        if (acctSiteNumCtrl.prop("tagName").toLowerCase() == "select"){
            acctSiteNumCtrl.select2();
        }

        acctSiteNumCtrl.add("#Order_PurchaseOrderNumber").change( function () {
            saveOrder();
        });

        //Edit qty
        $(".qty-field").focusin(function () {
            if ($(this).hasClass("inactive")) {
                toggleQtyField($(this));
            }
        });

        $(".qty-field").keyup(function (e) {
            if (e.which == 13) {
                e.preventDefault();
                $(this).parent().find(".save-qty-btn").click();
                $(this).blur();
            }
        });

        $("#BagForm .edit-qty-btn, #BagForm .cancel-qty-btn").click(function () {
            toggleQtyField($(this));
        });

        $("#BagForm .save-qty-btn").click(function () {
            saveOrder();
            var qtyField = $(this).closest(".bag-qty").find(".qty-field");
            qtyField.data("prev", qtyField.val());
            toggleQtyField($(this));
        });

        var toggleQtyField = function (el) {
            var parent = $(el).closest(".bag-qty");
            var qtyField = parent.find(".qty-field");
            var editBtn = parent.find(".edit-qty-btn");
            var saveBtn = parent.find(".save-qty-btn");
            var cancelBtn = parent.find(".cancel-qty-btn");

            if (qtyField.hasClass("inactive")) {
                qtyField.data("prev", qtyField.val());
                qtyField.removeClass("inactive");
                editBtn.hide();
                saveBtn.show();
                cancelBtn.show();
                qtyField.focus();
            } else {
                qtyField.val(qtyField.data("prev"));
                qtyField.removeAttr("data-prev");
                editBtn.show();
                saveBtn.hide();
                cancelBtn.hide();
                qtyField.addClass("inactive");
            }
        };

        //Only allow numbers for quantities
        $('input[type="number"]').keypress(function (key) {
            if (key.charCode > 8 && key.charCode < 48 || key.charCode > 57) return false;
        });

        //Delete Order Item Link
        $(".delete-orderitem").click(function (e) {
            e.stopPropagation();
            var itemCtnr = $(this).parents(".order-item");
            $.ajax({
                url: itemCtnr.data("deleteaction"),
                type: 'DELETE',
                success: function (result) {
                    if (result.success) {
                        itemCtnr.slideUp(300, function () {
                            $(this).remove();
                            CheckIfBagEmpty();
                        });
                    }
                },
                error: function (request, status, error) {
                    alert(error);
                }
            });
        });

        //submit order
        $("#SubmitOrder").click(function () {
            if (!CheckIfBagEmpty()) {
                var btn = $(this);

                //validate quantities
                $("#BagForm [type=number]").each(function () {
                    var qtyField = $(this);
                    if (qtyField.val().length == 0 || qtyField.val() == '') {
                        qtyField.select();
                        return false;
                    }
                });

                var defaultText = btn.text();
                btn.html('<img src="/img/form-icons/loading-reverse.gif" width="16" height="16" /> ' + defaultText);
                btn.attr("disabled", "disabled");
                $.ajax({
                    url: btn.data("action"),
                    type: 'POST',
                    data: $("#BagForm").serialize(),
                    success: function (result) {
                        if (result.success) {
                            _gaq.push(['_setAccount', result.Order.AccountNumber]);
                            _gaq.push(['_addTrans', result.Order.OrderID, 'solutions.starbucks.com', '', '', '', result.Order.ShipToCity, result.Order.ShipToState, '']);

                            for (var i = 0; i < result.Order.OrderItems.length; i++) {
                                var item = result.Order.OrderItems[i];
                                _gaq.push(['_addItem', item.OrderID, item.SKUNumber, item.Name, '', '0.00', item.Quantity]);
                            }
                            _gaq.push(['_trackTrans']);
                            btn.html('<i class="fa fa-check"></i> ' + defaultText);
                            document.location.href = "/my-bag/ConfirmOrder?orderid="+result.Order.OrderID;
                        } else {
                            alert(result.message);
                            btn.html(defaultText);
                            btn.removeAttr("disabled");
                        }
                    },
                    error: function (request, status, error) {
                        alert(error);
                        btn.html(defaultText);
                        btn.removeAttr("disabled");
                    }
                });
            }
            
        });

        var CheckIfBagEmpty = function () {
            if ($(".row.order-item").length == 0) {
                $(".bag-empty").show();
                $(".btn-green, .bag-shopping-form, .purchase-order-no").hide();
                $("#header-bag").removeClass("filled");
                return true;
            }
        }

        var saveOrder = function () {
            $.ajax({
                url: $("#BagForm").data("action"),
                type: 'POST',
                data: $("#BagForm").serialize(),
                success: function (result) {
                    if (!result.success) {
                        alert(result.message);
                    }
                },
                error: function (request, status, error) {
                    alert(error);
                }
            });
        }
    },

    AddItemToBag: function (btn) {

        var defaultText = "ADD TO BAG";
        if (!btn.hasClass("adding-item")) {

            var errorMsgCtnr = $("#forgotError");
            errorMsgCtnr.addClass("hide");

            if (!SBS.AccountAttribute("account-number")) {
                errorMsgCtnr.text("No account selected.").removeClass("hide");
                return false;
            }

            //validate the sku
            var sku = $(".add-to-bag [name=sku]").val().split("|").filter(function (n) { return n != "" })[0];
            if (sku == '') {
                errorMsgCtnr.text("No Size/SKU selected.").removeClass("hide");
                return false;
            }

            //validate the quantity
            var qtyField = $(".add-to-bag [name=qty]");
            if (qtyField.val().length == 0 || qtyField.val() == '') {
                errorMsgCtnr.text("No quantity indicated.").removeClass("hide");
                qtyField.select();
                return false;
            }

            //get the siteId
            var siteId = SBS.AccountAttribute("brand");
            if (siteId === "DUAL") {
                siteId = "SBUX";
                var selectedItemSiteIds = $(".add-to-bag [name=siteId]").val();
                if (selectedItemSiteIds.indexOf("\"SBC\"") > -1 && selectedItemSiteIds.indexOf("\"SBX\"") < 0)
                {
                    siteId = "SBC";
                }
            }

            btn.html('<img src="/img/form-icons/loading-reverse.gif" width="16" height="16" /> ' + defaultText);
            btn.addClass("adding-item");
            $.ajax({
                url: btn.data("action"),
                type: 'POST',
                data: {
                    sku: sku,
                    qty: qtyField.val(),
                    siteId: siteId
                },
                success: function (result) {
                    if (result.success) {
                        btn.html('<i class="fa fa-check"></i> ADDED TO BAG');
                        $("#header-bag").addClass("filled");
                    } else {
                        btn.text(defaultText);
                        alert(result.message);
                    }
                    btn.removeClass("adding-item");
                },
                error: function (request, status, error) {
                    alert(error);
                    btn.text(defaultText);
                    btn.removeClass("adding-item");
                }
            });
        }
    },

    AddProgramItemsToBag: function (btn) {

        var defaultText = "ADD TO BAG";
        if (!btn.hasClass("adding-item")) {

            var formIsValid = false;

            //One of these items needs to have a quantity, otherwise we have nothing to send to bag
            $(".qty-input").each(function () {
                if ($(this).val() != '0' && $(this).val() != '') {
                    formIsValid = true;
                }
            });

            if (!formIsValid) {
                alert("No items to add.");
                return false;
            }

            if (!SBS.AccountAttribute("account-number")) {
                alert("No account selected.");
                return false;
            }

            btn.html('<img src="/img/form-icons/loading-reverse.gif" width="16" height="16" /> ' + defaultText);
            btn.addClass("adding-item");
            $.ajax({
                url: btn.data("action"),
                type: 'POST',
                data: $("#programItemsForm").serialize(),
                success: function (result) {
                    if (result.success) {
                        btn.html('<i class="fa fa-check"></i> ADDED TO BAG');
                        $("#header-bag").addClass("filled");
                        if (result.redirect !== "") {
                            document.location.href = "/my-bag";
                        }
                    } else {
                        btn.text(defaultText);
                    }
                    btn.removeClass("adding-item");
                },
                error: function (request, status, error) {
                    alert(error);
                    btn.text(defaultText);
                    btn.removeClass("adding-item");
                }
            });

        }

    },

    AccountAttribute: function (attribute)
    {
        attribute = attribute.toLowerCase();
        switch (attribute) {
            case "brand":
            case "is-frap":
            case "is-espresso":
            case "is-partner":
            case "account-number":
                return $("#current-account-details").data(attribute);
            default:
                return "";
        }
    }
}