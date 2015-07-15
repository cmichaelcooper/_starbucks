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

    /* End the tour if a user clicks on the overlay */
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
}