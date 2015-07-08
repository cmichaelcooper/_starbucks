using solutions.starbucks.Model.Training;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Persistence;

namespace solutions.starbucks.Model.Pocos
{
    [TableName("InvitesSubjects")]
    [PrimaryKey("InviteSubjectID")]
    [ExplicitColumns]
    public class InvitesSubjects
    {
        [Column("InviteSubjectID")]
        public int InviteSubjectID { get; set; }

        [Column("InviteID")]
        public int InviteID { get; set; }

        [Column("SubjectID")]
        public int SubjectID { get; set; }

        [Column("DateCreated")]
        public DateTime DateCreated { get; set; }
        
        [Column("DateAccessed")]
        public DateTime? DateAccessed { get; set; }

        [Column("DateCompleted")]
        public DateTime? DateCompleted { get; set; }

        [Column("IsDeleted")]
        public bool IsDeleted { get; set; }

        public bool Completed { 
            get { return DateCompleted != null; }
        }

        public Subject Subject { get; set; }

        public static void SetAllSubjects(IEnumerable<InvitesSubjects> invitesSubjects, IEnumerable<Subject> subjects)
        {
            if (invitesSubjects != null && subjects != null)
            {
                var join = from si in invitesSubjects
                           join s in subjects on si.SubjectID equals s.SubjectID
                           select new { inviteSubject = si, subject = s };
                foreach (var joined in join)
                {
                    joined.inviteSubject.Subject = joined.subject;
                }
            }            
        }

        public static void SetAllSubjectQuizzes(IEnumerable<InvitesSubjects> invitesSubjects, IEnumerable<Quiz> subjectQuizzes)
        {
            if (invitesSubjects != null && subjectQuizzes != null)
            {
                var join = from si in invitesSubjects
                           join s in subjectQuizzes on si.SubjectID equals s.SubjectID
                           select new { inviteSubject = si, subjectQuiz = s};
                foreach (var joined in join)
                {
                    joined.inviteSubject.Subject.Quiz = joined.subjectQuiz;
                }
            }
        }

       
    }
}