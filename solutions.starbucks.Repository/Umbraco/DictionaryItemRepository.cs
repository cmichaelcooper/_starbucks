using solutions.starbucks.Interfaces;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace solutions.starbucks.Repository.PetaPoco
{
    public class DictionaryItemRepository : IDictionaryItemRepository
    {
        protected UmbracoDatabase _db = ApplicationContext.Current.DatabaseContext.Database;

        public string GetTermInEnglish(string term)
        {
            var queryText = Sql.Builder.Append("SELECT value FROM vw_DictionaryItems WHERE ISOCode = @0 AND DictionaryId = (SELECT TOP 1 DictionaryId FROM vw_DictionaryItems WHERE Value = @1)", "en-US", term);
            string value = _db.FirstOrDefault<string>(queryText) ?? term;
            return value;
        }
    }
}

           