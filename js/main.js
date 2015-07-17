$(document).ready(function () {
    SBS.Begin();
});

SBS = {

    // automatically superscript registered and trademark symbols
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

        // Open appropriate country select menu when flag is clicked/touched
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

        // expand sub-menus on navigation menu
        function expandNavigation() {
            if (!$(this).next($subMenu).is(':visible')) {
                $subMenu.slideUp(500);
                $(this).next($subMenu).slideDown(500);
            }
        }

        // close sub-menus on navigation menu
        function collapseNavigation() {
            if ($(this).next($subMenu).is(':visible')) {
                $(this).next($subMenu).slideUp(500);
            }
        }

        // is it a touch device?
        function is_touch_device() {
            return !!('ontouchstart' in window);
        }

        // Hopefully add :checked support for IE8 on custom check forms
        $('input:radio, input:checkbox').checkedPolyfill();
        $('input#RememberMe').on('click', function () {
            $('#RememberMe').toggleClass('checked');
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
}