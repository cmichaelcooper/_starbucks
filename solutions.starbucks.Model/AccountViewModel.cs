using solutions.starbucks.Model.Pocos;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace solutions.starbucks.Model
{
    public class LoginViewModel
    {
        [DisplayName("Email address")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Enter your email address")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string EmailAddress { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Enter your password")]
        public string Password { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string ReturnUrl { get; set; }

        [DisplayName("Stay Signed In")]
        public bool? RememberMe { get; set; }
    }

    public class ForgottenPasswordViewModel
    {
        [DisplayName("Email address")]
        [Required(ErrorMessage = "Enter your email address")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string EmailAddress { get; set; }
    }

    public class ContactViewModel : ForgottenPasswordViewModel
    {
        [DisplayName("Name")]
        [Required(ErrorMessage = "Enter your name")]
        public string Name { get; set; }

        [DisplayName("Phone")]
        [Required(ErrorMessage = "Enter your phone number")]
        public string Phone { get; set; }

        [DisplayName("Business Name")]
        [Required(ErrorMessage = "Enter your business name")]
        public string BusinessName { get; set; }

        [DisplayName("Account Number")]
        [RegularExpression(@"[0-9]{7,7}", ErrorMessage = "Invalid Account Number")]
        public string AccountNumber { get; set; }

        [DisplayName("Reason for Request")]
        [Required(ErrorMessage = "Please tell us how we can help")]
        public string Message { get; set; }

        public string CurrentForm { get; set; }
    }

    public class AccountAccessViewModel
    {
        public IEnumerable<SelectListItem> AssociatedAccounts { get; set; }

        public string SelectedAccount { get; set; }

        public bool isPartner { get; set; }

    }

    public class ResetPasswordViewModel
    {
        [DisplayName("Email address")]
        [Required(ErrorMessage = "Enter your email address")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string EmailAddress { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Enter your password")]
        //[RegularExpression(@"^[a-zA-Z0-9]{8,}", ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression(@"(?=^.{8,}$)((?=.*\d))(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$", ErrorMessage = "Password must be at least 8 characters, contain one uppercase letter, one lowercase letter and a number.")]
        public new string Password { get; set; }


        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Confirm your password")]
        //[RegularExpression(@"^[a-zA-Z0-9]{8,}", ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression(@"(?=^.{8,}$)((?=.*\d))(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$", ErrorMessage = "Password must be at least 8 characters, contain one uppercase letter, one lowercase letter and a number.")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Your passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
    public class CustomerAttributesViewModel
    {
        public IEnumerable<CustomerAttributes> Customer { get; set; }
    }
   
}