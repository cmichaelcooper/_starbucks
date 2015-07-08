courseBuilderModule.factory('invitesRepository', function ($http, $q) {

    return {
        get: function (operatorUserId) {
            var deferred = $q.defer();
            $http.get('/umbraco/api/invitesApi/GetByOperator/' + operatorUserId).success(deferred.resolve).error(deferred.reject);
            return deferred.promise;
        },
        details: function (invite) {
            var deferred = $q.defer();
            $http.post('/umbraco/api/invitesApi/GetDetails', invite)
                .success(function (data, status, headers, config) {
                    deferred.resolve(data);
                })
                .error(function () {
                    deferred.reject();
                });
            return deferred.promise;
        },
        save: function (invite) {
            var deferred = $q.defer();
            $http.post('/umbraco/api/invitesApi/Save', invite)
                .success(function (data, status, headers, config) {
                    deferred.resolve(data);
                })
                .error(function () {
                    deferred.reject();
                });
            return deferred.promise;            
        },
        delete: function (invite) {
            var deferred = $q.defer();
            $http.post('/umbraco/api/invitesApi/Delete', JSON.stringify(invite))
                .success(function (data, status, headers, config) {
                    deferred.resolve(data);
                })
                .error(function (xhr, status, p3, p4) {
                    var err = "Error " + " " + status + " " + p3;
                    if (xhr.responseText && xhr.responseText[0] == "{")
                        err = JSON.parse(xhr.responseText).message;
                    alert(err);

                    deferred.reject();
                });
            return deferred.promise;
        },
        send: function (invite) {
            
            var deferred = $q.defer();
            $http.post('/umbraco/api/invitesApi/Send', invite)
                .success(function (data, status, headers, config) {
                    deferred.resolve(data);
                })
                .error(function () {
                    deferred.reject();
                });            
            return deferred.promise;                         
        }
    }

 

});