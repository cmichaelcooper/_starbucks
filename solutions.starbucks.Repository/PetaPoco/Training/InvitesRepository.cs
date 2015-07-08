using solutions.starbucks.Interfaces;
using solutions.starbucks.Model.Pocos;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace solutions.starbucks.Repository.PetaPoco
{
    public class InvitesRepository : IInvitesRepository
    {
        protected UmbracoDatabase _db = ApplicationContext.Current.DatabaseContext.Database;

        public IEnumerable<Invites> GetInvitesByOperatorUmbracoUserID(int operatorUmbracoUserID, bool? isDeleted = false)
        {
            var queryText = Sql.Builder.Append("SELECT * FROM training.Invites WHERE OperatorUmbracoUserID=@0" + (isDeleted !=  null ? " AND IsDeleted = " + (Convert.ToBoolean(isDeleted) ? "1" : "0") : string.Empty), operatorUmbracoUserID.ToString());
            IEnumerable<Invites> invites = _db.Query<Invites>(queryText);
            return invites;
        }

        public IEnumerable<Invites> GetAccountInvitesByOperatorUmbracoUserID(int operatorUmbracoUserID, IMemberAttributesRepository memberAttributesRepository, bool? isDeleted = false)
        {
            List<int> umbracoUserIds = memberAttributesRepository.GetAssociatedAccountsByMemberAccount(operatorUmbracoUserID).Select(m => m.UmbracoUserID).ToList();
            if (!umbracoUserIds.Contains(operatorUmbracoUserID)) umbracoUserIds.Add(operatorUmbracoUserID);
            string umbracoUserIdsIn = string.Join(", ", umbracoUserIds.Select(i => i.ToString())).ToString();

            var queryText = Sql.Builder.Append("SELECT * FROM training.Invites WHERE OperatorUmbracoUserID IN (" + umbracoUserIdsIn.ToString() + ")" + (isDeleted != null ? " AND IsDeleted = " + (Convert.ToBoolean(isDeleted) ? "1" : "0") : string.Empty));
            IEnumerable<Invites> invites = _db.Query<Invites>(queryText);
            return invites;
        }

        public Invites GetByAccessToken(string accessToken)
        {
            var queryText = Sql.Builder.Append("SELECT * FROM training.Invites WHERE AccessToken=@0", accessToken);
            var invite = _db.SingleOrDefault<Invites>(queryText);
            return invite;
        }

        public Invites GetById(int inviteId)
        {
            var queryText = Sql.Builder.Append("SELECT * FROM training.Invites WHERE InviteID=@0", inviteId);
            var invite = _db.SingleOrDefault<Invites>(queryText);
            return invite;
        }

        public Invites Save(Invites model)
        {
            if (!(model.InviteID > 0)) {
                DateTime dateCreated = DateTime.UtcNow;
                model.DateCreated = dateCreated;
                model.DateInvited = dateCreated;
                Guid accessToken = Guid.NewGuid();
                model.AccessToken = accessToken;
                var query = Sql.Builder.Append("; EXEC training.InviteInsert @@operatorUmbracoUserID = @0, @@AccessToken = @1, @@traineeName = @2, @@traineeEmail = @3, @@dateInvited = @4, @@dateCreated = @5",
                        model.OperatorUmbracoUserID, model.AccessToken,  model.TraineeName, model.TraineeEmail, model.DateInvited, model.DateCreated);
                model.InviteID = Convert.ToInt32(_db.SingleOrDefault<int>(query));
            }
            else {
                var query = Sql.Builder.Append("; EXEC training.InviteSave @@inviteID = @0, @@operatorUmbracoUserID = @1, @@accessToken = @2, @@traineeName = @3, @@traineeEmail = @4, @@dateInvited = @5, @@dateCreated = @6, @@dateCompleted = @7, @@isDeleted = @8",
                        model.InviteID, model.OperatorUmbracoUserID, model.AccessToken, model.TraineeName, model.TraineeEmail, model.DateInvited, model.DateCreated, model.DateCompleted, model.IsDeleted);
                _db.Execute(query);
            }
            return model;
        }

        public Invites Delete(Invites model)
        {
            model.IsDeleted = true;
            Save(model);
            return model;
        }

    }
}