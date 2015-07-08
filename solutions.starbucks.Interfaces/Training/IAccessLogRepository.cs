using solutions.starbucks.Model.Pocos.Training;
using System.Collections.Generic;

namespace solutions.starbucks.Interfaces
{
    public interface IAccessLogRepository
    {
        IEnumerable<AccessLog> GetByInviteIDs(List<int> inviteIDs);
        AccessLog Insert(AccessLog.Access_Type accessType, int inviteId, int? inviteSubjectId = null);
        
    }
}