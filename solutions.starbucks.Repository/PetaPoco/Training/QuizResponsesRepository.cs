using solutions.starbucks.Interfaces;
using solutions.starbucks.Model.Pocos;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace solutions.starbucks.Repository.PetaPoco
{
    public class QuizResponsesRepository : IQuizResponsesRepository
    {
        protected UmbracoDatabase _db = ApplicationContext.Current.DatabaseContext.Database;

        public IEnumerable<QuizResponses> GetByInviteSubjectID(int inviteSubjectID, bool? isDeleted = false)
        {
            var queryText = Sql.Builder.Append("SELECT * FROM training.QuizResponses WHERE InviteSubjectID=@0" + (isDeleted != null ? " AND IsDeleted = " + (Convert.ToBoolean(isDeleted) ? "1" : "0") : string.Empty), inviteSubjectID.ToString());
            IEnumerable<QuizResponses> quizResponses = _db.Query<QuizResponses>(queryText);
            return quizResponses;
        }

        public IEnumerable<QuizResponses> GetByInviteSubjectIDs(List<int> inviteSubjectIDs, bool? isDeleted = false)
        {
            string inviteSubjectIDsIn = string.Join(", ", inviteSubjectIDs.Select(i => i.ToString())).ToString();
            var queryText = Sql.Builder.Append("SELECT * FROM training.QuizResponses WHERE InviteSubjectID IN (" + inviteSubjectIDsIn.ToString() + ")" + (isDeleted != null ? " AND IsDeleted = " + (Convert.ToBoolean(isDeleted) ? "1" : "0") : string.Empty));
            IEnumerable<QuizResponses> quizResponses = _db.Query<QuizResponses>(queryText);
            return quizResponses;
        }

        public QuizResponses Save(QuizResponses model)
        {
            if (!(model.QuizResponseID > 0)) {
                DateTime dateCreated = DateTime.UtcNow;
                var query = Sql.Builder.Append("; EXEC training.QuizResponseInsert @@inviteSubjectID = @0, @@questionID = @1, @@value = @2, @@dateCreated = @3",
                        model.InviteSubjectID, model.QuestionID,  model.Value, dateCreated);
                model.QuizResponseID = Convert.ToInt32(_db.SingleOrDefault<int>(query));                
            }
            else {
                var query = Sql.Builder.Append("; EXEC training.QuizResponseSave @@quizResponseID = @0, @@inviteSubjectID = @1, @@questionID = @2, @@value = @3, @@dateCreated = @4, @@isDeleted = @5",
                        model.QuizResponseID, model.InviteSubjectID, model.QuestionID, model.Value, model.DateCreated, model.IsDeleted);
                _db.Execute(query);
            }
            return model;
        }

        public QuizResponses Delete(QuizResponses model)
        {
            model.IsDeleted = true;
            Save(model);
            return model;
        }

    }
}