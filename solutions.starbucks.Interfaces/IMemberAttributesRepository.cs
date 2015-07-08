using solutions.starbucks.Model.Pocos;
using System.Collections.Generic;

namespace solutions.starbucks.Interfaces
{
    public interface IMemberAttributesRepository
    {
        IEnumerable<CustomerAttributes> GetCurrentMemberShipping(int umbracoUserID);
        //IEnumerable<CustomerAttributes> GetCurrentMemberShippingWithNickname(int umbracoUserID);
        IEnumerable<MemberIdWithAssociatedAccounts> GetAllAssociatedAccounts();
        IEnumerable<AssociatedMemberAccount> GetAssociatedAccount(int umbracoUserID);
        IEnumerable<AssociatedMemberAccount> GetAssociatedAccountsByMemberAccount(int umbracoUserID);
        IEnumerable<CustomerAttributes> GetMemberWithAccountNumberAndZip(string accountNumber, string zip);
        IEnumerable<AssociatedMemberAccount> GetMemberWithAccountZipAndId(string id, string accountNumber, string zip);
        AssociatedMemberAccount GetFirstAccountReference(int umbracoUserID);
        CustomerAttributes GetFirstCustomerAccountReference(string accountNumber);
        AssociatedMemberAccount GetFirstAccountReferenceByAccount(string accountNumber);
        CustomerAttributes GetMemberBrand(string accountNumber, string zip);
        void InsertAssociatedAccount(AssociatedMemberAccount associatedMemberAccount);
        OpportunityAttributes GetStarbucksBrewedAccount(string accountNumber);
        IEnumerable<CustomerAttributes> GetCustomerAttributes(string accountNumber);
    }
}