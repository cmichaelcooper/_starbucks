angular.module("product_lookup").controller('coffeeCatalogController', function ($scope, appService, isCoffeeProductFilter, $filter, $log) {
    $scope.Products = [];

    $scope.filters = [
           {
               label: "Drip",
               value: "Drip_Brewed",
               category: "kind",
               type: "product"
           },
           {
               label: "Espresso",
               value: "Espresso",
               category: "kind",
               type: "product"
           },
           {
               label: "Instant",
               value: "Instant_Coffee",
               category: "kind",
               type: "product"
           },
           {
               label: "Iced",
               value: "Iced_Coffee",
               category: "kind",
               type: "product"
           },
           {
               label: "Starbucks",
               value: "SBX",
               category: "brand",
               type: "product"
           },
           {
               label: "Seattle's Best",
               value: "SBC",
               category: "brand",
               type: "product"
           },
           {
               label: "Whole Bean",
               value: "Whole Bean",
               category: "form",
               type: "product"
           },
           {
               label: "Portion Pack",
               value: "Portion Pack",
               category: "form",
               type: "product"
           },
           {
               label: "Ground",
               value: "Ground",
               category: "form",
               type: "product"
           },
           {
               label: "Blonde",
               value: "Blonde Roast",
               category: "roast",
               type: "product"
           },
           {
               label: "Medium",
               value: "Medium Roast",
               category: "roast",
               type: "product"
           },
           {
               label: "Dark",
               value: "Dark Roast",
               category: "roast",
               type: "product"
           },
           {
               label: "Regular",
               value: "Regular",
               category: "caffeine",
               type: "product"
           },
           {
               label: "Decaf",
               value: "Decaf",
               category: "caffeine",
               type: "product"
           }
    ];

    $scope.$watch("filters", function (n, o) {
        var selected = $filter("filter")(n, { isSelected: true });

        var filters = $('.filter div');
        filters.filter('.active').removeClass('active');
        for (var s in selected) {
            for (f = 0; f < filters.length; f++) {
                var text = $.trim(filters.eq(f).text());
                if (text === selected[s].label) {
                    filters.eq(f).addClass('active');
                    break;
                }
            }
        }
    }, true);

    $scope.select = function (product) {
        $scope.selected = product;
        //$scope.someModel = product.SkuInformation[0].Value;

        var isDecaf = $filter('filter')($scope.Products, { ProductID: 'Decaf_' + $scope.selected.ProductID })[0];

        if (isDecaf) {
            product.decafVersion = isDecaf.ProductID;

        } else {
            product.decafVersion = false;
        }

        product.isCoffee = appService.checkCoffeeStatus(product.SkuInformation, 'Form');

        var instantFilter = $filter('filter')(product.ProductTraitValues, { Trait: 'Kind' })[0];
        if (instantFilter) {
            if (instantFilter.Value === "Instant") {
               product.isInstant = 1;
            }
        }

        product.isOrderable = appService.checkOrderableStatusByBrand(product.SiteId, SBS.AccountAttribute('brand'));

        product.items1 = $filter('filter')(product.SkuInformation, { Trait: 'Form' });
        product.selectedItem1 = {};
    }

    $scope.resetFilters = function () {
        angular.forEach($scope.filters, function (filter) {
            filter.isSelected = false;
        });
    }

    $scope.$watch("selected.selectedItem1.selectedItem", function () {
        if ($scope.selected != undefined) {
            console.log($scope.selected);
            var itemsTemp = $filter('filter')($scope.selected.SkuInformation, { Trait: 'Form' });
            $scope.selected.selectedItem1.items2 = $filter('filter')(itemsTemp, { Value: $scope.selected.selectedItem1.selectedItem.Value });

            var itemsTempSizes = $filter('filter')($scope.selected.SkuInformation, { Trait: 'Size' });
            var itemTempSizeItem = $filter('filter')(itemsTempSizes, { SKUNumber: $scope.selected.selectedItem1.selectedItem.SKUNumber })[0];
            if (itemTempSizeItem) {
                $scope.selected.selectedItem1.Size = itemTempSizeItem.Value;
            }


        }
    }, true);

    $scope.$watch("selected.selectedItem1.selectedItem2", function () {
        if ($scope.selected != undefined) {
            var itemsTemp = $filter('filter')($scope.selected.SkuInformation, { Trait: 'Brewer Type' });
            $scope.selected.selectedItem1.selectedItem2.items3 = $filter('filter')(itemsTemp, { SKUNumber: $scope.selected.selectedItem1.selectedItem2.SKUNumber });
        }
    }, true);

    $scope.removeFilter = function (filter) {
        filter.isSelected = false;
    };

    $scope.convertBrandAcronym = function (site) {
        if (site === 'SBX') {
            return 'Starbucks';
        }
        else if (site === 'SBC') {
            return 'Seattle\'s Best';
        }
        else if (site === 'SBXOCS') {
            return 'Starbucks Office Coffee';
        }
    }

    var promiseProductsGet = appService.getProducts(); //The MEthod Call from service
    promiseProductsGet.then(function (pl) {
        $scope.Products = isCoffeeProductFilter(pl.data.allProducts);
        $scope.selected = {};
    }, function (errorPl) {
        $log.error('failure loading coffee', errorPl);
    });
});

