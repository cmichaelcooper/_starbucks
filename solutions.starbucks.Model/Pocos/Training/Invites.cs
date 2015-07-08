using solutions.starbucks.Model.Pocos.Training;
using solutions.starbucks.Model.Training;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Persistence;

namespace solutions.starbucks.Model.Pocos
{
    [TableName("Invites")]
    [PrimaryKey("InviteID")]
    [ExplicitColumns]
    public class Invites
    {
        [Column("InviteID")]
        public int InviteID { get; set; }

        [Column("OperatorUmbracoUserID")]
        public int OperatorUmbracoUserID { get; set; }

        [Column("AccessToken")]
        public Guid AccessToken { get; set; }

        [Column("TraineeName")]
        public string TraineeName { get; set; }

        [Column("TraineeEmail")]
        public string TraineeEmail { get; set; }

        [Column("DateInvited")]
        public DateTime DateInvited { get; set; }

        [Column("DateCreated")]
        public DateTime DateCreated { get; set; }

        [Column("DateCompleted")]
        public DateTime? DateCompleted { get; set; }

        [Column("IsDeleted")]
        public bool IsDeleted { get; set; }

        [Column("InviteSubjects")]
        public IEnumerable<InvitesSubjects> InviteSubjects { get; set; }

        public double PercentComplete { get; set; }

        public IEnumerable<AccessLog> AccessLogs { get; set; }

        public AccessStates AccessState { get; set; }


        public const double ActiveHoursThreshold = 24;
        public const int ExpirationDaysThreshold = 30;

        public enum AccessStates { 
            Invited = 1,
            Active = 2,
            Idle = 4,
            Expired = 8
        }

        public static IEnumerable<Invites> SetInvitesSubjects(IEnumerable<Invites> invites, IEnumerable<InvitesSubjects> invitesSubjects)
        {
            foreach (var i in invites)
            {
                i.InviteSubjects = (from iSubj in invitesSubjects where iSubj.InviteID == i.InviteID select iSubj).OrderBy(s => s.Subject.SortOrder).ToList();
            }
            return invites;
        }

        public static IEnumerable<Invites> SetAccessLogs(IEnumerable<Invites> invites, IEnumerable<AccessLog> accessLogs)
        {
            if (invites != null && accessLogs != null)
            {

                var groupedAccessLogs = (from i in invites
                           join a in accessLogs on i.InviteID equals a.InviteID
                           group a by i.InviteID into grp
                           select new { inviteId = grp.Key, accessLogs = grp.OrderByDescending(a => a.DateAccessed).ToList() }).ToList();

                var join = from i in invites
                           join a in groupedAccessLogs on i.InviteID equals a.inviteId
                           select new { invite = i, accessLogs = a.accessLogs.ToList() };

                foreach (var joined in join) {
                    joined.invite.AccessLogs = joined.accessLogs;
                }
            }
            return invites;
        }

        public static IEnumerable<Invites> SetAccessStates(IEnumerable<Invites> invites)
        {
            var invitesAssessAccessStates = from i in invites
                        select new
                        {
                            invite = i,
                            accessState = GetAccessState(i)
                        };
            foreach (var inviteAssessed in invitesAssessAccessStates) {
                inviteAssessed.invite.AccessState = inviteAssessed.accessState;
            }
            return invites;
        }

        public static AccessStates GetAccessState(Invites invite) {

            AccessStates accessState = AccessStates.Invited;
            DateTime now = DateTime.UtcNow;
            if (now.Subtract(invite.DateInvited).Days > ExpirationDaysThreshold)
            {
                accessState = AccessStates.Expired;
            }
            else if (invite.AccessLogs == null) {
                accessState = AccessStates.Invited;
            }
            else if (invite.AccessLogs != null && now.Subtract(invite.AccessLogs.First().DateAccessed).TotalHours > ActiveHoursThreshold) {
                accessState = AccessStates.Idle;
            }
            else if (invite.AccessLogs != null && now.Subtract(invite.AccessLogs.First().DateAccessed).TotalHours <= ActiveHoursThreshold) {
                accessState = AccessStates.Active;
            }
            return accessState;
        }

        /// <summary>
        /// Use responses to quizzes and invitation views to determine percent complete
        /// </summary>
        /// <param name="invites">Invitations for which percent complete will be determined</param>
        /// <param name="invitesSubjects">The subjects for each invitation</param>
        /// <param name="responses">Responses to subjects' quizzes</param>
        /// <param name="subjectQuizzes">Quizzes for each subject</param>
        /// <returns>List of Invites with PercentComplete populated</returns>
        public static IEnumerable<Invites> SetPercentComplete(IEnumerable<Invites> invites, IEnumerable<InvitesSubjects> invitesSubjects, IEnumerable<QuizResponses> responses, IEnumerable<Quiz> subjectQuizzes)
        {
            if (invites == null || invites.Count() == 0 || invitesSubjects == null || invitesSubjects.Count() == 0)
            {
                return invites;
            }

            // Calculate total quiz responses per subject invite
            var responseCountPerSubjectInvite = from si in invitesSubjects
                                                join r in responses on si.InviteSubjectID equals r.InviteSubjectID
                                                group si by si.InviteSubjectID into grp
                                                select new { inviteSubjectId = grp.Key, responseCount = grp.Count() };

            // Determine completion status for each subject invite
            var inviteSubjectsStatus = from i in invites
                                       join si in invitesSubjects on i.InviteID equals si.InviteID
                                       join qz in subjectQuizzes on si.SubjectID equals qz.SubjectID into inviteQuizzes
                                            from iq in inviteQuizzes.DefaultIfEmpty()
                                       join r in responseCountPerSubjectInvite on si.InviteSubjectID equals r.inviteSubjectId into quizResponses
                                            from ir in quizResponses.DefaultIfEmpty()
                                       select new
                                       {
                                           inviteSubjectId = si.InviteSubjectID,
                                           invite = i,
                                           completed = CompletedCheck(si, iq != null, iq == null ? 0 : iq.Questions.Count(), ir == null ? 0 : ir.responseCount)
                                       };

            // Calculate percent complete for each invite (subject invites completed / total subjects)
            var percentCompleteByInvite = from s in inviteSubjectsStatus
                                          group s by s.invite into grp
                                          select new
                                          {
                                              invite = grp.Key,
                                              completed = Convert.ToDouble(grp.Count(e => e.completed == true)),
                                              total = Convert.ToDouble(grp.Count()),
                                              percentComplete = Convert.ToDouble(grp.Count(e => e.completed == true)) / Convert.ToDouble(grp.Count())
                                          };

            foreach (var inviteCalc in percentCompleteByInvite)
            {
                inviteCalc.invite.PercentComplete = inviteCalc.percentComplete;
            }
            return invites;
        }

        public static bool CompletedCheck(InvitesSubjects subjectInvite, bool hasQuiz, int questionCount, int responseCount)
        {
            bool completed = false;
            if ((!hasQuiz && subjectInvite.DateAccessed != null) || (hasQuiz && questionCount == responseCount))
            {
                completed = true;
            }
            return completed;
        }

    }
}