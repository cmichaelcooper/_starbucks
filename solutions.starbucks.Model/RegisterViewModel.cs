using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace solutions.starbucks.Model
{
    public class RegisterViewModel 
    {

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EmailAddress { get; set; }

        public string AccountNumber { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string CompanyName { get; set; }

        public string Title { get; set; }

        public string PhoneNumber { get; set; }

        public string Zip { get; set; }
        
        [HiddenInput(DisplayValue = false)]
        public string NextPage { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string MobilePhone { get; set; }
        
        [HiddenInput(DisplayValue = false)]
        public string PreviousPage { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int Attempts { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string SkipStep { get; set; }

        public string OptionalDetails { get; set; }
    }
    public class RegisterPageOneViewModel : RegisterViewModel
    {
        [DisplayName("Email address")]
        [Required(ErrorMessage = "Enter your email address")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        [Remote("CheckEmailIsUsed", "AccountSurface", ErrorMessage = "The email address has already been registered")]
        public new string EmailAddress { get; set; }
    }

    public class RegisterPageTwoViewModel : RegisterPageOneViewModel
    {

        //[DisplayName("Email address")]
        [Required(ErrorMessage = "Enter your email address")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        [Remote("CheckEmailIsUsed", "AccountSurface", ErrorMessage = "The email address has already been registered")]
        public new string Emailaddress { get; set; }

        [DisplayName("Account Number")]
        //[Required(ErrorMessage = "Enter your Account Number")]
        //[Remote("AccountNumberVerification", "AccountSurface", ErrorMessage = "Incorrect account number")]
        [RegularExpression(@"[0-9]{7,7}", ErrorMessage = "Invalid Account Number")]
        public new string AccountNumber { get; set; }

        //[Remote("AccountNumberVerification", "AccountSurface", AdditionalFields = "AccountNumber", ErrorMessage = "Your account number/zip combination is invalid. try again. ")]
        [RegularExpression(@"[0-9A-Za-z ]{5,}", ErrorMessage = "Invalid Zip")]
        [DisplayName("Zip code associated with the account")]
        public new string Zip { get; set; }


    }

    public class RegisterPageThreeViewModel : RegisterPageTwoViewModel
    {

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Enter your password")]
        //[RegularExpression(@"^[a-zA-Z0-9]{8,}", ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression(@"(?=^.{8,}$)((?=.*\d))(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$", ErrorMessage = "Password requirements not met")]
        public new string Password { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Confirm Password")]
        [Required(ErrorMessage = "Enter your password")]
        //[RegularExpression(@"(?=^.{8,}$)((?=.*\d))(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$", ErrorMessage = "Password requirements not met.")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Your passwords do not match")]
        public new string ConfirmPassword { get; set; }

        //[Remote("AccountNumberVerification", "AccountSurface", AdditionalFields = "AccountNumber", ErrorMessage = "Your account number/zip combination is invalid. try again. ")]
        //[DisplayName("Zip code associated with the account")]
        //public string Zip { get; set; }
    }

    public class RegisterPageFourViewModel : RegisterPageThreeViewModel
    {
        [Required(ErrorMessage = "Enter your first name")]
        public new string FirstName { get; set; }

        [Required(ErrorMessage = "Enter your last name")]
        public new string LastName { get; set; }

        [DisplayName("Email address")]
        [Required(ErrorMessage = "Enter your email address")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        [Remote("CheckEmailIsUsed", "AccountSurface", ErrorMessage = "The email address has already been registered")]
        public new string EmailAddress { get; set; }

        [DisplayName("Account Number")]
        //[Required(ErrorMessage = "Enter your Account Number")]
        //[Remote("AccountNumberVerification", "AccountSurface", ErrorMessage = "Incorrect account number")]
        [RegularExpression(@"[0-9]{7,}", ErrorMessage = "Invalid Account Number")]
        public new string AccountNumber { get; set; }

        [Required(ErrorMessage = "Enter your company name")]
        public new string CompanyName { get; set; }

        [Required(ErrorMessage = "Enter your title")]
        public new string Title { get; set; }

        [Required(ErrorMessage = "Enter your Phone number")]
        //[RegularExpression(@"[0-9]{7,}", ErrorMessage = "Invalid Phone Number.")]
        public new string PhoneNumber { get; set; }

        [DisplayName("Mobile")]
        //[RegularExpression(@"[0-9]{7,}", ErrorMessage = "Invalid Mobile Phone Number.")]
        public new string MobilePhone { get; set; }

        //[Required(ErrorMessage = "Enter your the zip code associated with this account.")]
        //[Remote("AccountNumberVerification", "AccountSurface", AdditionalFields = "AccountNumber", ErrorMessage = "Your account number/zip combination is invalid. try again. ")]
        //[DisplayName("Zip code associated with the account")]
        //public string Zip { get; set; }
        [Required(ErrorMessage = "Enter your city")]
        public new string City { get; set; }

        [Required(ErrorMessage = "state/prov")]
        public new string State { get; set; }

    }

    public class RegistrationViewModel : IValidatableObject
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string EmailAddress { get; set; }

        //[Required(ErrorMessage = "Enter your Account Number")]
        //[Remote("AccountNumberVerification", "AccountSurface", ErrorMessage = "Incorrect account number")]
        public string AccountNumber { get; set; }

        //[RegularExpression(@"[0-9a-zA-Z]{12,}", ErrorMessage = "Password must be at least 12 characters")]
        public string Password { get; set; }

        //[RegularExpression(@"[0-9a-zA-Z]{12,}", ErrorMessage = "Password must be at least 12 characters")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Your passwords do not match")]
        public string ConfirmPassword { get; set; }

        public string CompanyName { get; set; }

        public string Title { get; set; }

        //[RegularExpression(@"[0-9]{7,}", ErrorMessage = "Invalid Phone Number.")]
        public string PhoneNumber { get; set; }


        [HiddenInput(DisplayValue = false)]
        public string NextPage { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string MobilePhone { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string PreviousPage { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int Attempts { get; set; }
        
        [DisplayName("Zip code associated with the account")]
        public string Zip { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (NextPage == "pagetwo" && string.IsNullOrEmpty(EmailAddress))
            {
                yield return new ValidationResult("Enter your email address.");
            }
        }

    }
    public class EmailViewModel
    {
        public string Message { get; set; }

        [DisplayName("Email address")]
        [Required(ErrorMessage = "Enter your email address")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string FromEmail { get; set; }
    }
}