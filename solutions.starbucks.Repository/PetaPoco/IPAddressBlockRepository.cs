using solutions.starbucks.Interfaces;
using Umbraco.Core;
using Umbraco.Core.Persistence;



namespace solutions.starbucks.Repository.PetaPoco
{
    public class IPAddressBlockRepository : IIPAddressBlockRepository
    {
        protected UmbracoDatabase _db = ApplicationContext.Current.DatabaseContext.Database;

        public string GetCountryForIPAddress(string ipAddress)
        {
            string value = "US";
            try
            {
                var queryText = Sql.Builder.Append("select country from ipaddressblock where 1 = [dbo].IsIPAddressInRange(@0,BlockStart,BlockEnd);", ipAddress);
                value = _db.FirstOrDefault<string>(queryText) ?? "US";
            }
            catch { }
            return value;
        }
    }
}