angular.module("product_lookup").controller('tazoCatalogController', function ($scope, appService, isTeaProductFilter, $filter, $log) {
    $scope.Products = [];

    $scope.filters = [
           {
               label: "Hot",
               value: "Hot",
               category: "kind",
               type: "product"
           },
           {
               label: "Iced",
               value: "Iced",
               category: "kind",
               type: "product"
           },
           {
               label: "Latte Concentrate",
               value: "Tea Latte Concentrate",
               category: "format",
               type: "product"
           },
           {
               label: "Concentrate",
               value: "Iced Tea Concentrate",
               category: "format",
               type: "product"
           },
           {
               label: "Bulk",
               value: "1 Gallon Bulk Iced Tea",
               category: "format",
               type: "product"
           },
           {
               label: "Filterbag",
               value: "Filterbag",
               category: "format",
               type: "product"
           }
           ,
           {
               label: "Chai",
               value: "Chai",
               category: "type",
               type: "product"
           }
           ,
           {
               label: "Green",
               value: "Green",
               category: "type",
               type: "product"
           }
           ,
           {
               label: "Herbal",
               value: "Herbal",
               category: "type",
               type: "product"
           }
           ,
           {
               label: "Mixed",
               value: "Mixed",
               category: "type",
               type: "product"
           }
           ,
           {
               label: "Black",
               value: "Black",
               category: "type",
               type: "product"
           }
           ,
           {
               label: "Regular",
               value: "Regular",
               category: "caffeine",
               type: "product"
           },
           {
               label: "Decaf",
               value: "Decaf",
               category: "caffeine",
               type: "product"
           }
    ];

    $scope.$watch("filters", function (n, o) {
        var selected = $filter("filter")(n, { isSelected: true });

        var filters = $('.filter div');
        filters.filter('.active').removeClass('active');
        for (var s in selected) {
            for (f = 0; f < filters.length; f++) {
                var text = $.trim(filters.eq(f).text());
                if (text === selected[s].label) {
                    filters.eq(f).addClass('active');
                    break;
                }
            }
        }
    }, true);

    $scope.select = function (product) {
        $scope.selected = product;
        //$scope.someModel = product.SkuInformation[0].Value;

        product.isOrderable = appService.checkOrderableStatusByBrand(product.SiteId, SBS.AccountAttribute('brand'));

        var isDecaf = $filter('filter')($scope.Products, { ProductID: 'Decaf_' + $scope.selected.ProductID })[0];

        if (isDecaf) {
            product.decafVersion = isDecaf.ProductID;

        } else {
            product.decafVersion = false;
        }

        product.items1 = $filter('filter')(product.ProductTraitValues, { Trait: 'Format' });
        product.selectedItem1 = {};
    }

    $scope.resetFilters = function () {
        angular.forEach($scope.filters, function (filter) {
            filter.isSelected = false;
        });
    }

    $scope.$watch("selected.selectedItem1.selectedItem", function () {
        if ($scope.selected != undefined) {
            var itemsTemp = $filter('filter')($scope.selected.SkuInformation, { Trait: 'Size' });
            $scope.selected.selectedItem1.items2 = itemsTemp;
        }
    }, true);

    $scope.removeFilter = function (filter) {
        filter.isSelected = false;
    };

    $scope.convertBrandAcronym = function (site) {
        if (site === 'SBX') {
            return 'Starbucks';
        }
        else if (site === 'SBC') {
            return 'Seattle\'s Best';
        }
        else if (site === 'SBXOCS') {
            return 'Starbucks Office Coffee';
        }
    }

    var promiseProductsGet = appService.getProducts(); //The MEthod Call from service
    promiseProductsGet.then(function (pl) {
        $scope.Products = isTeaProductFilter(pl.data.allProducts);
        $scope.selected = {};
    }, function (errorPl) {
        $log.error('failure loading coffee', errorPl);
    });
});

angular.module("product_lookup").controller('fontanaCatalogController', function ($scope, appService, isFontanaProductFilter, $filter, $log) {
    $scope.Products = [];

    $scope.filters = [
           {
               label: "Syrup",
               value: "Syrup",
               category: "type",
               type: "product"
           },
           {
               label: "Sauce",
               value: "Sauce",
               category: "type",
               type: "product"
           },
           {
               label: "Blended Beverage Bases",
               value: "Beverage Bases",
               category: "type",
               type: "product"
           },
           {
               label: "Mocha",
               value: "Mocha",
               category: "flavor",
               type: "product"
           },
           {
               label: "Caramel",
               value: "Caramel",
               category: "flavor",
               type: "product"
           }, {
               label: "Vanilla",
               value: "Vanilla",
               category: "flavor",
               type: "product"
           }, {
               label: "Chocolate",
               value: "Chocolate",
               category: "flavor",
               type: "product"
           }, {
               label: "Other",
               value: "Other",
               category: "flavor",
               type: "product"
           },
    ];

    $scope.$watch("filters", function (n, o) {
        var selected = $filter("filter")(n, { isSelected: true });

        var filters = $('.filter div');
        filters.filter('.active').removeClass('active');
        for (var s in selected) {
            for (f = 0; f < filters.length; f++) {
                var text = $.trim(filters.eq(f).text());
                if (text === selected[s].label) {
                    filters.eq(f).addClass('active');
                    break;
                }
            }
        }
    }, true);

    $scope.select = function (product) {
        $scope.selected = product;

        product.isOrderable = appService.checkOrderableStatusByBrand(product.SiteId, SBS.AccountAttribute('brand'));

        var isSugarfree = $filter('filter')($scope.Products, { ProductID: 'Sugar-Free_' + $scope.selected.ProductID })[0];

        if (isSugarfree) {
            product.sugarfreeVersion = isSugarfree.ProductID;
        }

        product.items1 = $filter('filter')($scope.selected.SkuInformation, { Trait: 'Size' });
        product.selectedItem1 = {};
    }

    $scope.resetFilters = function () {
        angular.forEach($scope.filters, function (filter) {
            filter.isSelected = false;
        });
    }

    $scope.removeFilter = function (filter) {
        filter.isSelected = false;
    };

    $scope.convertBrandAcronym = function (site) {
        if (site === 'SBX') {
            return 'Starbucks';
        }
        else if (site === 'SBC') {
            return 'Seattle\'s Best';
        }
        else if (site === 'SBXOCS') {
            return 'Starbucks Office Coffee';
        }
    }

    var promiseProductsGet = appService.getProducts(); //The MEthod Call from service
    promiseProductsGet.then(function (pl) {
        $scope.Products = isFontanaProductFilter(pl.data.allProducts);
        $scope.selected = {};
    }, function (errorPl) {
        $log.error('failure loading coffee', errorPl);
    });
});

