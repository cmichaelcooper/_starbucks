angular.module("common").filter('split', function () {
    return function (input, splitChar, splitIndex) {
        // do some bounds checking here to ensure it has that index
        return input.split(splitChar)[splitIndex];
    }
})

angular.module("common").filter('groupBy', function () {
    return function (collection, key) {
        if (collection === null) return;
        return uniqueItems(collection, key);
    };
});

angular.module("common").filter('isCoffeeProduct', function ($filter) {
    return function (input) {
        var returnArray = [];
        angular.forEach(input, function (product) {
            if (
                $filter('filter')(product.ProductTraitValues, { TypeID: 'Iced_Coffee' })[0] ||
                $filter('filter')(product.ProductTraitValues, { TypeID: 'Espresso' })[0] ||
                $filter('filter')(product.ProductTraitValues, { TypeID: 'Instant_Coffee' })[0] ||
                $filter('filter')(product.ProductTraitValues, { TypeID: 'Drip_Brewed' })[0]) {
                returnArray.push(product);
            }
        })

        return returnArray;
    };
});

angular.module("common").filter('isFontanaProduct', function ($filter) {
    return function (input) {
        var returnArray = [];
        angular.forEach(input, function (product) {
            if (
                $filter('filter')(product.ProductTraitValues, { TypeID: 'Fontana_Syrups_And_Sauces' })[0]) {
                returnArray.push(product);
            }
        })

        return returnArray;
    };
});

angular.module("common").filter('isTeaProduct', function ($filter) {
    return function (input) {
        var returnArray = [];
        angular.forEach(input, function (product) {
            if (
                $filter('filter')(product.ProductTraitValues, { TypeID: 'Tazo_Tea' })[0] ||
                $filter('filter')(product.ProductTraitValues, { TypeID: 'Tazo_Chai' })[0]) {
                returnArray.push(product);
            }
        })

        return returnArray;
    };
});

angular.module("common").filter('isFrappuccinoProduct', function ($filter) {
    return function (input) {
        var returnArray = [];
        angular.forEach(input, function (product) {
            if (
                $filter('filter')(product.ProductTraitValues, { TypeID: 'Frappuccino' })[0]) {
                returnArray.push(product);
            }
        })

        return returnArray;
    };
});

angular.module("common").filter('isSupplyProduct', function ($filter) {
    return function (input) {
        var returnArray = [];
        angular.forEach(input, function (product) {
            if (
                $filter('filter')(product.ProductTraitValues, { TypeID: 'Supplies' })[0] ||
                $filter('filter')(product.ProductTraitValues, { TypeID: 'Supplies_Sbc' })[0] ||
                $filter('filter')(product.ProductTraitValues, { TypeID: 'Paper_Products' })[0] ||
                $filter('filter')(product.ProductTraitValues, { TypeID: 'Paper_Products_Sbc' })[0]) {
                returnArray.push(product);
            }
        })

        return returnArray;
    };
});

angular.module("common").filter('isMiscellaneousProduct', function ($filter) {
    return function (input) {
        var returnArray = [];
        angular.forEach(input, function (product) {
            if (
                $filter('filter')(product.ProductTraitValues, { TypeID: 'Ingredients_Merchandise' })[0]) {
                returnArray.push(product);
            }
        })

        return returnArray;
    };
});

angular.module("common").filter('isNotCoffeeProduct', function ($filter) {
    return function (input) {
        var returnArray = [];
        angular.forEach(input, function (product) {
            if (
                !$filter('filter')(product.ProductTraitValues, { TypeID: 'Iced_Coffee' })[0] &&
                !$filter('filter')(product.ProductTraitValues, { TypeID: 'Espresso' })[0] &&
                !$filter('filter')(product.ProductTraitValues, { TypeID: 'Instant_Coffee' })[0] &&
                !$filter('filter')(product.ProductTraitValues, { TypeID: 'Drip_Brewed' })[0]) {
                returnArray.push(product);
            }
        })

        return returnArray;
    };
});

