using solutions.starbucks.Model.Pocos;
using System.Collections.Generic;

namespace solutions.starbucks.Interfaces
{
    public interface IInvitesSubjectsRepository
    {
        IEnumerable<InvitesSubjects> GetInvitesSubjectsByInviteID(int inviteID, bool? isDeleted = false);
        IEnumerable<InvitesSubjects> GetInvitesSubjectsByInviteIDs(IEnumerable<int> inviteIDs, bool? isDeleted = false);
        InvitesSubjects GetByInviteIDSubjectID(int inviteID, int subjectID);
        InvitesSubjects Save(InvitesSubjects model);
        IEnumerable<InvitesSubjects> Delete(Invites invite);
        
    }
}