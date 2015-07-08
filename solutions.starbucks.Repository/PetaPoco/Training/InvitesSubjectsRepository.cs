using solutions.starbucks.Interfaces;
using solutions.starbucks.Model.Pocos;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace solutions.starbucks.Repository.PetaPoco
{
    public class InvitesSubjectsRepository : IInvitesSubjectsRepository
    {
        protected UmbracoDatabase _db = ApplicationContext.Current.DatabaseContext.Database;

        public IEnumerable<InvitesSubjects> GetInvitesSubjectsByInviteID(int inviteID, bool? isDeleted = false)
        {
            var queryText = Sql.Builder.Append("SELECT * FROM training.InvitesSubjects WHERE InviteID=@0" + (isDeleted != null ? " AND IsDeleted = " + (Convert.ToBoolean(isDeleted) ? "1" : "0") : string.Empty), inviteID.ToString());
            return _db.Query<InvitesSubjects>(queryText);
        }

        public InvitesSubjects GetByInviteIDSubjectID(int inviteID, int subjectID)
        {
            var queryText = Sql.Builder.Append("SELECT * FROM training.InvitesSubjects WHERE InviteID=@0 AND SubjectID=@1", inviteID, subjectID);
            var inviteSubject = _db.SingleOrDefault<InvitesSubjects>(queryText);
            return inviteSubject;
        }

        public InvitesSubjects Save(InvitesSubjects model)
        {
            if (!(model.InviteSubjectID > 0)) {
                DateTime dateCreated = DateTime.UtcNow;
                var query = Sql.Builder.Append("; EXEC training.InviteSubjectInsert @@inviteID = @0, @@subjectID = @1, @@dateCreated = @2 ",
                        model.InviteID, model.SubjectID, dateCreated);
                model.InviteID = Convert.ToInt32(_db.SingleOrDefault<int>(query));
            }
            else {
                var query = Sql.Builder.Append("; EXEC training.InviteSubjectSave @@inviteSubjectID = @0, @@inviteID = @1, @@subjectID = @2, @@dateCreated = @3, @@dateAccessed = @4, @@dateCompleted = @5, @@isDeleted = @6",
                        model.InviteSubjectID, model.InviteID, model.SubjectID, model.DateCreated, model.DateAccessed, model.DateCompleted, model.IsDeleted);
                _db.Execute(query);
            }
            return model;

        }

        public IEnumerable<InvitesSubjects> GetInvitesSubjectsByInviteIDs(IEnumerable<int> inviteIDs, bool? isDeleted = false)
        {
            if (inviteIDs != null && inviteIDs.Count() > 0)
            {
                string inviteIDsIn = string.Join(", ", inviteIDs.Select(i => i.ToString())).ToString();
                var queryText = Sql.Builder.Append("; SELECT * FROM training.InvitesSubjects WHERE InviteID IN (" + inviteIDsIn + ")" + (isDeleted != null ? " AND IsDeleted = " + (Convert.ToBoolean(isDeleted) ? "1" : "0") : string.Empty));
                IEnumerable<InvitesSubjects> inviteSubjects = _db.Query<InvitesSubjects>(queryText);
                return inviteSubjects.ToList();
            }
            else {
                return null;
            }
        }

        public IEnumerable<InvitesSubjects> Delete(Invites invite)
        {
            if (invite.InviteSubjects != null && invite.InviteSubjects.Count() > 0)
            {
                foreach (var inviteSubject in invite.InviteSubjects)
                {
                    inviteSubject.IsDeleted = true;
                }
            }
            
            var queryText = Sql.Builder.Append("UPDATE training.InvitesSubjects SET IsDeleted = 1 WHERE InviteID=@0", invite.InviteID.ToString());
            _db.Execute(queryText); 
            return invite.InviteSubjects;
        }

    }
}