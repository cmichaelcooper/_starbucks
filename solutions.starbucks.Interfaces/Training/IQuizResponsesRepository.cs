using solutions.starbucks.Model.Pocos;
using System.Collections.Generic;

namespace solutions.starbucks.Interfaces
{
    public interface IQuizResponsesRepository
    {
        IEnumerable<QuizResponses> GetByInviteSubjectID(int inviteSubjectID, bool? isDeleted = false);
        IEnumerable<QuizResponses> GetByInviteSubjectIDs(List<int> inviteSubjectIDs, bool? isDeleted = false);
        QuizResponses Save(QuizResponses model);
        QuizResponses Delete(QuizResponses model);
    }
}