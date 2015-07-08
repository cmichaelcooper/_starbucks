using solutions.starbucks.Interfaces;
using solutions.starbucks.Model.Pocos.Training;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace solutions.starbucks.Repository.PetaPoco
{
    public class AccessLogRepository : IAccessLogRepository
    {
        protected UmbracoDatabase _db = ApplicationContext.Current.DatabaseContext.Database;

        public IEnumerable<AccessLog> GetByInviteIDs(List<int> inviteIds)
        {
            string inviteIdsIn = string.Join(", ", inviteIds.Select(i => i.ToString())).ToString();
            var queryText = Sql.Builder.Append("SELECT * FROM training.AccessLog WHERE InviteID IN (" + inviteIdsIn.ToString() + ")");
            IEnumerable<AccessLog> accessLogs = _db.Query<AccessLog>(queryText).ToList();
            return accessLogs;
        }

        public AccessLog Insert(AccessLog.Access_Type accessType, int inviteId, int? inviteSubjectId = null)
        {
            AccessLog model = new AccessLog();
            model.AccessType = accessType;
            model.InviteID = inviteId;
            model.InviteSubjectID = inviteSubjectId;
            model.DateAccessed = DateTime.UtcNow;
            var query = Sql.Builder.Append("; EXEC training.AccessLogInsert @@accessType = @0, @@inviteID = @1, @@inviteSubjectID = @2, @@dateAccessed = @3",
                        model.AccessType, model.InviteID, model.InviteSubjectID, model.DateAccessed);
            model.AccessLogID = Convert.ToInt32(_db.SingleOrDefault<int>(query));                
            return model;
        }

    }
}