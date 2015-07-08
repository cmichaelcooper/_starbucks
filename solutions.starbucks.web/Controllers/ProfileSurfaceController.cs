using DotNetOpenAuth.OAuth2;
using Newtonsoft.Json;
using solutions.starbucks.Interfaces;
using solutions.starbucks.Model;
using solutions.starbucks.Model.Pocos;
using solutions.starbucks.web.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TheAlchemediaProject.Services;
using umbraco.cms.businesslogic.member;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Web.Mvc;
using uMember;

namespace solutions.starbucks.web.Controllers
{
    public class ProfileSurfaceController : SurfaceController
    {
        private readonly IMemberAttributesRepository _memberRepository;
        private readonly IOrdersRepository _orderRepository;

        public ProfileSurfaceController(IMemberAttributesRepository memberRepository, IOrdersRepository orderRepository)
        {
            _memberRepository = memberRepository;
            _orderRepository = orderRepository;
        }

        [Authorize]
        public ActionResult RenderEditProfile(string customerEmail)
        {
            ProfileViewModel profileModel = new ProfileViewModel();
            //If user is logged in then let's pre-populate the model
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                //Let's fill it up
                //Member currentMember = Member.GetCurrentMember();
                var currentProfile = Members.GetCurrentMemberProfileModel();
                var currentMember = Services.MemberService.GetByEmail(currentProfile.Email);
                if (currentMember != null)
                {
                    bool isPartnerAdmin = Roles.IsUserInRole(currentMember.Email, "PartnerAdmin");
                    bool isSuperPartnerAdmin = Roles.IsUserInRole(currentMember.Email, "SuperPartnerAdmin");
                    bool isPartner = Roles.IsUserInRole(currentMember.Email, "Partner");
                    bool isUnverified = Roles.IsUserInRole(currentMember.Email, "Unverified Customer");

                    TempData["InvalidAccount"] = isUnverified;
                    //Partner and super partner admins have special consideration that need made
                    if (isPartnerAdmin || isSuperPartnerAdmin)
                    {
                        TempData["IsCurrentUserPartnerAdmin"] = true;
                        if (!String.IsNullOrEmpty(customerEmail))
                        {
                            var checkMemberExists = Member.GetMemberFromEmail(customerEmail);
                            if (checkMemberExists != null)
                            {
                                TempData["Impersonation"] = true;
                            }
                            else
                            {
                                TempData["InvalidCustomerEmail"] = true;
                            }
                        }
                        else
                        {
                            TempData["Impersonation"] = false;
                        }
                    }
                    else
                    {
                        TempData["IsCurrentUserPartnerAdmin"] = false;
                    }

                    TempData["IsCurrentUserPartner"] = isPartner;

                    //Umbraco Specific member attributes
                    profileModel.Name = currentMember.Name;
                    profileModel.EmailAddress = currentMember.Email;
                    profileModel.MemberID = currentMember.Id;

                    //Custom defined attributes
                    profileModel.Title = currentMember.Properties["title"].Value.ToString();
                    //profileModel.Title = currentMember.getProperty("title").Value.ToString();
                    profileModel.Phone = currentMember.Properties["phoneNumber"].Value.ToString();
                    //profileModel.Phone = currentMember.getProperty("phoneNumber").Value.ToString();
                    profileModel.MobilePhone = currentMember.Properties["mobilePhone"].Value.ToString();
                    profileModel.Password = currentMember.RawPasswordValue;
                    profileModel.OptionalDetails = currentMember.Properties["optionalDetails"].Value.ToString();

                    if (!currentMember.Email.Contains("@starbucks.com"))
                    {
                        profileModel.CompanyName = currentMember.Properties["companyName"].Value.ToString();
                        profileModel.City = currentMember.Properties["city"].Value.ToString();
                        profileModel.State = currentMember.Properties["state"].Value.ToString();
                    }

                    var currentMemberType = currentMember.ContentType.Alias;
                    if (currentMemberType.ToLower() != "partner")
                    {
                        string attempts = currentMember.Properties["accountNumberAttempts"].Value.ToString();
                        if (!String.IsNullOrEmpty(currentMember.Properties["accountNumberAttemptsTimeout"].Value.ToString()))
                        {
                            DateTime lockedOutTime = DateTime.ParseExact(currentMember.Properties["accountNumberAttemptsTimeout"].Value.ToString(), "dd/MM/yyyy @ HH:mm:ss", null);
                            //DateTime lockedOutTime = Convert.ToDateTime(currentMember.getProperty("accountNumberAttemptsTimeout").Value.ToString());
                            if (DateTime.Now > lockedOutTime)
                            {
                                attempts = "0";
                                currentMember.Properties["accountNumberAttemptsTimeout"].Value = "";
                                currentMember.Properties["accountNumberAttempts"].Value = 0;
                                Services.MemberService.Save(currentMember);
                                //currentMember.Save();
                            }
                        }
                        if (String.IsNullOrEmpty(attempts))
                        {
                            attempts = "0";
                        }
                        //var memberAttributesRepository = new MemberAttributesRepository();
                        profileModel.Attempts = Convert.ToInt32(attempts);

                        int umbracoUserID = currentMember.Id;

                        var associatedMemberAccounts = _memberRepository.GetAssociatedAccount(umbracoUserID);
                        bool memberContains = associatedMemberAccounts.Any();
                        int userAccountCount = associatedMemberAccounts.Count();

                        var shippingAccountInfo = _memberRepository.GetCurrentMemberShipping(umbracoUserID);
                        
                        if (memberContains)
                        {
                            profileModel.AssociatedAccounts = associatedMemberAccounts;
                            profileModel.AccountShippingInformation = shippingAccountInfo;

                        }
                    }
                }
                else
                {
                    //member not found, clear session cookies
                    return new RedirectResult("/umbraco/Surface/AccountSurface/Logout");
                }

                //Pass the model to the view
                return PartialView("EditProfile", profileModel);
            }
            else
            {
                //They are not logged in, redirect to home
                return Redirect("/");
            }

        }