angular.module("product_lookup").controller('coffeeDetailsController', function ($scope, appService, $filter, filterFilter) {
    $scope.IsNewRecord = 1; //The flag for the new record

    loadRecords();
    function loadRecords() {
        var pathArray = window.location.pathname.split('/');
        var prodId = pathArray[4];
        var promiseProductsGet = appService.getProductsWithId(prodId); //The MEthod Call from service
        promiseProductsGet.then(function (pl) {

            $scope.Products = pl.data.Data.allProducts;
            var result = $filter('filter')($scope.Products, { ProductID: prodId })[0];
            $scope.product = result;

            $scope.product.isOrderable = appService.checkOrderableStatusByBrand($scope.product.SiteId, SBS.AccountAttribute('brand'));

            var coffeeType = $filter('filter')(result.ProductTraitValues, { Trait: 'Type' })[0];
            $scope.product.coffeeType = coffeeType;
            var arrayOfLikeItems = new Array();
            $scope.product.relatedProducts = pl.data.Data.relatedProducts;

            var roastProfileIndicator = $filter('filter')(result.ProductTraitValues, { Trait: 'Roast Profile' })[0];
            if (roastProfileIndicator) {
                $scope.product.roastProfile = roastProfileIndicator.Value.replace(' Roast', '').toLowerCase();
            }

            var kindIndicator = $filter('filter')(result.ProductTraitValues, { Trait: 'Kind' });
            if (kindIndicator) {
                for (var i = 0; i < kindIndicator.length; i++) {
                    if (kindIndicator[i].Value === "Drip") {
                        $scope.product.isDrip = kindIndicator[i].Value;
                    }
                    if (kindIndicator[i].Value === "Iced") {
                        $scope.product.isIced = kindIndicator[i].Value;
                    }
                    if (kindIndicator[i].Value === "Instant") {
                        $scope.product.isInstant = kindIndicator[i].Value;
                    }
                }
            }

            var tasteIndicator = $filter('filter')(result.ProductTraitValues, { Trait: 'Blend Description' })[0];
            if (tasteIndicator) {
                $scope.product.tastingNotes = tasteIndicator.Value;
            }

            var pairsIndicator = $filter('filter')(result.ProductTraitValues, { Trait: 'Pairs Well With' })[0];
            if (pairsIndicator) {
                $scope.product.pairsWith = pairsIndicator.Value;
            }

            var regionIndicator = $filter('filter')(result.ProductTraitValues, { Trait: 'Region' })[0];
            if (regionIndicator) {
                $scope.product.region = regionIndicator.Value;
            }

            var recommendationIndicator = $filter('filter')(result.ProductTraitValues, { Trait: 'Recommendation' })[0];
            if (recommendationIndicator) {
                $scope.product.recommendation = recommendationIndicator.Value;
            }

            var certificationFilter = $filter('filter')(result.ProductTraitValues, { Trait: 'Certification' })[0];
            if (certificationFilter) {
                if (certificationFilter.Value === "Organic") {
                    $scope.product.organic = certificationFilter.Value;
                } else if (certificationFilter.Value === "Fairtrade") {
                    $scope.product.fairtrade = certificationFilter.Value;
                }
            }

            var isDecaf = $filter('filter')($scope.Products, { ProductID: 'Decaf_' + result.ProductID })[0];
            if (isDecaf) {
                $scope.product.decafVersion = isDecaf.ProductID;

            }

            $scope.product.items1 = $filter('filter')(result.SkuInformation, { Trait: 'Form' });
            $scope.product.selectedItem1 = {};

            $scope.product.isCoffee = appService.checkCoffeeStatus(result.SkuInformation, 'Form');
            
            var instantFilter = $filter('filter')(result.ProductTraitValues, { Trait: 'Kind' })[0];
            if (instantFilter) {
                if (instantFilter.Value === "Instant") {
                    $scope.product.isInstant = 1;
                }
            }
        });
    }


    $scope.$watch("product.selectedItem1.selectedItem", function () {
        if ($scope.product != undefined) {
            var itemsTemp = $filter('filter')($scope.product.SkuInformation, { Trait: 'Form' });
            $scope.product.selectedItem1.items2 = $filter('filter')(itemsTemp, { Value: $scope.product.selectedItem1.selectedItem.Value });

            var itemsTempSizes = $filter('filter')($scope.product.SkuInformation, { Trait: 'Size' });
            var itemTempSizeItem = $filter('filter')(itemsTempSizes, { SKUNumber: $scope.product.selectedItem1.selectedItem.SKUNumber })[0];
            if (itemTempSizeItem) {
                $scope.product.selectedItem1.Size = itemTempSizeItem.Value;
            }
        }
    }, true);

    $scope.$watch("product.selectedItem1.selectedItem2", function () {
        if ($scope.product != undefined) {
            var itemsTemp = $filter('filter')($scope.product.SkuInformation, { Trait: 'Brewer Type' });
            $scope.product.selectedItem1.selectedItem2.items3 = $filter('filter')(itemsTemp, { SKUNumber: $scope.product.selectedItem1.selectedItem2.SKUNumber });
            //$scope.product.selectedItem1.selectedItem2.selectedItem3 = $scope.product.selectedItem1.selectedItem2.items3[0];
        }
    }, true);

    $scope.convertBrandAcronym = function (site) {
        if (site === 'SBX') {
            return 'Starbucks';
        }
        else if (site === 'SBC') {
            return 'Seattle\'s Best';
        }
        else if (site === 'SBXOCS') {
            return 'Starbucks Office Coffee';
        }
    }

});

