using solutions.starbucks.Model.Pocos;
using System.Collections.Generic;
using System.Linq;

namespace solutions.starbucks.Model.Training
{

    public class SupplementalCategories
    {
        public List<Category> Categories { get; set; }
        public List<Module> Modules { get; set; }
        
    }

    public class CourseContent
    {
        public bool RenderQuiz { get; set; }
        public Subject CurrentSubject { get; set; }
        public List<Subject> CourseSubjects { get; set; }
        public bool CourseCompleted { get; set; }
        public List<InvitesSubjects> SubjectInvites { get; set; }
        public List<Quiz> SubjectQuizzes { get; set; }
        public Quiz SubjectQuiz { get; set; }
        public Invites Invite { get; set; }
        public InvitesSubjects InviteSubject { get; set; }
        public string Console { get; set; }
        public Subject NextSubject { get; set; }

        public void SetNextSubject() {

            NextSubject = GetNext(CurrentSubject);

        }

        public Subject GetNext(Subject currentSubject)
        {

            Subject nextSubject = null;
            if (currentSubject.SortOrder + 1 == CourseSubjects.Count())
                nextSubject = CourseSubjects.First();
            else 
                nextSubject = CourseSubjects.Where(cs => cs.SortOrder == currentSubject.SortOrder + 1).SingleOrDefault();
            
            // If course has been completed, simply return the next subject
            if (CourseCompleted) return nextSubject;

            // If the quiz is to be included and course is not yet complete, next subject will depend on what Quiz is not yet completed
            if (RenderQuiz)
            {
                var nextSubjectInvite = SubjectInvites.Where(si => si.SubjectID == nextSubject.SubjectID).SingleOrDefault();
                if (nextSubjectInvite.Completed)
                {
                    return GetNext(nextSubject);
                }
            }
            return nextSubject;
        }

        public static bool GetCourseCompleted(List<InvitesSubjects> subjectInvites)
        {
            var completedSubjects = subjectInvites != null ? subjectInvites.Where(si => si.Completed).ToList() : null;
            return subjectInvites == null ? true : completedSubjects != null && completedSubjects.Count == subjectInvites.Count;
        }


   
    }

}