        [Authorize]
        public ActionResult RenderMemberList()
        {
            
            MemberReportViewModel memberViewModel = new MemberReportViewModel();
            var db = new uMembership();

            IEnumerable<Umbraco.Core.Models.IMember> unverifiedMembers = Services.MemberService.GetMembersByGroup("Unverified Customer");

            memberViewModel.ActiveMembers = Services.MemberService.GetMembersByGroup("Customer").Where(m => m.Properties["hasVerifiedEmail"].Value.ToString() == "1" && m.Properties["validatedAccountNumber"].Value.ToString() == "1");
            
            unverifiedMembers = Services.MemberService.GetMembersByGroup("Unverified Customer");
            memberViewModel.NonActiveMembers = Services.MemberService.GetMembersByGroup("Customer").Where(m => m.Properties["hasVerifiedEmail"].Value.ToString() != "1" || m.Properties["validatedAccountNumber"].Value.ToString() != "1").Union(unverifiedMembers);

            memberViewModel.InactiveMembers = Services.MemberService.GetMembersByGroup("Inactive");
            memberViewModel.PartnerMembers = Services.MemberService.GetMembersByGroup("Partner");

            IEnumerable<MemberIdWithAssociatedAccounts> associatedMemberAccounts = _memberRepository.GetAllAssociatedAccounts();
            memberViewModel.AssociatedAccounts =  associatedMemberAccounts.ToDictionary(a => a.MemberId, a => a.AssociatedAccounts);

            return PartialView("MemberListHandle", memberViewModel);
        }

        //UmbracoDatabase _db = ApplicationContext.Current.DatabaseContext.Database;