angular.module("product_lookup").controller('productDetailsController', function ($scope, appService, $filter, filterFilter) {
    $scope.IsNewRecord = 1; //The flag for the new record

    loadRecords();
    function loadRecords() {
        var pathArray = window.location.pathname.split('/');
        var prodId = pathArray[3];


        var promiseProductsGet = appService.getProducts();
        promiseProductsGet.then(function (pl) {
            $scope.Products = pl.data.allProducts;
            var promiseProductGet= appService.getProductsWithId(prodId);
            

            promiseProductGet.then(function (pl) {
                var productResult = pl.data.Data.allProducts;
                var result = $filter('filter')(productResult, { ProductID: prodId })[0];
                $scope.product = result;

                var kindIndicator = $filter('filter')(result.ProductTraitValues, { Trait: 'Kind' });
                if (kindIndicator.length > 0) {
                    for (var i = 0; i < kindIndicator.length; i++) {
                        if (kindIndicator[i].Value === 'Drip') {
                            $scope.product.isDrip = kindIndicator[i].Value;
                        }
                        if (kindIndicator[i].Value === 'Iced') {
                            $scope.product.isIced = kindIndicator[i].Value;
                        }
                        if (kindIndicator[i].Value === 'Instant') {
                            $scope.product.isInstant = kindIndicator[i].Value;
                        }
                    }
                }

                var frappIndicator = $filter('filter')(result.ProductTraitValues, { TypeID: 'Frappuccino' })[0];
                if (frappIndicator) {
                    $scope.product.isFrapp = true;
                    $scope.product.isOrderable = (SBS.AccountAttribute('is-frap') == true);
                } else {
                    $scope.product.isOrderable = appService.checkOrderableStatusByBrand($scope.product.SiteId, SBS.AccountAttribute('brand'));
                }

                $scope.product.isOrderable = appService.checkOrderableStatusByBrand($scope.product.SiteId, SBS.AccountAttribute('brand'));

                var frappOnlyIndicator = $filter('filter')(result.ProductTraitValues, { Trait: 'Solution' })[0];
                if (frappOnlyIndicator) {
                    if (frappOnlyIndicator.Value === "Frappuccino") {
                        $scope.product.isOrderable = (SBS.AccountAttribute('is-frap') == true);
                    }
                }
                               


                if (result.SiteId === 'SBC') {
                    $scope.product.isSBC = true;
                }

                $scope.product.items1 = $filter('filter')(result.SkuInformation, { Trait: 'Size' });

                var coffeeIndicator = (
                    $filter('filter')(result.ProductTraitValues, { TypeID: 'Iced_Coffee' })[0] ||
                    $filter('filter')(result.ProductTraitValues, { TypeID: 'Espresso' })[0] ||
                    $filter('filter')(result.ProductTraitValues, { TypeID: 'Instant_Coffee' })[0] ||
                    $filter('filter')(result.ProductTraitValues, { TypeID: 'Drip_Brewed' })[0]);

                if (coffeeIndicator) {
                    $scope.product.isCoffee = true;

                    var tasteIndicator = $filter('filter')(result.ProductTraitValues, { Trait: 'Blend Description' });
                    if (tasteIndicator) {
                        $scope.product.tastingNotes = tasteIndicator.Value;
                    }

                    var pairsIndicator = $filter('filter')(result.ProductTraitValues, { Trait: 'Pairs Well With' });
                    if (pairsIndicator) {
                        $scope.product.pairsWith = pairsIndicator.Value;
                    }

                    var regionIndicator = $filter('filter')(result.ProductTraitValues, { Trait: 'Region' });
                    if (regionIndicator) {
                        $scope.product.region = regionIndicator.Value;
                    }

                    var recommendationIndicator = $filter('filter')(result.ProductTraitValues, { Trait: 'Recommendation' });
                    if (recommendationIndicator) {
                        $scope.product.recommendation = recommendationIndicator.Value;
                    }

                    var certificationFilter = $filter('filter')(result.ProductTraitValues, { Trait: 'Certification' });
                    if (certificationFilter) {
                        if (certificationFilter.Value === "Organic") {
                            $scope.product.organic = certificationFilter.Value;
                        } else if (certificationFilter.Value === "Fairtrade") {
                            $scope.product.fairtrade = certificationFilter.Value;
                        }
                    }

                    $scope.product.items1 = $filter('filter')(result.ProductTraitValues, { Trait: 'Format' });

                    var isDecaf = $filter('filter')($scope.Products, { TypeID: 'Decaf_' + result.ProductID })[0];
                    if (isDecaf) {
                        $scope.product.decafVersion = isDecaf.ProductID;
                    }
                } else {
                    var tazoIndicator = $filter('filter')(result.ProductTraitValues, { TypeID: 'Tazo_Tea' })[0] || $filter('filter')(result.ProductTraitValues, { TypeID: 'Tazo_Chai' })[0];
                    if (tazoIndicator) {
                        $scope.product.isTazo = true;
                        $scope.product.items1 = $filter('filter')(result.ProductTraitValues, { Trait: 'Format' });

                        var isDecaf = $filter('filter')($scope.Products, { TypeID: 'Decaf_' + result.ProductID })[0];
                        if (isDecaf) {
                            $scope.product.decafVersion = isDecaf.ProductID;
                        }
                        var formatIndicator = $filter('filter')(result.ProductTraitValues, { Trait: 'Format' })[0];
                        if (formatIndicator) {
                            if (formatIndicator.Value === "Filterbag") {
                                $scope.product.isFilterbag = true;
                            }
                            if (formatIndicator.Value.toLowerCase().indexOf("bulk iced tea") > -1) {
                                $scope.product.isBulkIcedTea = true;
                            }
                        }
                    } else {
                        var unsortedSizes = $filter('filter')(result.SkuInformation, { Trait: 'Size' });
                        if (unsortedSizes) {
                            var sortedSizes = [];
                            for (var x = 0; x < unsortedSizes.length; x++) {
                                sortedSizes.push({ rank: parseInt(unsortedSizes[x].Value), skuItem: unsortedSizes[x] });
                            }

                            var productSizes = sortedSizes.sort(function (a, b) {
                                return a.rank - b.rank;
                            });

                            var sortedSkus = [];
                            for (var x = 0; x < productSizes.length; x++) {
                                sortedSkus.push(productSizes[x].skuItem);
                            }

                            $scope.product.items1 = sortedSkus;
                        }
                    }

                    var fontanaIndicator = $filter('filter')(result.ProductTraitValues, { TypeID: 'Fontana_Syrups_And_Sauces' })[0];
                    if (fontanaIndicator) {
                        $scope.product.isFontana = true;
                        var isSugarfree = $filter('filter')($scope.Products, { ProductID: 'Sugar-Free_' + result.ProductID })[0];

                        if (isSugarfree) {
                            $scope.product.sugarfreeVersion = isSugarfree.ProductID;
                        }
                    }


                    var suppliesIndicator = $filter('filter')(result.ProductTraitValues, { TypeID: 'Supplies' })[0];
                    if (suppliesIndicator) {
                        $scope.product.isSupplies = true;
                    }

                    var isMiscellaneous = $filter('filter')(result.ProductTraitValues, { TypeID: 'Ingredients_Merchandise' })[0];
                    if (isMiscellaneous){
                        $scope.product.isMiscellaneous = true;
                    }
                }

                $scope.product.selectedItem1 = {};
                console.log($scope.product);
            });
        });
    }

    
    $scope.$watch("product.selectedItem1.selectedItem", function () {
        if ($scope.product != undefined) {
            var unsortedSizes = $filter('filter')($scope.product.SkuInformation, { Trait: 'Size' });
            if (unsortedSizes) {
                var sortedSizes = [];
                for (var x = 0; x < unsortedSizes.length; x++) {
                    sortedSizes.push({ rank: parseInt(unsortedSizes[x].Value), skuItem: unsortedSizes[x] });
                }

                var productSizes = sortedSizes.sort(function (a, b) {
                    return a.rank - b.rank;
                });

                var sortedSkus = [];
                for (var x = 0; x < productSizes.length; x++) {
                    sortedSkus.push(productSizes[x].skuItem);
                }

                $scope.product.selectedItem1.items2 = sortedSkus;
            }
        }
    }, true);

    $scope.$watch("selected.selectedItem1.selectedItem", function () {
        if ($scope.selected != undefined) {
            var itemsTemp = $filter('filter')($scope.selected.SkuInformation, { Trait: 'Size' });
            $scope.selected.selectedItem1.items2 = itemsTemp;
        }
    }, true);

    $scope.convertBrandAcronym = function (site) {
        if (site === 'SBX') {
            return 'Starbucks';
        }
        else if (site === 'SBC') {
            return 'Seattle\'s Best';
        }
        else if (site === 'SBXOCS') {
            return 'Starbucks Office Coffee';
        }
    }

});

