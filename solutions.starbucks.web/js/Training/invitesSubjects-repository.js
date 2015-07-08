courseBuilderModule.factory('invitesSubjectsRepository', function ($http, $q) {
    return {
        get: function(inviteSubjectId) {
            var deferred = $q.defer();
            $http.get('/umbraco/api/invitesSubjectsApi/index/' + inviteSubjectId).success(deferred.resolve).error(deferred.reject);
            return deferred.promise;
        },
        save: function (invite, selection) {
            var deferred = $q.defer();
            var postData = { Invite: invite, SubjectSelection: selection };
            $http.post('/umbraco/api/invitesSubjectsApi/Save', JSON.stringify(postData))
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