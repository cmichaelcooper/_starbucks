courseBuilderModule.controller("InvitesController", function ($scope, $window, invitesRepository, invitesSubjectsRepository, subjectIds, operatorUserId) {
    
    fillInvites();

    function fillInvites() {
        $scope.errorLoadingInvites = false;
        $scope.loadingInvites = true;
        invitesRepository.get(operatorUserId).then(
            function (invites) {
                $scope.invites = invites;
                $scope.loadingInvites = false;
            }, function () {
                $scope.errorLoadingInvites = true;
                $scope.loadingInvites = false;
            }
         );
    }

    function clearForm() {
        $scope.invite.traineeName = '';
        $scope.invite.traineeEmail = '';
        $scope.inviteForm.$setPristine();
    }

    
    $scope.delete = function (invite) {
        $scope.error = false;
        invitesRepository.delete(invite).then(
            function (invite) {
                fillInvites();
            }, function () {
                $scope.error = true;
            }
        );
    };

    $scope.confirmDelete = function (invite) {
        $scope.error = false;
        invite.AreYouSure = true;
    };

    $scope.cancelDelete = function (invite) {
        invite.AreYouSure = false;
    };

    $scope.delete = function (invite) {
        $scope.error = false;
        invite.AreYouSure = false;
        invitesRepository.delete(invite).then(
            function (invite) {
                fillInvites();                
            }, function () {
                $scope.error = true;
            }
        );

    };

    $scope.resetSubmitted = function () {
        $scope.submitted = false;
    };

    $scope.save = function (invite) {
        $scope.error = false;
        $scope.submitted = true;
        $scope.inviteSent = false;
        
        if ($scope.inviteForm.$invalid) {
            return false;
        }
        
        invitesRepository.save(invite).then(
            function (invite) {
                invitesSubjectsRepository.save(invite, $scope.selection).then(function (retVal) {                    
                    fillInvites();
                    clearForm();
                    sendInvite(invite);
                    $scope.submitted = false;
                }, function () { $scope.error = true; })
            }, function () {
                $scope.error = true;
                $scope.submitted = false;
            }
        );
        
    };

    $scope.toggleDrawer = function (invite, open, $index, $event) {

        var target = jQuery($event.target);
        var detailPanel = target.parent().parent().parent().find(".training-invites-report-record-drawer");
        detailPanel.slideToggle(.3875);
        
        invitesRepository.details(invite).then(
            function (inviteWithDetails) {
                
                $scope.invites[$index].InviteSubjects = inviteWithDetails.InviteSubjects;
                $scope.invites[$index].DetailsOpen = open;

            }, function () {
                $scope.error = true;
            }
        );
    };

    $scope.toggleCourseBuilderSection = function ($event) {
        var target = jQuery($event.target);
        var courseBuilderSection = target.next(".training-select-expander");
        courseBuilderSection.slideToggle(.3875);
    }

    $scope.toggleProgressDrawer = function (open, $index, $event) {
        
        $scope.invites[$index].ProgressOpen = open;

        var target = jQuery($event.target);
        var progressReportPanel = target.parent().parent().parent().find(".training-progress-report");
        progressReportPanel.slideToggle(.3875, function () {
            // Animation complete.
        });

    };

    function sendInvite (invite) {
        $scope.inviteSent = false;
        $scope.inviteSendError = false;
        $scope.sendingInvite = true;
        invitesRepository.send(invite).then(
            function (inviteSent) {
                $scope.sendingInvite = false;
                $scope.inviteSent = true;
            }, function () {
                $scope.inviteSendError = true;
                $scope.sendingInvite = false;
            }
        );
    };

   
    $scope.resend = function (invite, index) {
        invite.Resending = true;
        $scope.inviteSent = false;
        $scope.inviteSendError = false;
        invitesRepository.send(invite).then(
            function (resentInvite) {
                invite.Resending = false;
                $scope.inviteSent = true;
                $scope.invites[index] = resentInvite;
            }, function () {
                $scope.inviteSendError = true;
                invite.Resending = false;
            }
        );
    };
               
    // selected Subjects
    $scope.selection = []; //TODO: default all to selected? subjectIds; //

    // toggle selection for a subject
    $scope.toggleSelection = function toggleSelection(subjectId) {
        var idx = $scope.selection.indexOf(subjectId);

        // is currently selected
        if (idx > -1) {
            $scope.selection.splice(idx, 1);
        }
        // is newly selected
        else {
            $scope.selection.push(subjectId);
        }
    };

}); 