        [Authorize]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult ProcessProfileEdit(ProfileViewModel model, CustomerAttributes[] custAtt)
        {
            Member updateMember = new Member(model.MemberID);

            MailService email = new MailService();
            
            if(String.IsNullOrEmpty(model.AccountNumber) && String.IsNullOrEmpty(model.Zip) && String.IsNullOrEmpty(model.Password))
            {
                UmbracoDatabase dbConnect = ApplicationContext.Current.DatabaseContext.Database;

                string updateSQL = " set nickname = @0 where accountNumber = @1 and AccountSiteNumber = @2 and PostalCode = @3";
                //string updateSQL = "select * from AssociatedMemberAccounts where umbracouserid = @0 and accountNumber = @1";
                for (int i=0; i < custAtt.Length; i++)
                {
                    if (Request.Form.AllKeys.Contains("nick-"+i.ToString()))
                    {
                        var a = dbConnect.Update<CustomerAttributes>(updateSQL, custAtt[i].Nickname.Trim(), custAtt[i].AccountNumber, custAtt[i].AccountSiteNumber, custAtt[i].PostalCode);
                    }
                }
                //var a = _db.SingleOrDefault<CustomerAttributes>(updateSQL, model.MemberID, custAtt[0].AccountNumber);
                try
                {

                    var messsage = string.Format(
                                            "<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'> <html xmlns='http://www.w3.org/1999/xhtml'> <head> <meta http-equiv='Content-Type' content='text/html; charset=UTF-8' /> <title>Profile Update Successful</title> </head> <body style='-webkit-text-size-adjust: none;'> <table align='center' width='600' border='0' cellspacing='0' cellpadding='0'> <tr> <td colspan='2' bgcolor='#3E3935' height='90' style='text-align:left;'><p style='margin-left:18px; margin-top:18px; margin-bottom:18px; margin-right:18px'><img src='http://marlinco.com/eblast/sbs/system_emails/branded_solutions_logo.png' width='223' height='54'/></p></td> </tr> <tr> <td bgcolor='#FFFFFF' colspan='2' width='600' style='text-align:left;'><p style='text-align:left; font-family:Corbel, sans-serif; font-size:25px; font-weight:normal; color:#3E3935; margin-left:18px; margin-right:18px; margin-top:30px;'>Your profile was successfully updated for Starbucks Branded Solutions.</p> <p style='text-align:left; font-family:Corbel, sans-serif; font-size:17px; font-weight:normal; color:#3E3935; margin-left:18px; margin-right:18px; margin-bottom:30px;'>Please contact your representative if you did not initiate this process. If you suspect unauthorized access, you may wish to notify us of this activity here: <span style='color:#63A70A;'>fsinsidesales@starbucks.com</span>.<br> <br> Thanks again,<br> Starbucks Branded Solutions</p></td> </tr> </table></body> </html>");
                    email.SendEmail(updateMember.Email, "no-reply@starbucksfs.com", "Profile change for Starbucks Branded Solutions", messsage, true);
                }
                catch (Exception e)
                {

                }


                TempData["CustomerEmail"] = updateMember.Email;

                TempData["Success"] = true;
                return RedirectToCurrentUmbracoPage();
            }

            updateMember.getProperty("title").Value = model.Title;
            updateMember.getProperty("phoneNumber").Value = model.Phone;
            updateMember.getProperty("mobilePhone").Value = model.MobilePhone;
            updateMember.getProperty("optionalDetails").Value = model.OptionalDetails;
            int attempts = Convert.ToInt32(model.Attempts);

            if (Roles.IsUserInRole(updateMember.LoginName, "Inactive"))
            {

            }
            if (!String.IsNullOrEmpty(model.Password) && !String.IsNullOrEmpty(model.NewPassword))
            {
                if (!Membership.ValidateUser(updateMember.Email, model.Password))
                {
                    ModelState.AddModelError("Password Mismatch", "Old password does not match your current password. Please try again.");
                    return RedirectToCurrentUmbracoPage();
                }
                else
                {
                    updateMember.Password = model.NewPassword;
                }
            }

            updateMember.Text = model.Name;
            if (!model.EmailAddress.Contains("@starbucks.com"))
            {
                if (!String.IsNullOrEmpty(model.AccountNumber) && !String.IsNullOrEmpty(model.Zip))
                {
                    bool accountNumberValid = CheckAccountNumber(model.AccountNumber, model.Zip);
                    bool currentValidAccount = Convert.ToBoolean(updateMember.getProperty("validatedAccountNumber").Value);

                    if (!accountNumberValid && !currentValidAccount)
                    {

                        attempts++;
                        updateMember.getProperty("accountNumberAttempts").Value = attempts;


                        TempData["AccountValidCount"] = attempts;

                        if (attempts == 3)
                        {
                            updateMember.getProperty("accountNumberAttemptsTimeout").Value = DateTime.Now.AddMinutes(30).ToString("dd/MM/yyyy @ HH:mm:ss");
                        }
                        ModelState.AddModelError("Invalid account", "You must enter a valid account number and zip before you can modify your account");
                        updateMember.Save();
                        TempData["CustomerEmail"] = updateMember.Email;
                        return new RedirectResult("/profile/edit#Account");
                    }
                    var memberUnverified = Roles.IsUserInRole(updateMember.LoginName, "Unverified Customer");

                    if (memberUnverified)
                    {
                        MemberGroup UnverifiedGroup = MemberGroup.GetByName("Unverified Customer");
                        MemberGroup VerifiedGroup = MemberGroup.GetByName("Customer");

                        updateMember.getProperty("validatedAccountNumber").Value = true;
                        updateMember.RemoveGroup(UnverifiedGroup.Id);
                        updateMember.AddGroup(VerifiedGroup.Id);

                    }

                    updateMember.getProperty("accountNumber").Value = model.AccountNumber;
                    updateMember.getProperty("zip").Value = model.Zip;
                }
                updateMember.getProperty("city").Value = model.City;
                updateMember.getProperty("state").Value = model.State;
                updateMember.getProperty("companyName").Value = model.CompanyName;
            }
            if (!String.IsNullOrEmpty(model.AccountNumber) && !String.IsNullOrEmpty(model.Zip))
            {
                var accountNumberAddition = new AssociatedMemberAccount();
                accountNumberAddition.AccountID = model.MemberID;
                accountNumberAddition.UmbracoUserID = model.MemberID;

                accountNumberAddition.AccountNumber = model.AccountNumber;
                accountNumberAddition.Zip = model.Zip;

                bool userAccount = _memberRepository.GetMemberWithAccountZipAndId(model.MemberID.ToString(), model.AccountNumber, model.Zip).Any();

                if (userAccount)
                {

                    ModelState.AddModelError("DuplicateAccount", "This account is already part of the profile. If you feel this is in error, please contact Starbucks Customer Service.");
                    return RedirectToCurrentUmbracoPage();
                }
                bool isAccount = _memberRepository.GetMemberWithAccountNumberAndZip(model.AccountNumber, model.Zip).Any();

                if (isAccount)
                {
                    //Insert AssociatedMemberAccount
                    updateMember.getProperty("validatedAccountNumber").Value = true;
                    updateMember.getProperty("accountNumberAttempts").Value = 0;
                    updateMember.Save();
                    var userBrand = _memberRepository.GetMemberBrand(model.AccountNumber, model.Zip);
                    var userBrandInfo = userBrand.Brands;
                    if (userBrandInfo != "SBUX" && userBrandInfo != "SBC")
                    {
                        TempData["BrandInfo"] = "Dual";
                    }
                    else
                    {
                        TempData["BrandInfo"] = userBrandInfo;
                    }
                    _memberRepository.InsertAssociatedAccount(accountNumberAddition);
                }
                else
                {
                    attempts++;
                    TempData["AccountValidCount"] = attempts;

                    updateMember.getProperty("accountNumberAttempts").Value = attempts;
                    if (attempts == 3)
                    {
                        updateMember.getProperty("accountNumberAttemptsTimeout").Value = DateTime.Now.AddMinutes(30).ToString("dd/MM/yyyy @ HH:mm:ss");
                    }

                    ModelState.AddModelError("Invalid account", "Invalid account / zip combination.");
                    updateMember.Save();

                    return new RedirectResult("/profile/edit#Account");
                }
            }

            updateMember.Save();

            try
            {

                var messsage = string.Format(
                                        "<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'> <html xmlns='http://www.w3.org/1999/xhtml'> <head> <meta http-equiv='Content-Type' content='text/html; charset=UTF-8' /> <title>Profile Update Successful</title> </head> <body style='-webkit-text-size-adjust: none;'> <table align='center' width='600' border='0' cellspacing='0' cellpadding='0'> <tr> <td colspan='2' bgcolor='#3E3935' height='90' style='text-align:left;'><p style='margin-left:18px; margin-top:18px; margin-bottom:18px; margin-right:18px'><img src='http://marlinco.com/eblast/sbs/system_emails/branded_solutions_logo.png' width='223' height='54'/></p></td> </tr> <tr> <td bgcolor='#FFFFFF' colspan='2' width='600' style='text-align:left;'><p style='text-align:left; font-family:Corbel, sans-serif; font-size:25px; font-weight:normal; color:#3E3935; margin-left:18px; margin-right:18px; margin-top:30px;'>Your profile was successfully updated for Starbucks Branded Solutions.</p> <p style='text-align:left; font-family:Corbel, sans-serif; font-size:17px; font-weight:normal; color:#3E3935; margin-left:18px; margin-right:18px; margin-bottom:30px;'>Please contact your representative if you did not initiate this process. If you suspect unauthorized access, you may wish to notify us of this activity here: <span style='color:#63A70A;'>fsinsidesales@starbucks.com</span>.<br> <br> Thanks again,<br> Starbucks Branded Solutions</p></td> </tr> </table></body> </html>");
                email.SendEmail(updateMember.Email, "no-reply@starbucksfs.com", "Profile change for Starbucks Branded Solutions", messsage, true);
            }
            catch (Exception e)
            {

            }


            TempData["CustomerEmail"] = updateMember.Email;

            TempData["Success"] = true;
            return RedirectToCurrentUmbracoPage();

        }

