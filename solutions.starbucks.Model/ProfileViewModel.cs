using solutions.starbucks.Model.Pocos;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Umbraco.Core.Models;


namespace solutions.starbucks.Model
{
	public class ProfileViewModel
	{
        [HiddenInput(DisplayValue = false)]
        public int MemberID { get; set; }

        [Required(ErrorMessage = "Enter your name")]
        public string Name { get; set; }

        //[DisplayName("Email Address")]
        //[Required(ErrorMessage = "Please enter your email address")]
        //[EmailAddress(ErrorMessage = "Please enter a valid email address")]
        //[Remote("CheckProfileEmailIsUsed", "ProfileSurface", ErrorMessage = "The email address has already been registered")]
        [HiddenInput(DisplayValue = false)]
        public string EmailAddress { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Previous Password")]
        [Remote("CheckOldPassword", "ProfileSurface", ErrorMessage = "The previous password field doesn't match your current password. Please try again.")]
        public string Password { get; set; }

        [DisplayName("New Password")]
        [DataType(DataType.Password)]
        [RegularExpression(@"(?=^.{8,}$)((?=.*\d))(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$", ErrorMessage = "Password must be at least 8 characters, contain one uppercase letter, one lowercase letter and a number.")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Confirm Password")]
        [RegularExpression(@"(?=^.{8,}$)((?=.*\d))(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$", ErrorMessage = "Password must be at least 8 characters, contain one uppercase letter, one lowercase letter and a number.")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "Your passwords do not match")]
        public string ConfirmPassword { get; set; }

        [DisplayName("Account Number")]
        //[Required(ErrorMessage = "Please enter your Account Number")]
        //[Remote("AccountNumberVerification", "AccountSurface", ErrorMessage = "Incorrect account number")]
        [RegularExpression(@"[0-9]{7,7}", ErrorMessage = "Invalid Account Number")]
        public string AccountNumber { get; set; }

        //[Remote("AccountNumberVerification", "AccountSurface", AdditionalFields = "AccountNumber", ErrorMessage = "Your account number/zip combination is invalid. Please try again. ")]
        [DisplayName("Account Shipping Zip Code")]
        [RegularExpression(@"[0-9A-Za-z ]{5,}", ErrorMessage = "Invalid Zip")]
        public string Zip { get; set; }

        [DisplayName("Title")]
        [Required(ErrorMessage = "Enter your title")]
        public string Title { get; set; }

        [DisplayName("Company Name")]
        [Required(ErrorMessage = "Enter your company name")]
        public string CompanyName { get; set; }

        [DisplayName("City")]
        [Required(ErrorMessage = "Enter your city")]
        public string City { get; set; }

        [DisplayName("State")]
        [Required(ErrorMessage = "Enter your state")]
        public string State { get; set; }

        [DisplayName("Phone Number")]
        //[RegularExpression(@"[0-9]{7,}", ErrorMessage = "Invalid Phone Number")]
        [Required(ErrorMessage = "Enter your phone number")]
        public string Phone { get; set; }
        
        [DisplayName("Mobile Phone")]
        //[RegularExpression(@"[0-9]{7,7}", ErrorMessage = "Invalid Mobile Phone")]
        public string MobilePhone { get; set; }

        public IEnumerable<AssociatedMemberAccount> AssociatedAccounts { get; set; }

        public IEnumerable<CustomerAttributes> AccountShippingInformation { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int Attempts { get; set; }

        public string OptionalDetails { get; set; }
	}

    public class MemberReportViewModel
    {
        public string SearchTerm { get; set; }
        public string ActiveFilter { get; set; }
        public IEnumerable<IMember> ActiveMembers { get; set; }
        public IEnumerable<IMember> NonActiveMembers { get; set; }
        public IEnumerable<IMember> InactiveMembers { get; set; }
        public IEnumerable<IMember> PartnerMembers { get; set; }
        public Dictionary<int, string> AssociatedAccounts { get; set; }
    }

}