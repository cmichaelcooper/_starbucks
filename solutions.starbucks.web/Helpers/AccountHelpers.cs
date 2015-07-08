using solutions.starbucks.Interfaces;
using solutions.starbucks.Model.Enums;
using solutions.starbucks.Model.Pocos;
using System;
using System.Web;
using System.Web.Security;
using Umbraco.Core.Models;

namespace solutions.starbucks.web.Helpers
{
    public class AccountHelpers
    {

        public static void SetCurrentAccountSessionVars(string accountNumber, IMember currentMember, IMemberAttributesRepository memberRepository)
        {
            AssociatedMemberAccount associatedMemberAccount = new AssociatedMemberAccount();
            CustomerAttributes associatedCustomerAccount = new CustomerAttributes();
            
            //if the account number isn't passed in, get the first from currentMember
            if (string.IsNullOrEmpty(accountNumber))
            {
                associatedMemberAccount = memberRepository.GetFirstAccountReference(currentMember.Id);

                if (associatedMemberAccount == null)
                {
                    SetCurrentAccountSessionVars("", "", false, false, false);
                    return;
                }
                accountNumber = associatedMemberAccount.AccountNumber;
            }
            associatedCustomerAccount = memberRepository.GetFirstCustomerAccountReference(accountNumber);

            bool isEspresso = true;
            if (memberRepository.GetStarbucksBrewedAccount(accountNumber) != null)
            {
                isEspresso = false;
            }

            SetCurrentAccountSessionVars(accountNumber, DetermineBrand(associatedCustomerAccount.Brands).Code, DetermineIsPartner(currentMember.Email), isEspresso, Convert.ToBoolean(associatedCustomerAccount.IsFrapp));
        }

        public static void SetCurrentAccountSessionVars(string accountNumber, string brand, bool isPartner, bool isEspresso, bool isFrappuccino)
        {
            HttpContext.Current.Session["CurrentAccount"] = accountNumber;

            HttpContext.Current.Response.Cookies["AccountAttributes"]["CurrentAccount"] = accountNumber;
            HttpContext.Current.Response.Cookies["AccountAttributes"]["CustomerBrand"] = brand;
            HttpContext.Current.Response.Cookies["AccountAttributes"]["IsPartner"] = isPartner.ToString().ToLower();
            HttpContext.Current.Response.Cookies["AccountAttributes"]["IsEspresso"] = isEspresso.ToString().ToLower();
            HttpContext.Current.Response.Cookies["AccountAttributes"]["IsFrappuccino"] = isFrappuccino.ToString().ToLower();
            HttpContext.Current.Response.Cookies["AccountAttributes"].Expires = DateTime.MaxValue;

        }

        public static BrandCode DetermineBrand(string brandList)
        {
            BrandCode retval = BrandCode.DUAL;
            bool hasSbc = brandList.ToUpper().Contains("SBC");
            bool hasSbx = brandList.ToUpper().Contains("SBUX");

            if (hasSbc && !hasSbx)
            {
                retval = BrandCode.SBC;
            }
            else if (hasSbx && !hasSbc)
            {
                retval = BrandCode.SBUX;
            }

            return retval;
        }

        public static bool DetermineIsPartner(string email)
        {
            return Roles.IsUserInRole(email, "Partner") || Roles.IsUserInRole(email, "PartnerAdmin") || Roles.IsUserInRole(email, "SuperPartnerAdmin") ? true : false; ;
        }

    }
}