        [HttpGet]
        public JsonNetResult GetProfile(string ReturningTo)
        {
            ResourceServer server = new ResourceServer(new StandardAccessTokenAnalyzer((RSACryptoServiceProvider)MvcApplication.SigningCertificate.PublicKey.Key, (RSACryptoServiceProvider)MvcApplication.EncryptionCertificate.PrivateKey));
            AccessToken token = server.GetAccessToken();

            var currentProfile = Members.GetCurrentMemberProfileModel();
            var currentMember = Services.MemberService.GetById(Convert.ToInt32(token.User));
            //bool isPartner = Roles.IsUserInRole(currentMember.Email, "Partner") || Roles.IsUserInRole(currentMember.Email, "PartnerAdmin") || Roles.IsUserInRole(currentMember.Email, "SuperPartnerAdmin") ? true : false;

            string brand = "SBUX";
            string brandUrl = "google.com";
            Orders orderInBag = _orderRepository.GetOrderInBag();

            JsonNetResult jsonNetResult = new JsonNetResult();
            jsonNetResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonNetResult.Formatting = Formatting.Indented;
            jsonNetResult.Data = new
            {
                EmailAddress = currentMember.Email,
                AccountNumber = _memberRepository.GetFirstAccountReference(Convert.ToInt32(token.User)).AccountNumber,
                Brand = brand,
                ReturningTo = brandUrl,
                OrderId = orderInBag.OrderID.ToString()
            };

            return jsonNetResult;
        }


