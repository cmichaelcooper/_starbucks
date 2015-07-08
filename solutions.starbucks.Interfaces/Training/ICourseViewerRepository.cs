using solutions.starbucks.Model.Training;
using Umbraco.Core.Models;

namespace solutions.starbucks.Interfaces
{
    public interface ICourseViewerRepository
    {
        CourseContent FillCourseContent(IInvitesRepository invitesRepository, IInvitesSubjectsRepository invitesSubjectsRepository, ICourseBuilderRepository courseBuilderRepository, IQuizResponsesRepository quizResponsesRepository, int inviteId, int subjectId, IPublishedContent currentPage, bool renderQuiz);
    }
}