angular.module("product_lookup").controller('recipeCatalogController', function ($scope, appService, recipeCategoryFilter, $filter, $log) {
    $scope.initialize = function (category) {
        $scope.CurrentCategory = category;

        if (category != "Frozen_Blended_Smoothies") {
            $scope.filters.push(
                {
                    label: "Tazo Tea",
                    value: "Tazo_Tea",
                    category: "brand",
                    type: "recipe"
                });
        }
    };

    $scope.filters =[
        {
            label: "Starbucks",
            value: "SBX",
            category : "brand",
            type: "recipe"
        },
        {
            label: "Seattle's Best",
            value: "SBC",
            category: "brand",
            type: "recipe"
        },
        {
            label: "Coffee",
            value: "Coffee",
            category: "ingredient",
            type: "recipe"
        },
        {
            label: "Non-Coffee",
            value : "non-coffee",
            category: "ingredient",
            type: "recipe"
        },
        {
            label: "Mocha",
            value: "Mocha",
            category: "flavor",
            type: "recipe"
        },
        {
            label: "Caramel",
            value: "Caramel",
            category: "flavor",
            type: "recipe"
        },
        {
            label: "Vanilla",
            value: "Vanilla",
            category: "flavor",
            type : "recipe"
        },
        {
            label: "White Chocolate",
            value: "White Chocolate",
            category : "flavor",
            type: "recipe"
        },
        {
            label: "Other",
            value: "Other",
            category: "flavor",
            type: "recipe"
        }
    ];

    $scope.resetFilters = function () {
        angular.forEach($scope.filters, function (filter) {
            filter.isSelected = false;
        });
    }

    $scope.$watch("filters", function (n, o) {
        var selected = $filter("filter") (n, {
            isSelected: true
        });

        var filters = $('.filter div');
        filters.filter('.active').removeClass('active');
        for (var s in selected) {
            for (f = 0; f < filters.length; f++) {
                    var text = $.trim(filters.eq(f).text());
                if (text === selected[s].label) {
                        filters.eq(f).addClass('active');
                    break;
                }
            }
        }
    }, true);

    var promiseRecipesGet = appService.getRecipes();
        promiseRecipesGet.then(function (pl) {
            $scope.Recipes = recipeCategoryFilter(pl.data.allRecipes, $scope.CurrentCategory);
    }, function (errorPl) {
        $log.error('failure loading recipes', errorPl);
            });

        $scope.removeFilter = function (filter) {
        filter.isSelected = false;
    };
});

