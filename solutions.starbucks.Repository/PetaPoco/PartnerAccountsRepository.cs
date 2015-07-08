using solutions.starbucks.Interfaces;
using solutions.starbucks.Model.Pocos;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace solutions.starbucks.Repository.PetaPoco
{
    public class PartnerAccountsRepository : IPartnerAccountsRepository
    {
        protected UmbracoDatabase _db = ApplicationContext.Current.DatabaseContext.Database;


        public PartnerAdmin GetPartnerByEmail(string emailAddress)
        {
            var queryText = Sql.Builder.Append("SELECT TOP (1) * FROM PartnerAdmin WHERE PartnerEmail=@0", emailAddress);
            var a = _db.SingleOrDefault<PartnerAdmin>(queryText);
            return a;
        }

        public SuperPartnerAdmin GetSuperPartnerByEmail(string emailAddress)
        {
            var queryText = Sql.Builder.Append("SELECT TOP (1) * FROM SuperPartnerAdmin WHERE PartnerEmail=@0", emailAddress);
            var a = _db.SingleOrDefault<SuperPartnerAdmin>(queryText);
            return a;
        }
    }
}