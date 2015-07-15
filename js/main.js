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