angular.module("product_lookup").controller('recipeDetailsController', function ($scope, appService, $filter) {

    $scope.IsNewRecord = 1; //The flag for the new record

    loadRecords();

    function loadRecords() {
        var pathArray = window.location.pathname.split('/');
        var recipeIdentifier = pathArray[4];
        var siteId = recipeIdentifier.substring(0, 3);
        var recipeId = recipeIdentifier.substring(4, recipeIdentifier.length);

        if (siteId != 'SBX' && siteId != 'SBC') {
            siteId = 'SBX';
            recipeId = pathArray[4];
        }
        

        var promiseRecipesGet = appService.getRecipeWithId(recipeId, siteId);

        promiseRecipesGet.then(function (pl) {

            $scope.Recipe = pl.data.Data.recipeWithId;

            $scope.Recipe.isSBX = (siteId == 'SBX');
            $scope.Recipe.isSBC = (siteId == 'SBC');

            for (var cat in $scope.Recipe.TypeID) {
                if (/Hot_Beverages/.test($scope.Recipe.TypeID[cat])) {
                    $scope.Recipe.isHotBeverages = true;
                    break;
                }
                else if (/Cold_Beverages/.test($scope.Recipe.TypeID[cat])) {
                    $scope.Recipe.isColdBeverages = true;
                    break;
                }
                else if (/Frozen_Blended_Smoothies/.test($scope.Recipe.TypeID[cat])) {
                    $scope.Recipe.isFrozenBeverages = true;
                    break;
                }
            }

            for (var cat in $scope.Recipe.TypeID) {
                if (/Tazo_Tea/.test($scope.Recipe.TypeID[cat])) {
                    $scope.Recipe.isTazo = true;
                    break;
                }
            }

            $scope.Recipe.PrepSteps = $scope.Recipe.PreparationSteps.split("\r\n");

            $scope.RelatedRecipes = pl.data.Data.relatedRecipes;
            var relatedProducts = pl.data.Data.relatedProducts;

            var unsortedSizes = $filter('filter')($scope.Recipe.RecipeIngredients, { SizeName: '!All' });

            var sortedSizes = [];
            for (var x = 0; x < unsortedSizes.length; x++) {
                sortedSizes.push({ rank: parseInt(unsortedSizes[x].SizeName), SizeName: unsortedSizes[x].SizeName });
            }

            $scope.recipeSizes = sortedSizes.sort(function (a, b) {
                return a.rank - b.rank;
            });

            $scope.selectedSize = $scope.Recipe.RecipeIngredients[0].SizeName;

            for (var i = 0; i < relatedProducts.length; i++) {
                relatedProducts[i].items1 = $filter('filter') (relatedProducts[i].SkuInformation, {Trait: 'Form'});
                relatedProducts[i].selectedItem1 = {};
                relatedProducts[i].selectedItem1.selectedItem = relatedProducts[i].items1[0];
                relatedProducts[i].IsCoffee = appService.checkCoffeeStatus(relatedProducts[i].SkuInformation, 'Form');
                if (relatedProducts[i].IsCoffee) {
                    loadSelections(relatedProducts[i]);
                }
                //relatedProducts[i].isOrderable = appService.checkOrderableStatusByBrand(relatedProducts[i].SiteId, SBS.AccountAttribute('brand'));
            }

            $scope.RelatedProducts = relatedProducts;

            $scope.selectIngredient($scope.Recipe, true);
            console.log($scope.Recipe);

        });

        $scope.selectIngredient = function (recipe, doNotShow) {
            if(recipe.SizeName == 'All') {
                $scope.IngredientToSize = null;
            }
            else {
                $scope.IngredientToSize = $filter('filter') ($scope.Recipe.RecipeIngredients, {
                        SizeName: recipe.SizeName
            });
            }

            if (!doNotShow)
                $scope.IngredientForAll = $filter('filter')($scope.Recipe.RecipeIngredients, { SizeName: 'All' });            
        }
    }

    function loadSelections(product) {
        if (product != undefined) {
            var itemsTemp = $filter('filter')(product.SkuInformation, { Trait: 'Form' });
            product.selectedItem1.items2 = $filter('filter')(itemsTemp, { Value: product.selectedItem1.selectedItem.Value });
            //$scope.items2 = $filter('filter')(itemsForm, { Value: $scope.selectedItem1.selectedItem.Value });
            //product.selectedItem1.selectedItem2 = product.selectedItem1.items2[0];

            itemsTemp = $filter('filter')(product.SkuInformation, { Trait: 'Brewer Type' });
            for (var i = 0; i < product.selectedItem1.items2.length; i++) {
                product.selectedItem1.items2[i].items3 = $filter('filter')(itemsTemp, { SKUNumber: product.selectedItem1.items2[i].SKUNumber });
                //product.selectedItem1.items2[i].selectedItem3 = product.selectedItem1.items2[i].items3[0];
            }
        }
    }

    $scope.LoadSelections = function (product) {
        loadSelections(product);
    }

    $scope.convertBrandAcronym = function (site) {
        if (site === 'SBX') {
            return 'Starbucks';
        }
        else if (site === 'SBC') {
            return 'Seattle\'s Best';
        }
        else if (site === 'SBXOCS') {
            return 'Starbucks Office Coffee';
        }
    }
});