        public ActionResult ProcessMemberImpersonation(string CustomerEmail)
        {
            TempData["CustomerEmail"] = CustomerEmail;

            return RedirectToCurrentUmbracoPage();
        }

        public ActionResult ProcessMemberInactivation(string customerEmail)
        {
            var inactiveMember = Member.GetMemberFromEmail(customerEmail);

            bool currentCustomer = Roles.IsUserInRole(inactiveMember.LoginName, "Customer");
            bool currentUnverfiedCustomer = Roles.IsUserInRole(inactiveMember.LoginName, "Unverified Customer");
            bool currentPartner = Roles.IsUserInRole(inactiveMember.LoginName, "Partner");
            bool currentPartnerAdmin = Roles.IsUserInRole(inactiveMember.LoginName, "PartnerAdmin");
            bool currentSuperPartnerAdmin = Roles.IsUserInRole(inactiveMember.LoginName, "SuperPartnerAdmin");

            if (currentCustomer)
            {
                InactivateMember(inactiveMember, "Customer", customerEmail);
            }
            if (currentUnverfiedCustomer)
            {
                InactivateMember(inactiveMember, "Unverified Customer", customerEmail);
            }
            if (currentPartner)
            {
                InactivateMember(inactiveMember, "Partner", customerEmail);
            }
            if (currentPartnerAdmin)
            {
                InactivateMember(inactiveMember, "PartnerAdmin", customerEmail);
            }
            TempData["CustomerEmail"] = customerEmail;

            return RedirectToCurrentUmbracoPage();


        }