angular.module("common").filter('recipeCategory', function ($filter) {
    return function (input, val) {
        var returnArray = [];
        angular.forEach(input, function (recipe) {
            if (recipe.TypeID.indexOf(val) > -1) {
                returnArray.push(recipe);
            }
        })
        return returnArray;
    };
});

angular.module("common").filter('isSelected', function () {
    return function (input) {
        var returnArray = [];
        angular.forEach(input, function (item) {
            if(item.isSelected)
            {
                returnArray.push(item);
            }
        })

        return returnArray;
    };
});

angular.module("common").filter('isInFilterCategory', function () {
    return function (input, val) {

        var returnArray = [];
        angular.forEach(input, function (item) {
            if (item.category == val) {
                returnArray.push(item);
            }
        })

        return returnArray;
    };
});

angular.module("common").filter('isInSelectedFilters', function () {
    return function (input, filters) {
        var returnArray = [];
        angular.forEach(input, function (item) {
            var returnValue = true;
            var returnChecks = {
                brand: { checked: false, valid: false },
                kind: { checked: false, valid: false },
                form: { checked: false, valid: false },
                roast: { checked: false, valid: false },
                format: { checked: false, valid: false },
                caffeine: { checked: false, valid: false },
                type: { checked: false, valid: false },
                flavor: { checked: false, valid: false },
                ingredient: { checked: false, valid: false }
            }

            angular.forEach(filters, function (filter) {
                if (filter.isSelected) {

                    returnChecks[filter.category].checked = true;
                    if (filter.type == "product") {
                        if (filter.category == "brand") {
                            if (item.SiteId.indexOf(filter.value) > -1) {
                                returnChecks[filter.category].valid = true;
                            }
                        }

                        if (filter.category == "caffeine" || filter.category == "roast" || filter.category == "kind" || filter.category == "type" || filter.category == "format" || filter.category == "flavor") {
                            var alreadyProcessed = 0;

                            if (filter.label === 'Serveware') {
                                for (var jj = item.ProductTraitValues.length - 1; jj >= 0; jj--) {
                                    if (item.ProductTraitValues[jj].Value === 'Serveware') {
                                        returnChecks[filter.category].valid = true;
                                    }
                                }
                                alreadyProcessed = 1;
                            }

                            if (filter.label === 'Supplies') {
                                for (var jj = item.ProductTraitValues.length - 1; jj >= 0; jj--) {
                                    if (item.ProductTraitValues[jj].Value === 'Supplies') {
                                        returnChecks[filter.category].valid = true;
                                    }
                                }
                                alreadyProcessed = 1;
                            }

                            if (filter.label === 'Cleaning') {
                                for (var jj = item.ProductTraitValues.length - 1; jj >= 0; jj--) {
                                    if (item.ProductTraitValues[jj].Value === 'Cleaning') {
                                        returnChecks[filter.category].valid = true;
                                    }
                                }
                                alreadyProcessed = 1;
                            }

                            if (filter.value === 'Other') {
                                for (var jj = item.ProductTraitValues.length - 1; jj >= 0; jj--) {
                                    if ((item.ProductTraitValues[jj].Trait === 'Flavor'
                                            && (item.ProductTraitValues[jj].Value !== 'Mocha'
                                            && item.ProductTraitValues[jj].Value !== 'Caramel'
                                            && item.ProductTraitValues[jj].Value !== 'Vanilla'
                                            && item.ProductTraitValues[jj].Value !== 'Chocolate'))) {
                                        returnChecks[filter.category].valid = true;
                                    }
                                }
                                alreadyProcessed = 1;
                            }

                            if (alreadyProcessed == 0 && filter.value !== 'Other') {
                                for (var jj = item.ProductTraitValues.length - 1; jj >= 0; jj--) {
                                    if ((filter.value === item.ProductTraitValues[jj].TypeID) || (filter.value === item.ProductTraitValues[jj].Value)) {
                                        returnChecks[filter.category].valid = true;
                                    }
                                }
                            }
                        }

                        if (filter.category == "form") {
                            for (var jj = item.SkuInformation.length - 1; jj >= 0; jj--) {
                                if (filter.value === item.SkuInformation[jj].Value) {
                                    returnChecks[filter.category].valid = true;
                                }
                            }
                        }
                    } else if (filter.type == "recipe") {


                        if (filter.category == "brand") {
                            if (item.SiteID == filter.value || item.TypeID.indexOf(filter.value) > -1) {
                                returnChecks[filter.category].valid = true;
                            }
                        }


                        if (filter.category == "ingredient") {
                            for (var jj = item.RecipeTraits.length - 1; jj >= 0; jj--) {
                                
                                if (filter.value === 'Coffee') {
                                    if ((item.RecipeTraits[jj].TraitID === 'Main_Ingredient' && (item.RecipeTraits[jj].Value === 'Espresso' || item.RecipeTraits[jj].Value === 'Espresso_Roast' || item.RecipeTraits[jj].Value === 'Coffee' || item.RecipeTraits[jj].Value === 'Starbucks VIA® Ready Brew') )) {
                                        returnChecks[filter.category].valid = true;
                                    }
                                } else if (filter.value === 'non-coffee') {

                                    if ((item.RecipeTraits[jj].TraitID === 'Main_Ingredient' && (item.RecipeTraits[jj].Value === 'Beverage Base' || item.RecipeTraits[jj].Value === 'Blended Base' || item.RecipeTraits[jj].Value === 'Chai' || item.RecipeTraits[jj].Value === 'Chocolate chai concentrate' || item.RecipeTraits[jj].Value === 'Cocoa' || item.RecipeTraits[jj].Value === 'Fontana® Caramel Syrup' || item.RecipeTraits[jj].Value === 'Macchiato' || item.RecipeTraits[jj].Value === 'Milk' || item.RecipeTraits[jj].Value === 'Tea'))) {
                                        returnChecks[filter.category].valid = true;
                                    }

                                }
                            }
                        }

                        if (filter.category == "flavor") {
                            for (var jj = item.RecipeTraits.length - 1; jj >= 0; jj--) {
                                if (filter.value !== 'Other') {
                                    if ((item.RecipeTraits[jj].TraitID === 'Flavor' && (item.RecipeTraits[jj].Value === filter.value ))) {
                                        returnChecks[filter.category].valid = true;
                                    }
                                } else {
                                    if ((item.RecipeTraits[jj].TraitID === 'Flavor' && (item.RecipeTraits[jj].Value !== 'Mocha' && item.RecipeTraits[jj].Value !== 'Caramel' && item.RecipeTraits[jj].Value !== 'Vanilla' && item.RecipeTraits[jj].Value !== 'White Chocolate'))) {
                                        returnChecks[filter.category].valid = true;
                                    }
                                }
                            }
                        }
                    }

                }
            });
            
            var dupeFound = false;
            angular.forEach(returnArray, function (filteredItem) {
                if (item.RecipeID != null && item.Title == filteredItem.Title) {
                    dupeFound = true;
                }
            });

            if ((!returnChecks.brand.checked || returnChecks.brand.valid) &&
                (!returnChecks.caffeine.checked || returnChecks.caffeine.valid) &&
                (!returnChecks.roast.checked || returnChecks.roast.valid) &&
                (!returnChecks.kind.checked || returnChecks.kind.valid) &&
                (!returnChecks.form.checked || returnChecks.form.valid) &&
                (!returnChecks.format.checked || returnChecks.format.valid) &&
                (!returnChecks.ingredient.checked || returnChecks.ingredient.valid) &&
                (!returnChecks.flavor.checked || returnChecks.flavor.valid) &&
                (!returnChecks.type.checked || returnChecks.type.valid) &&
                !dupeFound) returnArray.push(item);
        })

        return returnArray;
    };
});

