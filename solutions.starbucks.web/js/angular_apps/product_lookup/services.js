/// <reference path="lib/angular/angular.js" />
/// <reference path="Module.js" />


angular.module("product_lookup").service('appService', function ($http) {


    //Create new record
    //this.post = function (Employee) {
    //    var request = $http({
    //        method: "post",
    //        url: "/api/EmployeesAPI",
    //        data: Employee
    //    });
    //    return request;
    //}
    ////Get Single Records
    //this.get = function (EmpNo) {
    //    return $http.get("/api/EmployeesAPI/" + EmpNo);
    //}

    //Get All Products
    this.getProducts = function () {
        //console.log($http.get("/Umbraco/Api/ProductApi/GetAllProducts"));
        return $http.get("/Umbraco/Api/ProductApi/GetAllProducts");
    }

    this.getRecipes = function () {
        return $http.get("/Umbraco/Api/RecipeApi/GetAllRecipes");
    }

    this.getProductsWithId = function (productId) {
        return $http.get("/Umbraco/Api/ProductApi/GetProductWithId?productId=" + productId);
    }

    this.getRecipeWithId = function (recipeId, siteId) {
        return $http.get("/Umbraco/Api/RecipeApi/GetRecipeWithId?recipeId=" + recipeId +"&siteId=" + siteId);
    }

    this.getProductTypes = function () {

        //console.log($http.get("/Umbraco/Api/ProductApi/GetProductTypes"));
        return $http.get("/Umbraco/Api/ProductApi/GetProductTypes");
    }

    this.checkCoffeeStatus = function(array, id) {
        //console.log(array.length);
        for (var i = 0; i < array.length; i++) {
            if (array[i].Trait === id) {
                return true;
            }
        }
        return false;
    }

    this.checkOrderableStatusByBrand = function (productBrands, accountBrand) {
        
        //If this comes in as an array of strings, make it a single string
        if (!$.isArray(productBrands)) {
            productBrands = productBrands.split(" ");
        }

        $.each(productBrands, function (key, val) {
            productBrands[key] = $.trim(val);
        });

        if (accountBrand === "DUAL") {
            if ($.inArray("SBX", productBrands) > -1 || $.inArray("SBC", productBrands) > -1) {
                return true;
            }
        } else {
            if (accountBrand === 'SBUX') {
                return ($.inArray("SBX", productBrands) > -1);
            }
            if (accountBrand === 'SBC') {
                return ($.inArray("SBC", productBrands) > -1);
            }
        }
    }

    //Update the Record
    //this.put = function (EmpNo, Employee) {
    //    var request = $http({
    //        method: "put",
    //        url: "/api/EmployeesAPI/" + EmpNo,
    //        data: Employee
    //    });
    //    return request;
    //}
    //Delete the Record
    //this.delete = function (EmpNo) {
    //    var request = $http({
    //        method: "delete",
    //        url: "/api/EmployeesAPI/" + EmpNo
    //    });
    //    return request;
    //}
});