angular.module("product_lookup").controller('frappuccinoCatalogController', function ($scope, appService, isFrappuccinoProductFilter, $filter, $log) {
    $scope.Products = [];

    $scope.filters = [];

    $scope.select = function (product) {
        $scope.selected = product;
        //$scope.someModel = product.SkuInformation[0].Value;

        product.isOrderable = SBS.AccountAttribute('is-frap');

        var isDecaf = $filter('filter')($scope.Products, { ProductID: 'Decaf_' + $scope.selected.ProductID })[0];

        if (isDecaf) {
            product.decafVersion = isDecaf.ProductID;

        } else {
            product.decafVersion = false;
        }

        product.items1 = $filter('filter')(product.ProductTraitValues, { Trait: 'Format' });
        product.selectedItem1 = {};
        //product.selectedItem1.selectedItem = product.items1[0];
        //$scope.$apply();
    }

    $scope.$watch("selected.selectedItem1.selectedItem", function () {
        if ($scope.selected != undefined) {
            var itemsTemp = $filter('filter')($scope.selected.SkuInformation, { Trait: 'Size' });
            $scope.selected.selectedItem1.items2 = itemsTemp;
            //$scope.selected.selectedItem1.items2 = $filter('filter')(itemsTemp, { Value: $scope.selected.selectedItem1.selectedItem.Value });
            //$scope.items2 = $filter('filter')(itemsForm, { Value: $scope.selectedItem1.selectedItem.Value });
            //$scope.selected.selectedItem1.selectedItem2 = $scope.selected.selectedItem1.items2[0];
        }
    }, true);

    $scope.removeFilter = function (filter) {
        filter.isSelected = false;
    };

    $scope.convertBrandAcronym = function (site) {
        if (site === 'SBX') {
            return 'Starbucks';
        }
        else if (site === 'SBC') {
            return 'Seattle\'s Best';
        }
        else if (site === 'SBXOCS') {
            return 'Starbucks Office Coffee';
        }
    }

    var promiseProductsGet = appService.getProducts(); //The MEthod Call from service
    promiseProductsGet.then(function (pl) {
        $scope.Products = isFrappuccinoProductFilter(pl.data.allProducts);
        $scope.selected = {};
    }, function (errorPl) {
        $log.error('failure loading coffee', errorPl);
    });
});

