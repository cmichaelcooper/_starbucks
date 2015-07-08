using solutions.starbucks.Interfaces;
using solutions.starbucks.Model.Pocos;
using solutions.starbucks.Model.Training;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace solutions.starbucks.Repository.Umbraco
{
    public class CourseBuilderRepository : ICourseBuilderRepository
    {

        private static UmbracoRepository _umbracoRepository = new UmbracoRepository();

        public IPublishedContent SubjectsParent(IPublishedContent rootContent)
        {
            var subjectsParent = rootContent.Descendants().Where(c => c.DocumentTypeAlias == "Training").Single();
            return subjectsParent;
        }

        public IEnumerable<IPublishedContent> SubjectsContentsFromRoot(IPublishedContent rootContent)
        {            
            var subjectsContent = rootContent.Children.Where(c => c.Name == "Resources").Single().Children.Where(c => c.DocumentTypeAlias == "Training").Single().Children.Where(c => c.DocumentTypeAlias == "Fundamentals");
            return subjectsContent;
        }

        /// <summary>
        /// Retrieve the "Fundamental" subjects for training
        /// </summary>
        /// <param name="rootContent">The root content page that hosts the umbraco site content</param>
        /// <returns>The subjects available for a course in an IEnumberable</returns>
        public IEnumerable<Subject> SubjectsFromRoot(IPublishedContent rootContent, List<int> subjectIds = null)
        {
            return Subjects(SubjectsParent(rootContent), subjectIds);
        }
        
        /// <summary>
        /// Retrieve the "Fundamental" subjects for training
        /// </summary>
        /// <param name="content">The parent content page that hosts the Fundamental training pages</param>
        /// <returns>The subjects available for a course in an IEnumberable</returns>
        public IEnumerable<Subject> Subjects(IPublishedContent content, List<int> subjectIds = null)
        {
            List<Subject> subjects = _umbracoRepository.GetPublishedChildItems(content)
                            .Where(s => s.DocumentTypeAlias == "Fundamentals")
                            .Select(S => new Subject { 
                                SubjectID = S.Id,
                                FundamentalIcon = S.GetPropertyValue<string>("FundamentalIcon"),
                                Title = S.GetPropertyValue<string>("PageTitle"),
                                ShortDescription = S.GetPropertyValue<string>("ShortDescription"),
                                Url = S.Url,
                                SortOrder = S.SortOrder
                            }).OrderBy(S => S.SortOrder).ToList();
            // Must reset sortOrder because "Fundamentals" has other child items
            subjects = subjectIds != null ? subjects.Where(s => subjectIds.Contains(s.SubjectID)).ToList() : subjects;
            ResetSortOrder(subjects);
            return subjects;
        }

        private static void ResetSortOrder(List<Subject> subjects)
        {
            for (int idx = 0; idx < subjects.Count(); idx++)
            {
                subjects[idx].SortOrder = idx;
            }            
        }
        
        /// <summary>
        /// Serves up a complete quiz entity with Question, Answers, Responses, etc.
        /// </summary>
        /// <param name="content">The umbraco page from which the quiz is being served.</param>
        /// <returns>A helper Quiz object used to store questions and answers for a given subjectId</returns>
        public Quiz SubjectQuiz(IPublishedContent content, IEnumerable<QuizResponses> responses = null)
        {
            IPublishedContent quizDef = content.Children.Where(cq => cq.DocumentTypeAlias == "Quiz").SingleOrDefault();
            
            Quiz quiz = null;
            if (quizDef != null)
            {
                IEnumerable<Question> questions = null;
                questions = quizDef.Children.Where(q => q.DocumentTypeAlias == "Question")
                .Select(Q => new Question {
                    QuestionID = Q.Id,
                    SubjectID = content.Id,
                    QuestionText = Q.GetPropertyValue<string>("questionText"),
                    SortOrder = Q.SortOrder,
                    Answers = Q.Children.Select(A => new Answer
                    {
                        AnswerID = A.Id,
                        QuestionID = Q.Id,
                        AnswerText = A.GetPropertyValue<string>("answerText"),
                        IsCorrect = A.GetPropertyValue<bool>("isCorrect"),
                        SortOrder = A.SortOrder
                    })
                }).ToList();

                // Set correct answer property
                var join = from q in questions
                           join a in questions.SelectMany(q => q.Answers) on q.QuestionID equals a.QuestionID
                           where a.IsCorrect
                           select new { question = q, answer = a };
                foreach (var joined in join)
                {
                    joined.question.CorrectAnswer = joined.answer;
                }

                Quiz.SetQuestionResponses(questions, responses);
                
                quiz = new Quiz(content.Id, questions);
            }
            
            return quiz;
        }

        
        /// <summary>
        /// This method fills the model used to navigate course content. It runs with or without an inviteId, depending on who is viewing the training.
        /// </summary>
        /// <param name="invitesRepository">Repo to look up invite</param>
        /// <param name="invitesSubjectsRepository">Repo to look up invitation subjects</param>
        /// <param name="quizResponsesRepository">Repo to look up quiz responses</param>
        /// <param name="inviteId">The ID for the invitation</param>
        /// <param name="subjectId">The ID for the current subject in the course</param>
        /// <param name="currentPage">The umbraco page on which training is taking place</param>
        /// <param name="renderQuiz">A true/false value that specifies whether the quiz needs to be rendered (operators don't see the quiz)</param>
        /// <returns>Helper object used to navigate course content based on invitation, completion status and type of person viewing the training (trainee vs. operator)</returns>
       public CourseContent FillCourseContent(IInvitesRepository invitesRepository,
                                                IInvitesSubjectsRepository invitesSubjectsRepository,
                                                IQuizResponsesRepository quizResponsesRepository,
                                                int inviteId, int subjectId, IPublishedContent currentPage, bool renderQuiz)
        {
            CourseContent model = new CourseContent();
            model.RenderQuiz = renderQuiz;

            Invites invite = inviteId > 0 ? invitesRepository.GetById(inviteId) : null;
            model.Invite = invite;

            // Retrieve all subject invites and fill the property
            List<InvitesSubjects> subjectInvites = inviteId > 0 ? invitesSubjectsRepository.GetInvitesSubjectsByInviteID(invite.InviteID).ToList() : null;

            // Set up the subjects for the course
            List<Subject> subjects = null;
            if (inviteId > 0)
            {
                List<int> subjectIds = subjectInvites.Select(si => si.SubjectID).ToList();
                subjects = this.Subjects(currentPage.Parent, subjectIds).ToList();
            }
            else
            {
                // There is not an invite, just get all the subjects
                subjects = this.Subjects(currentPage.Parent).ToList();
            }
            InvitesSubjects.SetAllSubjects(subjectInvites, subjects);

            // Set up subject quizzes for entire course in order to know what is next
            List<Quiz> subjectQuizzes = new List<Quiz>();
            if (renderQuiz)
            {
                IEnumerable<QuizResponses> responses = null;
                // Collect all quiz responses
                if (subjectInvites != null && subjectInvites.Count() > 0)
                {
                    responses = quizResponsesRepository.GetByInviteSubjectIDs(subjectInvites.Select(si => si.InviteSubjectID).ToList());
                }
                foreach (Subject subject in subjects)
                {
                    Quiz quiz = this.SubjectQuiz(currentPage.Parent.Children.Where(s => s.Id == subject.SubjectID).SingleOrDefault(), responses);
                    if (quiz != null) subjectQuizzes.Add(quiz);
                }
            }
            if (subjectQuizzes.Count() == 0) subjectQuizzes = null;
            Subject.SetAllQuizzes(subjects, subjectQuizzes);

            // Populate active Subject property
            model.CurrentSubject = subjects.Where(cs => cs.SubjectID == subjectId).SingleOrDefault();
            model.SubjectInvites = subjectInvites;
            model.CourseSubjects = subjects;
            model.SubjectQuizzes = subjectQuizzes;

            // Set completed status for the course
            model.CourseCompleted = CourseContent.GetCourseCompleted(subjectInvites);

            // Populate the current quiz and include responses for given Invite
            model.SubjectQuiz = subjectQuizzes != null ? subjectQuizzes.Where(sq => sq.SubjectID == subjectId).SingleOrDefault() : null;
            // Populate the current subject invite
            model.InviteSubject = subjectInvites != null ? subjectInvites.Where(si => si.SubjectID == subjectId).SingleOrDefault() : null;

            // Set the next subject property
            model.SetNextSubject();

            return model;
        }


    }
}