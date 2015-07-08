using solutions.starbucks.Interfaces;
using solutions.starbucks.Model.Pocos;
using solutions.starbucks.Model.Training;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace solutions.starbucks.Repository.Umbraco
{
    public class CourseViewerRepository : ICourseViewerRepository
    {

        private static UmbracoRepository _umbracoRepository = new UmbracoRepository();


        /// <summary>
        /// Training modules 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public IEnumerable<Module> Modules(IPublishedContent content)
        {
            var trainingModulesContent = content.AncestorsOrSelf("Home").Single().Descendant("TrainingModules");

            // Look up modules for given category
            List<Module> modules = _umbracoRepository.GetPublishedChildItems(trainingModulesContent)
                            .Select(M => new Module
                            {
                                Id = M.Id,
                                Type = Module.GetTypeFromAlias(M.DocumentTypeAlias),
                                Title = M.GetPropertyValue<string>("ModuleTitle"),
                                ShortDescription = M.GetPropertyValue<string>("ShortDescription"),
                                LongDescription = M.GetPropertyValue<string>("LongDescription"),
                                CategoryNames = M.GetPropertyValue<string>("Categories").Split(new char[] { ',' }).ToList(),
                                SortOrder = M.SortOrder
                            }).OrderBy(S => S.SortOrder).ToList();
            return modules;
        }

        /// <summary>
        /// Returns the categories by which training modules are organized. These are created by using datatype in the developer section of the umbraco cms.
        /// </summary>
        /// <param name="dataTypeId">The id used to look up the categories from the umbraco datatype library</param>
        /// <returns>List of categories for organizing training modules</returns>
        public IEnumerable<Category> Categories(int dataTypeId)
        {
            // Look up categories from datatypes
            List<Category> categories = new List<Category>();
            XPathNodeIterator preValueRootElementIterator = umbraco.library.GetPreValues(dataTypeId);
            preValueRootElementIterator.MoveNext(); //move to first
            XPathNodeIterator preValueIterator = preValueRootElementIterator.Current.SelectChildren("preValue", "");
            while (preValueIterator.MoveNext())
            {
                categories.Add(new Category(Convert.ToInt32(preValueIterator.Current.GetAttribute("id", "")), preValueIterator.Current.Value));
            }
            if (categories.Count() == 0) categories = null;
            return categories;
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
                                                ICourseBuilderRepository courseBuilderRepository, 
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
                subjects = courseBuilderRepository.Subjects(currentPage.Parent, subjectIds).ToList();
            }
            else
            {
                // There is not an invite, just get all the subjects
                subjects = courseBuilderRepository.Subjects(currentPage.Parent).ToList();
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
                    Quiz quiz = courseBuilderRepository.SubjectQuiz(currentPage.Parent.Children.Where(s => s.Id == subject.SubjectID).SingleOrDefault(), responses);
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