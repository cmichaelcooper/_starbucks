
angular.module("product_lookup", ['ngSanitize', 'angular.filter', 'ngRoute', 'common'])

angular.module("product_lookup").config(function ($routeProvider) {
    $routeProvider.when(
        "/coffee", {
            templateUrl: "/our-brand"
        }).otherwise({
            redirectTo: '/phones'
        });
});