        private void InactivateMember(Member currentMember, string memberGroup, string customerEmail)
        {


            MemberGroup CustomerGroup = MemberGroup.GetByName(memberGroup);
            MemberGroup InactiveGroup = MemberGroup.GetByName("Inactive");

            //updateMember.ContentType.Alias = "Customer";
            currentMember.RemoveGroup(CustomerGroup.Id);
            currentMember.AddGroup(InactiveGroup.Id);

            currentMember.Save();



        }

        public ActionResult ProcessResendEmail(string customerEmail)
        {
            var member = Services.MemberService.GetByEmail(customerEmail);
            Umbraco.Core.Models.IMember memberToSave = member;
            var checkGuid = memberToSave.Properties["emailVerifyGUID"].Value.ToString();
            var emailFrom = "no-reply@starbucksfs.com";
            var emailSubject = "Starbucks Branded Solutions - Verify Email";
            MailService email = new MailService();
            //string baseURL = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Replace(System.Web.HttpContext.Current.Request.Url.AbsolutePath, string.Empty);
            string baseURL = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
            var verifyURL = baseURL + "/verify-user?verifyGUID=" + checkGuid;

            var messsage = string.Format(
          "<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'> <html xmlns='http://www.w3.org/1999/xhtml'> <head> <meta http-equiv='Content-Type' content='text/html; charset=UTF-8' /> <title>Profile Created</title> </head> <body style='-webkit-text-size-adjust: none;'> <table align='center' width='600' border='0' cellspacing='0' cellpadding='0'> <tr> <td style='text-align:left;' colspan='2' bgcolor='#3E3935' height='90'><p style='margin-left:18px; margin-top:18px; margin-bottom:18px; margin-right:18px'><img src='http://marlinco.com/eblast/sbs/system_emails/branded_solutions_logo.png' width='223' height='54'/></p></td> </tr> <tr> <td style='text-align:left;' bgcolor='#FFFFFF' colspan='2' width='600'><p style='text-align:left; font-family:Corbel, sans-serif; font-size:25px; font-weight:normal; color:#3E3935; margin-left:18px; margin-right:18px; margin-top:30px;'>Thank you for creating your profile on Starbucks Branded Solutions.</p> <p style='margin-top:0px; margin-right:0px; margin-bottom:0px; margin-left:18px;'><a style='text-decoration:none; border:0;' href='{0}'><img src='http://marlinco.com/eblast/sbs/system_emails/click_to_confirm_button.png' width='180' height='35' alt='' style='border:0;'/></a></p> <p style='text-align:left; font-family:Corbel, sans-serif; font-size:18px; font-weight:normal; color:#3E3935; margin-left:18px; margin-right:18px; margin-bottom:30px;'>If you did not initiate this process, you can ignore this email. If concerned, you may wish to notify us of this activity here: <span style='color:#63A70A;'>fsinsidesales@starbucks.com</span>.<br> <br> Thanks again,<br> Starbucks Branded Solutions</p> </td> </tr> </table> </body> </html>",
          verifyURL);
            email.SendEmail(customerEmail, emailFrom, emailSubject, messsage, true);
            TempData["CustomerEmail"] = customerEmail;
            return RedirectToCurrentUmbracoPage();
        }

