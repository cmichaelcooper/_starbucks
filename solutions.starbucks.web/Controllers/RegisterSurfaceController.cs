using solutions.starbucks.Interfaces;
using solutions.starbucks.Model;
using solutions.starbucks.Model.Pocos;
using solutions.starbucks.web.ExtensionMethods;
using System;
using System.Linq;
using System.Web.Mvc;
using TheAlchemediaProject.Services;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.member;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace solutions.starbucks.web.Controllers
{
    public class RegisterSurfaceController : SurfaceController
    {
        private readonly IMemberAttributesRepository _memberRepository;
        private readonly IPartnerAccountsRepository _partnerRepository;

        public RegisterSurfaceController(IMemberAttributesRepository memberRepository, IPartnerAccountsRepository partnerRepository)
        {
            _memberRepository = memberRepository;
            _partnerRepository = partnerRepository;
        }
        public ActionResult RenderRegistration(string page=null, string skip=null)
        {
            var currentFormPage = Request.Url.ToString();
            var currentEmail = Session["CurrentEmail"] != null ? Session["CurrentEmail"].ToString() : "";
            var skipStep = skip;
            if (String.IsNullOrEmpty(skipStep))
            {
                skipStep = Session["SkipStep"] != null ? Session["SkipStep"].ToString() : "";
            }
            if (page == "begin")
            {
                RegisterPageOneViewModel register = new RegisterPageOneViewModel();
                if (currentEmail != "")
                {
                    register.EmailAddress = currentEmail;
                }
                register.NextPage = "pagetwo";

                return PartialView("RegisterHandlePageOne", register);
            }
            else if ((page == "pagetwo") && (currentEmail != ""))
            {
                //var test = Session["test"].ToString();
                RegisterPageTwoViewModel register = new RegisterPageTwoViewModel();
                register.EmailAddress = currentEmail;
                register.NextPage = "pagethree";
                register.PreviousPage = "pageone";
                register.Attempts = Convert.ToInt32(Session["AccountValidCount"]);
                
                return PartialView("RegisterHandlePageTwo", register);
            }
            else if ((page == "pagethree") && (currentEmail != ""))
            {
                RegisterPageThreeViewModel register = new RegisterPageThreeViewModel();
                //Add in attributes from step three model
                register.EmailAddress = currentEmail;
                register.NextPage = "pagefour";
                if (String.IsNullOrEmpty(skipStep) && !currentEmail.Contains("@starbucks.com"))
                {
                    register.Zip = Session["Zip"] != null ? Session["Zip"].ToString() : "";
                    register.AccountNumber = Session["AccountNumber"] != null ? Session["AccountNumber"].ToString() : "";
                }
                register.SkipStep = skipStep;
                
                if (currentEmail.Contains("@starbucks.com"))
                {
                    register.PreviousPage = "pageone";
                }
                else
                {
                    register.PreviousPage = "pagetwo";
                }
                register.Attempts = Convert.ToInt32(Session["AccountValidCount"]);
                
                return PartialView("RegisterHandlePageThree", register);
            }
            else if ((page == "pagefour") && (currentEmail != ""))
            {

                RegisterPageFourViewModel register = new RegisterPageFourViewModel();
                register.EmailAddress = currentEmail;
                register.NextPage = "finish";
                register.PreviousPage = "pagethree";
                register.Zip = Session["Zip"] != null ? Session["Zip"].ToString() : "";
                register.AccountNumber = Session["AccountNumber"] != null ? Session["AccountNumber"].ToString() : "";
                register.Password = Session["DetailedPass"] != null ? Session["DetailedPass"].ToString() : "" ;
                register.ConfirmPassword = Session["DetailedPass"] != null ? Session["DetailedPass"].ToString() : "";
                register.SkipStep = skipStep;
                register.Attempts = Convert.ToInt32(Session["AccountValidCount"]);
                
                return PartialView("RegisterHandlePageFour", register);
            }
            else
            {
                RegisterPageOneViewModel register = new RegisterPageOneViewModel();
                if (currentEmail != "")
                {
                    register.EmailAddress = currentEmail;
                }
                register.NextPage = "pagetwo";
                return PartialView("RegisterHandlePageOne", register);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProcessRegistration(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }
            string currentEmail = model.EmailAddress;
            string domainName = currentEmail.Split('@').Last().ToLower() ;
            var currentMemberEmail = Request.Cookies["emailAddress"] != null ? Request.Cookies["emailAddress"].Value : "";
            bool validatedAccount = false;
            TempData["CurrentEmail"] = currentEmail;
            var currentMemberInfo = Member.GetMemberFromEmail(currentEmail);
            //New Member information
            MemberType sbsMemberType = MemberType.GetByAlias("UnverifiedCustomer");
            var memberGroup = MemberGroup.GetByName("Unverified Customer");
            
            bool accountNumberValid = ApplicationContext.AccountNumberVerification(model.AccountNumber, model.Zip);
            //change member type if the user has a starbucks email
            if (domainName == "starbucks.com")
            {
                sbsMemberType = MemberType.GetByAlias("Partner");
                memberGroup = MemberGroup.GetByName("Partner");
                validatedAccount = true;

                var partnerAdmin = _partnerRepository.GetPartnerByEmail(currentEmail);

                if (partnerAdmin != null)
                {
                    memberGroup = MemberGroup.GetByName("PartnerAdmin");
                }

                var superPartnerAdmin = _partnerRepository.GetSuperPartnerByEmail(currentEmail);

                if (superPartnerAdmin != null)
                {
                    memberGroup = MemberGroup.GetByName("SuperPartnerAdmin");
                }
                //Partner does not need to enter account/zip combination, skip step two
                if (model.NextPage == "pagetwo")
                {
                    model.NextPage = "pagethree";
                }

            }
            else
            {
                //bool accountNumberValid = ApplicationContext.AccountNumberVerification(model.AccountNumber, model.Zip);

              
                if (model.NextPage == "pagethree")
                {
                    if ((!accountNumberValid && !currentEmail.Contains("@starbucks.com")) && model.SkipStep != "1")
                    {
                        int attempts = Convert.ToInt32(model.Attempts);
                        attempts++;
                        Session["AccountValidCount"] = attempts;
                        if (attempts < 3)
                        {
                           model.NextPage = "pagetwo";
                        }
                        else
                        {
                            Session["SkipStep"] = "1";
                        }
                    }
                    else
                    {

                        validatedAccount = true;
                        Session["AccountValidCount"] = 0;
                        Session["ValidatedAccount"] = validatedAccount;
                    }
                    Session["AccountNumber"] = model.AccountNumber;
                    Session["Zip"] = model.Zip;
                }
                if (model.NextPage == "pagefour")
                {
                    //Session["DetailedPass"] = model.Password;
                    Session["AccountNumber"] = model.AccountNumber;
                    Session["Zip"] = model.Zip;
                    Session["SkipStep"] = model.SkipStep;
                }
            }
            if (model.NextPage == "pagefour")
            {
                Session["DetailedPass"] = model.Password;
            }

            if (model.NextPage == "finish")
            {
                var tempGUID = Guid.NewGuid();
                var checkEmail = Member.GetMemberFromEmail(model.EmailAddress);

                if (checkEmail != null)
                {
                    ModelState.AddModelError("error", "Email Address is already in use.");
                    return RedirectToCurrentUmbracoPage();
                }
                Member createMember = Member.MakeNew(model.FirstName + " " + model.LastName, model.EmailAddress, model.EmailAddress, sbsMemberType, new User(0));
                if (Convert.ToBoolean(Session["ValidatedAccount"]))
                {
                    sbsMemberType = MemberType.GetByAlias("Customer");
                    memberGroup = MemberGroup.GetByName("Customer");
                    
                }
                if (Session["AccountValidCount"] != null)
                {
                    createMember.getProperty("accountNumberAttempts").Value = Convert.ToInt32(Session["AccountValidCount"]);
                    if (Convert.ToInt32(Session["AccountValidCount"]) == 3)
                    {
                        createMember.getProperty("accountNumberAttemptsTimeout").Value = DateTime.Now.AddMinutes(30).ToString("dd/MM/yyyy @ HH:mm:ss");
                    }
                }
                createMember.AddGroup(memberGroup.Id);
                createMember.getProperty("validatedAccountNumber").Value = validatedAccount;
                createMember.getProperty("emailVerifyGUID").Value = tempGUID.ToString() ;
                createMember.getProperty("accountNumber").Value = model.AccountNumber;
                createMember.getProperty("zip").Value = model.Zip;
                createMember.Password = model.Password;
                createMember.getProperty("firstName").Value = model.FirstName;
                createMember.getProperty("lastName").Value = model.LastName;
                createMember.getProperty("title").Value = model.Title;
                createMember.getProperty("optionalDetails").Value = model.OptionalDetails;
                if (domainName != "starbucks.com")
                {
                    createMember.getProperty("companyName").Value = model.CompanyName;
                    createMember.getProperty("city").Value = model.City;
                    createMember.getProperty("state").Value = model.State;
                }
                createMember.getProperty("phoneNumber").Value = model.PhoneNumber;
                createMember.getProperty("mobilePhone").Value = model.MobilePhone;
                createMember.getProperty("joinedDate").Value = DateTime.Now.ToString("dd/MM/yyyy @ HH:mm:ss");
               
                var emailFrom = CurrentPage.GetPropertyValue("emailFrom", "sjones@marlinco.com").ToString();
                var emailSubject = "Verify your Branded Solutions account";

                MailService email = new MailService();
                string baseURL = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Replace(System.Web.HttpContext.Current.Request.Url.AbsolutePath, string.Empty);
                var verifyURL = baseURL + "/verify-user?verifyGUID=" + tempGUID;
                

                var messsage = string.Format(
                                        "<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'> <html xmlns='http://www.w3.org/1999/xhtml'> <head> <meta http-equiv='Content-Type' content='text/html; charset=UTF-8' /> <title>Profile Created</title> </head> <body style='-webkit-text-size-adjust: none;'> <table align='center' width='600' border='0' cellspacing='0' cellpadding='0'> <tr> <td style='text-align:left;' colspan='2' bgcolor='#3E3935' height='90'><p style='margin-left:18px; margin-top:18px; margin-bottom:18px; margin-right:18px'><img src='http://marlinco.com/eblast/sbs/system_emails/branded_solutions_logo.png' width='223' height='54'/></p></td> </tr> <tr> <td style='text-align:left;' bgcolor='#FFFFFF' colspan='2' width='600'><p style='text-align:left; font-family:Corbel, sans-serif; font-size:25px; font-weight:normal; color:#3E3935; margin-left:18px; margin-right:18px; margin-top:30px;'>Thank you for creating your profile on Starbucks Branded Solutions.</p> <p style='margin-top:0px; margin-right:0px; margin-bottom:0px; margin-left:18px;'><a style='text-decoration:none; border:0;' href='{0}'><img src='http://marlinco.com/eblast/sbs/system_emails/click_to_confirm_button.png' width='180' height='35' alt='' style='border:0;'/></a></p> <p style='text-align:left; font-family:Corbel, sans-serif; font-size:18px; font-weight:normal; color:#3E3935; margin-left:18px; margin-right:18px; margin-bottom:30px;'>If you did not initiate this process, you can ignore this email. If concerned, you may wish to notify us of this activity here: <span style='color:#63A70A;'>fsinsidesales@starbucks.com</span>.<br> <br> Thanks again,<br> Starbucks Branded Solutions</p> </td> </tr> </table>  </body> </html>",
                                        verifyURL);
                try
                {
                    email.SendEmail(model.EmailAddress, "no-reply@starbucksfs.com", emailSubject, messsage, true);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("EmailError", "There was a problem sending the verification email. Please try again. If this problem persists, please contact Starbucks customer service.");
                }

                if (domainName != "starbucks.com")
                {

                    bool accountVerified = _memberRepository.GetMemberWithAccountNumberAndZip(model.AccountNumber, model.Zip).Any();
                   
                    //User account is validated
                    if (accountVerified)
                    {
                   
                        createMember.getProperty("validatedAccountNumber").Value = true;
                        TempData["AccountValidated"] = true;
                        TempData["CustomerState"] = createMember.getProperty("state").Value.ToString();
                        var accountNumberAddition = new AssociatedMemberAccount();
                        accountNumberAddition.AccountID = createMember.Id;
                        accountNumberAddition.UmbracoUserID = createMember.Id;
                        accountNumberAddition.AccountNumber = model.AccountNumber;
                        accountNumberAddition.Zip = model.Zip;
                   
                        //Umbraco db context
                        //var db = ApplicationContext.DatabaseContext.Database;
                        //Get the current Umbraco Node ID
                        _memberRepository.InsertAssociatedAccount(accountNumberAddition);
                        //Insert AssociatedMemberAccount
                        //db.Insert(accountNumberAddition);

                    }
                    else
                    {
                        TempData["AccountValidated"] = false;
                        TempData["CustomerState"] = createMember.getProperty("state").Value.ToString();
                    }
                }
                else
                {
                    createMember.getProperty("validatedAccountNumber").Value = true;
                    TempData["PartnerRegistration"] = true;
                    TempData["AccountValidated"] = true;
                    TempData["CustomerState"] = createMember.getProperty("state").Value.ToString();
                }
                createMember.XmlGenerate(new System.Xml.XmlDocument());
                createMember.Save();
                TempData["FinishedSignup"] = true;
                Session.RemoveAll();
                return new RedirectResult("/");
            }


            Session["CurrentEmail"] = currentEmail;
            Session["NextPage"] = model.NextPage.ToString() ;

            return RedirectToCurrentUmbracoPage();
        }

        public ActionResult RenderEmailForm()
        {
            var currentEmail = Request.Cookies["emailAddress"] != null ? Request.Cookies["emailAddress"].Value : "";
            EmailViewModel model = new EmailViewModel();
            model.FromEmail = currentEmail;
            return PartialView("CustomerServiceEmail", model);


        }

        public ActionResult ProcessCustomerServiceEmail(EmailViewModel model)
        {
            MailService email = new MailService();
            var messsage = "I would like to know my account number for access to solutions.starbucks.com. Please contact me at this email.";
            if (!String.IsNullOrEmpty(model.Message))
            {
                messsage = model.Message.ToString();
            }
            email.SendEmail("sjones@marlinco.com", model.FromEmail, "Starbucks Branded Solutions Account Inquiry", messsage, true);

            TempData["Success"] = true;

            return RedirectToCurrentUmbracoPage();
        }
    }
}