angular.module("product_lookup").controller('suppliesCatalogController', function ($scope, appService, isSupplyProductFilter, $filter, $log) {
    $scope.Products = [];

    $scope.filters = [
            {
                label: "Starbucks",
                value: "SBX",
                category: "brand",
                type: "product"
            },
            {
                label: "Seattle's Best",
                value: "SBC",
                category: "brand",
                type: "product"
            },
            {
                label: "Tazo",
                value: "Tazo",
                category: "brand",
                type: "product"
            },
            {
                label: "Fontana",
                value: "SBC",
                category: "brand",
                type: "product"
            }
           ,
           {
               label: "Serveware",
               value: "Serveware",
               category: "type",
               type: "product"
           },
           {
               label: "Supplies",
               value: "Supplies",
               category: "type",
               type: "product"
           }
           ,
           {
               label: "Cleaning",
               value: "Cleaning",
               category: "type",
               type: "product"
           }
    ];

    $scope.$watch("filters", function (n, o) {
        //for (var i = 0; i < $scope.filters.length; i++) {
        //    if (n.label == $scope.filters[i].label) {
        //        var selected = selected + $filter("filter")($scope.filters[i], { isSelected: true });
        //    }
        //}
        var selected = $filter("filter")(n, { isSelected: true });

        var filters = $('.filter div');
        filters.filter('.active').removeClass('active');
        for (var s in selected) {
            for (f = 0; f < filters.length; f++) {
                var text = $.trim(filters.eq(f).text());
                if (text === selected[s].label) {
                    filters.eq(f).addClass('active');
                    break;
                }
            }
        }
    }, true);

    $scope.select = function (product) {
        $scope.selected = product;
        //$scope.someModel = product.SkuInformation[0].Value;

        product.isOrderable = appService.checkOrderableStatusByBrand(product.SiteId, SBS.AccountAttribute('brand'));

        var isDecaf = $filter('filter')($scope.Products, { ProductID: 'Decaf_' + $scope.selected.ProductID })[0];

        if (isDecaf) {
            product.decafVersion = isDecaf.ProductID;

        } else {
            product.decafVersion = false;
        }

        var unsortedSizes = $filter('filter')($scope.selected.SkuInformation, { Trait: 'Size' });
        if (unsortedSizes) {
            var sortedSizes = [];
            for (var x = 0; x < unsortedSizes.length; x++) {
                sortedSizes.push({ rank: parseInt(unsortedSizes[x].Value), skuItem: unsortedSizes[x] });
            }

            var productSizes = sortedSizes.sort(function (a, b) {
                return a.rank - b.rank;
            });

            var sortedSkus = [];
            for (var x = 0; x < productSizes.length; x++) {
                sortedSkus.push(productSizes[x].skuItem);
            }

            product.items1 = sortedSkus;
        }

        product.selectedItem1 = {};
        //product.selectedItem1.selectedItem = product.items1[0];
        //$scope.$apply();
    }

    $scope.resetFilters = function () {
        angular.forEach($scope.filters, function (filter) {
            filter.isSelected = false;
        });
    }

    $scope.$watch("selected.selectedItem1.selectedItem", function () {
        if ($scope.selected != undefined) {
            var itemsTemp = $filter('filter')($scope.selected.SkuInformation, { Trait: 'Size' });
            $scope.selected.selectedItem1.items2 = itemsTemp;
            //$scope.selected.selectedItem1.items2 = $filter('filter')(itemsTemp, { Value: $scope.selected.selectedItem1.selectedItem.Value });
            //$scope.items2 = $filter('filter')(itemsForm, { Value: $scope.selectedItem1.selectedItem.Value });
            //$scope.selected.selectedItem1.selectedItem2 = $scope.selected.selectedItem1.items2[0];
        }
    }, true);

    $scope.removeFilter = function (filter) {
        filter.isSelected = false;
    };

    $scope.convertBrandAcronym = function (site) {
        if (site === 'SBX') {
            return 'Starbucks';
        }
        else if (site === 'SBC') {
            return 'Seattle\'s Best';
        }
        else if (site === 'SBXOCS') {
            return 'Starbucks Office Coffee';
        }
    }

    var promiseProductsGet = appService.getProducts(); //The MEthod Call from service
    promiseProductsGet.then(function (pl) {
        $scope.Products = isSupplyProductFilter(pl.data.allProducts);
        $scope.selected = {};
    }, function (errorPl) {
        $log.error('failure loading coffee', errorPl);
    });
});

angular.module("product_lookup").controller('miscellaneousCatalogController', function ($scope, appService, isMiscellaneousProductFilter, $filter, $log) {
    $scope.Products = [];

    $scope.filters = [
            {
                label: "Ingredients",
                value: "Beverage Ingredients",
                category: "type",
                type: "product"
            },
            {
                label: "Merchandise",
                value: "Resale Merchandise",
                category: "type",
                type: "product"
            }
    ];

    $scope.$watch("filters", function (n, o) {
        //for (var i = 0; i < $scope.filters.length; i++) {
        //    if (n.label == $scope.filters[i].label) {
        //        var selected = selected + $filter("filter")($scope.filters[i], { isSelected: true });
        //    }
        //}
        var selected = $filter("filter")(n, { isSelected: true });

        var filters = $('.filter div');
        filters.filter('.active').removeClass('active');
        for (var s in selected) {
            for (f = 0; f < filters.length; f++) {
                var text = $.trim(filters.eq(f).text());
                if (text === selected[s].label) {
                    filters.eq(f).addClass('active');
                    break;
                }
            }
        }
    }, true);

    $scope.select = function (product) {
        $scope.selected = product;
        //$scope.someModel = product.SkuInformation[0].Value;

        product.isOrderable = appService.checkOrderableStatusByBrand(product.SiteId, SBS.AccountAttribute('brand'));

        var isDecaf = $filter('filter')($scope.Products, { ProductID: 'Decaf_' + $scope.selected.ProductID })[0];

        if (isDecaf) {
            product.decafVersion = isDecaf.ProductID;

        } else {
            product.decafVersion = false;
        }

        product.items1 = $filter('filter')(product.SkuInformation, { Trait: 'Size' });
        product.selectedItem1 = {};
    }

    $scope.resetFilters = function () {
        angular.forEach($scope.filters, function (filter) {
            filter.isSelected = false;
        });
    }

    $scope.$watch("selected.selectedItem1.selectedItem", function () {
        if ($scope.selected != undefined) {
            var itemsTemp = $filter('filter')($scope.selected.SkuInformation, { Trait: 'Size' });
            $scope.selected.selectedItem1.items2 = itemsTemp;
        }
    }, true);

    $scope.removeFilter = function (filter) {
        filter.isSelected = false;
    };

    $scope.convertBrandAcronym = function (site) {
        if (site === 'SBX') {
            return 'Starbucks';
        }
        else if (site === 'SBC') {
            return 'Seattle\'s Best';
        }
        else if (site === 'SBXOCS') {
            return 'Starbucks Office Coffee';
        }
    }

    var promiseProductsGet = appService.getProducts(); //The MEthod Call from service
    promiseProductsGet.then(function (pl) {
        $scope.Products = isMiscellaneousProductFilter(pl.data.allProducts);
        $scope.selected = {};
    }, function (errorPl) {
        $log.error('failure loading coffee', errorPl);
    });
});