        public ActionResult ProcessVerifyEmail(string customerEmail)
        {
            var member = Services.MemberService.GetByEmail(customerEmail);
            Umbraco.Core.Models.IMember memberToSave = member;
            member.SetValue("hasVerifiedEmail", true);
            Services.MemberService.Save(member);
            return RedirectToCurrentUmbracoPage();
        }

        public ActionResult ProcessMemberActivation(string customerEmail)
        {
            var activeMember = Member.GetMemberFromEmail(customerEmail);

            if (customerEmail.Contains("@starbucks.com"))
            {
                bool currentInactivePartner = Roles.IsUserInRole(activeMember.LoginName, "Inactive");
                if (currentInactivePartner)
                {
                    ActivateMember(activeMember, "Partner", customerEmail);


                }

                return RedirectToCurrentUmbracoPage();
            }

            bool currentInactiveCustomer = Roles.IsUserInRole(activeMember.LoginName, "Inactive");

            if (currentInactiveCustomer)
            {
                ActivateMember(activeMember, "Customer", customerEmail);
            }
            TempData["CustomerEmail"] = customerEmail;
            return RedirectToCurrentUmbracoPage();
        }

        private void ActivateMember(Member currentMember, string memberGroup, string customerEmail)
        {


            MemberGroup ActiveGroup = MemberGroup.GetByName(memberGroup);
            MemberGroup InactiveGroup = MemberGroup.GetByName("Inactive");

            //updateMember.ContentType.Alias = "Customer";
            currentMember.RemoveGroup(InactiveGroup.Id);
            currentMember.AddGroup(ActiveGroup.Id);

            currentMember.Save();

        }

        public JsonResult CheckProfileEmailIsUsed(string emailAddress)
        {
            //Get Current Member
            var member = Member.GetCurrentMember();

            if (member != null)
            {
                //if the email is the same as the one stored then we're good
                if (member.Email == emailAddress)
                {
                    //Email is the same as one currently stored on the member - so email ok to use & rule valid (return true, back to validator)
                    return Json(true, JsonRequestBehavior.AllowGet);
                }

                //Try and get member by email typed in
                var checkEmail = Member.GetMemberFromEmail(emailAddress);

                if (checkEmail != null)
                {
                    return Json(String.Format("The email address '{0}' is already in use.", emailAddress), JsonRequestBehavior.AllowGet);
                }

                return Json(true, JsonRequestBehavior.AllowGet);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public bool CheckAccountNumber(string accountNumber, string zip)
        {
            bool userAccount = _memberRepository.GetMemberWithAccountNumberAndZip(accountNumber, zip).Any();
            int userAccountCount = _memberRepository.GetMemberWithAccountNumberAndZip(accountNumber, zip).Count();

            //Account/zip combination exists, ready to add to profile
            if (userAccountCount > 0 && userAccount)
            {
                return true;
            }

            return false;
        }

        public JsonResult CheckOldPassword(ProfileViewModel model)
        {
            var member = Member.GetCurrentMember();

            if (member != null)
            {
                if (Membership.ValidateUser(member.Email, model.Password))
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }

            }

            return Json(String.Format("The previous password field doesn't match your current password. Please try again. "), JsonRequestBehavior.AllowGet);

        }
    }
}