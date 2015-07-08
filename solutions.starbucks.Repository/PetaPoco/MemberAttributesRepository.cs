using solutions.starbucks.Interfaces;
using solutions.starbucks.Model.Pocos;
using System.Collections.Generic;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace solutions.starbucks.Repository.PetaPoco
{
    public class MemberAttributesRepository : IMemberAttributesRepository
    {
        protected UmbracoDatabase _db = ApplicationContext.Current.DatabaseContext.Database;

        public IEnumerable<CustomerAttributes> GetCurrentMemberShipping(int umbracoUserID)
        {
            //string shippingQuery = "SELECT CustomerAttributes.AccountName, CustomerAttributes.AccountNumber, CustomerAttributes.AccountSiteNumber, CustomerAttributes.Nickname, CustomerAttributes.PostalCode,CustomerAttributes.Address1, CustomerAttributes.Address2, CustomerAttributes.City, CustomerAttributes.State, AssociatedMemberAccounts.UmbracoUserID FROM CustomerAttributes LEFT JOIN AssociatedMemberAccounts ON CustomerAttributes.AccountNumber = AssociatedMemberAccounts.AccountNumber WHERE AssociatedMemberAccounts.UmbracoUserID=@0 AND CustomerAttributes.PostalCode = AssociatedMemberAccounts.Zip ORDER BY CustomerAttributes.AccountNumber, CustomerAttributes.AccountSiteNumber";
            string shippingQuery = "SELECT CustomerAttributes.AccountName, CustomerAttributes.AccountNumber, CustomerAttributes.AccountSiteNumber, CustomerAttributes.Nickname, OpportunityAttributes.OpportunityName, CustomerAttributes.PostalCode,CustomerAttributes.Address1, CustomerAttributes.Address2, CustomerAttributes.City, CustomerAttributes.State, AssociatedMemberAccounts.UmbracoUserID FROM CustomerAttributes LEFT JOIN AssociatedMemberAccounts ON CustomerAttributes.AccountNumber = AssociatedMemberAccounts.AccountNumber LEFT JOIN OpportunityAttributes on CustomerAttributes.AccountNumber = OpportunityAttributes.AccountNumber AND CustomerAttributes.AccountSiteNumber = OpportunityAttributes.SiteID WHERE AssociatedMemberAccounts.UmbracoUserID=@0 ORDER BY CustomerAttributes.AccountNumber, CustomerAttributes.AccountSiteNumber";

            return _db.Query<CustomerAttributes>(shippingQuery, umbracoUserID);
        }

        //public IEnumerable<CustomerAttributes> GetCurrentMemberShippingWithNickname(int umbracoUserID)
        //{
        //    string shippingQuery = "SELECT CustomerAttributes.AccountName, CustomerAttributes.AccountNumber, OpportunityAttributes.SiteID AS AccountSiteNumber, OpportunityAttributes.Nickname, CustomerAttributes.PostalCode,CustomerAttributes.Address1, CustomerAttributes.Address2, CustomerAttributes.City, CustomerAttributes.State, AssociatedMemberAccounts.UmbracoUserID FROM CustomerAttributes LEFT JOIN AssociatedMemberAccounts ON CustomerAttributes.AccountNumber = AssociatedMemberAccounts.AccountNumber LEFT JOIN OpportunityAttributes ON OpportunityAttributes.AccountNumber = CustomerAttributes.AccountNumber WHERE AssociatedMemberAccounts.UmbracoUserID=@0 AND CustomerAttributes.AccountSiteUsePrimary = 'Y' ORDER BY CustomerAttributes.AccountNumber, OpportunityAttributes.SiteID";

        //    return _db.Query<CustomerAttributes>(shippingQuery, umbracoUserID);
        //}

        public IEnumerable<AssociatedMemberAccount> GetAssociatedAccountsByMemberAccount(int umbracoUserID)
        {
            string queryText = "SELECT * FROM AssociatedMemberAccounts WHERE AccountNumber IN (SELECT AccountNumber FROM AssociatedMemberAccounts WHERE UmbracoUserID=@0)";
            return _db.Query<AssociatedMemberAccount>(queryText, umbracoUserID);
        }

        public IEnumerable<MemberIdWithAssociatedAccounts> GetAllAssociatedAccounts()
        {
            string queryText = "SELECT [ID] AS MemberId, STUFF(( SELECT ', ' + [AccountNumber] FROM [AssociatedMemberAccounts] WHERE (ID = Results.ID) FOR XML PATH(''),TYPE).value('(./text())[1]','VARCHAR(MAX)') ,1,2,'') AS AssociatedAccounts FROM [AssociatedMemberAccounts] Results GROUP BY ID";
            return _db.Query<MemberIdWithAssociatedAccounts>(queryText);
        }

        public IEnumerable<AssociatedMemberAccount> GetAssociatedAccount(int umbracoUserID)
        {
            string queryText = "SELECT * FROM AssociatedMemberAccounts WHERE UmbracoUserID=@0";
            return _db.Query<AssociatedMemberAccount>(queryText, umbracoUserID);
        }

        public IEnumerable<CustomerAttributes> GetMemberWithAccountNumberAndZip(string accountNumber, string zip)
        {
            string queryText = "SELECT * FROM CustomerAttributes WHERE AccountNumber=@0 AND PostalCode LIKE @1";
            return _db.Query<CustomerAttributes>(queryText, accountNumber, "%" + zip + "%");
        }

        public IEnumerable<AssociatedMemberAccount> GetMemberWithAccountZipAndId(string id, string accountNumber, string zip)
        {
            //SELECT * FROM AssociatedMemberAccounts WHERE ID=@0 AND AccountNumber=@1 AND Zip LIKE @2
            var queryText = Sql.Builder.Append("SELECT * FROM AssociatedMemberAccounts WHERE ID=@0", id)
                .Append(" AND AccountNumber=@0", accountNumber)
                .Append(" AND Zip LIKE @0", "%" + zip + "%");



            return _db.Query<AssociatedMemberAccount>(queryText);
        }

        public AssociatedMemberAccount GetFirstAccountReference(int umbracoUserID)
        {
            //SELECT * FROM AssociatedMemberAccounts WHERE ID=@0 AND AccountNumber=@1 AND Zip LIKE @2
            var queryText = Sql.Builder.Append("SELECT TOP (1) * FROM AssociatedMemberAccounts WHERE UmbracoUserID=@0", umbracoUserID);
            var a = _db.SingleOrDefault<AssociatedMemberAccount>(queryText);
            return a;
            //return db.Query<AssociatedMemberAccount>(queryText);
        }

        public CustomerAttributes GetFirstCustomerAccountReference(string accountNumber)
        {
            //SELECT * FROM AssociatedMemberAccounts WHERE ID=@0 AND AccountNumber=@1 AND Zip LIKE @2
            var queryText = Sql.Builder.Append("SELECT TOP (1) * FROM CustomerAttributes WHERE AccountNumber=@0", accountNumber);
            var a = _db.SingleOrDefault<CustomerAttributes>(queryText);
            return a;
            //return db.Query<AssociatedMemberAccount>(queryText);
        }

        public AssociatedMemberAccount GetFirstAccountReferenceByAccount(string accountNumber)
        {
            //SELECT * FROM AssociatedMemberAccounts WHERE ID=@0 AND AccountNumber=@1 AND Zip LIKE @2
            var queryText = Sql.Builder.Append("SELECT TOP (1) * FROM AssociatedMemberAccounts WHERE AccountNumber=@0", accountNumber);
            var a = _db.SingleOrDefault<AssociatedMemberAccount>(queryText);
            return a;
            //return db.Query<AssociatedMemberAccount>(queryText);
        }
        
        public CustomerAttributes GetMemberBrand(string accountNumber, string zip)
        {
            //SELECT * FROM AssociatedMemberAccounts WHERE ID=@0 AND AccountNumber=@1 AND Zip LIKE @2
            var queryText = Sql.Builder.Append("SELECT TOP (1) * FROM CustomerAttributes WHERE AccountNumber=@0", accountNumber);
                //.Append(" AND PostalCode LIKE @0", "%" + zip + "%");
            var a = _db.SingleOrDefault<CustomerAttributes>(queryText);
            return a;
            //return db.Query<AssociatedMemberAccount>(queryText);
        }

        public void InsertAssociatedAccount(AssociatedMemberAccount associatedMemberAccount)
        {
            _db.Insert(associatedMemberAccount);
        }

        public OpportunityAttributes GetStarbucksBrewedAccount(string accountNumber)
        {
            var queryText = Sql.Builder.Append("SELECT TOP (1) * FROM OpportunityAttributes WHERE (AccountNumber=@0)", accountNumber)
                .Append("AND (SBUXBrewProgram = @0", "Signature Brewed")
                .Append("OR SBUXBrewProgram = @0)", "Core Brewed")
                .Append("AND (SBUXEspressoProgram NOT LIKE '%' + @0 + '%'", "Espresso")
                .Append("OR SBUXEspressoProgram IS NULL)");

            var a = _db.SingleOrDefault<OpportunityAttributes>(queryText);
            return a;
        }

        public IEnumerable<CustomerAttributes> GetCustomerAttributes(string accountNumber)
        {
            var queryText = Sql.Builder
                .Append("SELECT CA.*, OA.OpportunityName ")
                .Append("FROM CustomerAttributes CA ")
                .Append("LEFT JOIN OpportunityAttributes OA ON CA.AccountSiteNumber = OA.SiteID ")
                .Append("	AND CA.AccountNumber = OA.AccountNumber")
                .Append("WHERE CA.AccountNumber = @0", accountNumber);

            return _db.Query<CustomerAttributes>(queryText);
        }

    }

}