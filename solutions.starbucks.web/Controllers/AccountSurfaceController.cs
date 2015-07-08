using solutions.starbucks.Interfaces;
using solutions.starbucks.Model;
using solutions.starbucks.web.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Services;
using TheAlchemediaProject.Services;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace solutions.starbucks.web.Controllers
{
    public class AccountSurfaceController : SurfaceController
    {
        
        //protected MemberAttributesRepository _memberRepository;
        private readonly IMemberAttributesRepository _memberRepository;

        public AccountSurfaceController(IMemberAttributesRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        public ActionResult RenderLogin(string ReturnUrl, string verifyGUID)
        {
            var loginModel = new LoginViewModel();
           
            if (!String.IsNullOrEmpty(verifyGUID))
            {
                TempData["VerifyUserCreds"] = true;
            }
            ModelState.Clear();
            return PartialView("LoginHandle", loginModel);
        }

        public ActionResult RenderStart()
        {
            return PartialView("GetStartedHandle");
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public JsonResult ProcessLogin(LoginViewModel model)
        {

            var returningTo = HttpUtility.UrlDecode(model.ReturnUrl);
            var returningToParsed = HttpUtility.ParseQueryString(returningTo);
            string retValue = "";
            if (!ModelState.IsValid)
            {
                retValue = "Invalid username or password";
                Response.StatusCode = 400;
                Response.StatusDescription = retValue;
                ModelState.AddModelError("LoginForm.", retValue);
                return Json(retValue, JsonRequestBehavior.AllowGet);
            }

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                 return Json(true, JsonRequestBehavior.AllowGet);
            }

            try
            {
                bool invalidCustomer = Roles.IsUserInRole(model.EmailAddress, "Inactive");
                if (invalidCustomer)
                {
                    retValue = "This account has been deactivated. Please <a href='/" + @Umbraco.GetDictionaryValue("contact-form") + "'>contact your customer service representative</a> if you feel this is in error. ";
                    Response.StatusCode = 400;
                    Response.StatusDescription = retValue;
                    ModelState.AddModelError("LoginForm.", retValue);
                    return Json(retValue, JsonRequestBehavior.AllowGet);
                }
                //TODO: Create password expiration variable and force user to change password on expiration.
                if (Membership.ValidateUser(model.EmailAddress, model.Password))
                {
                    var member = Services.MemberService.GetByEmail(model.EmailAddress);
                    IMember memberToSave = member ;
                    if (member != null)
                    {
                        if (Convert.ToBoolean(member.Properties["hasVerifiedEmail"].Value))
                        {
                            int loginNumber = 0;
                            if (int.TryParse(member.Properties["numberOfLogins"].Value.ToString(), out loginNumber))
                            {

                            }
                            memberToSave.SetValue("numberOfLogins", loginNumber + 1);
                            memberToSave.SetValue("lastLoggedIn", DateTime.Now.ToString("dd/MM/yyyy @ HH:mm:ss"));
                            //member.Properties["lastLoggedIn"].Value = DateTime.Now.ToString("dd/MM/yyyy @ HH:mm:ss");
                            string hostName = Dns.GetHostName();
                            string clientIP = Dns.GetHostAddresses(hostName).Where(h => h.AddressFamily == AddressFamily.InterNetwork).FirstOrDefault().ToString();
                            memberToSave.SetValue("hostNameOfLastLogin", hostName);
                            //member.Properties["hostNameOfLastLogin"].Value = hostName;
                            memberToSave.SetValue("IPOfLastLogin", GetVisitorIPAddress());
                            //member.Properties["IPOfLastLogin"].Value = clientIP;

                            Services.MemberService.Save(memberToSave);

                            if (Convert.ToBoolean(memberToSave.Properties["validatedAccountNumber"].Value))
                            {
                                bool rememberMe = false;
                                if (model.RememberMe.HasValue)
                                {
                                    rememberMe = true;
                                }
                                FormsAuthentication.SetAuthCookie(model.EmailAddress, rememberMe);
                                var customerType = "operator";
                                if (AccountHelpers.DetermineIsPartner(member.Email))
                                {
                                    customerType = "partner";
                                }
                                AccountHelpers.SetCurrentAccountSessionVars("", member, _memberRepository);
                                return Json(new
                                {
                                    redirectUrl = Url.Action(model.ReturnUrl),
                                    isRedirect = true
                                });
                                //return Json(customerType, JsonRequestBehavior.AllowGet);

                            }
                            else
                            {
                                FormsAuthentication.SetAuthCookie(model.EmailAddress, true);
                                return Json("You are logged in but you have not added a valid account/zip to your profile.", JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("LoginForm.", "Email account hasn't been verified. Verification email has been resent.");

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
                            email.SendEmail(model.EmailAddress, emailFrom, emailSubject, messsage, true);
                            Response.StatusCode = 400;
                            retValue = "Email account hasn't been verified. Verification email has been resent.";
                            Response.StatusDescription = retValue;
                            return Json(retValue, JsonRequestBehavior.AllowGet);
                        }

                    }
                }
                else
                {
                    ModelState.AddModelError("LoginForm.", "Invalid details");
                    retValue = "Invalid email address/password. Please try again.";
                    Response.StatusCode = 400;
                    Response.StatusDescription = retValue;
                    return Json(retValue, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("LoginForm.", "Invalid details");
                retValue = "Invalid email address/password. Please try again.";
                Response.StatusCode = 400;
                Response.StatusDescription = retValue;
                return Json(retValue, JsonRequestBehavior.AllowGet);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Logout()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                Session.Abandon();
                FormsAuthentication.SignOut();
                Response.Cookies["AccountAttributes"].Expires = DateTime.Now.AddDays(-1);
                return Redirect("/");                
            }
            else
            {
                return Redirect("/");
            }
        }

        public ActionResult RenderForgottenPassword()
        {
            return PartialView("ForgottenPasswordHandle", new ForgottenPasswordViewModel());
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult ProcessForgottenPassword(ForgottenPasswordViewModel model)
        {
            string retValue = "There was an error submitting the form, please try again later.";
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 400;
                ModelState.AddModelError("ForgotPassword1", "Incorrect Email");
                return Content(retValue) ;
            }

            IMember member = Services.MemberService.GetByEmail(model.EmailAddress);
            
            if (member != null)
            {
                DateTime expireTime = DateTime.Now.AddHours(2);

                member.SetValue("resetGUID", expireTime.ToString("ddMMyyyyHHmmssFFFF"));

                Services.MemberService.Save(member);

                var emailFrom = "no-reply@starbucksfs.com";
                var emailSubject = "Forgotten password request";
                MailService email = new MailService();
                string baseURL = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
                var resetURL = baseURL + "/reset-password?resetGUID=" + expireTime.ToString("ddMMyyyyHHmmssFFFF");
                var messsage = string.Format(
                                        "<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'> <html xmlns='http://www.w3.org/1999/xhtml'> <head> <meta http-equiv='Content-Type' content='text/html; charset=UTF-8' /> <title>Password Reset</title> </head> <body style='-webkit-text-size-adjust: none;'> <table align='center' width='600' border='0' cellspacing='0' cellpadding='0'> <tr> <td style='text-align:left;' colspan='2' bgcolor='#3E3935' height='90'><p style='margin-left:18px; margin-top:18px; margin-bottom:18px; margin-right:18px'><img src='http://marlinco.com/eblast/sbs/system_emails/branded_solutions_logo.png' width='223' height='54'/></p></td> </tr> <tr> <td bgcolor='#FFFFFF' colspan='2' width='600' style='text-align:left;'><p style='text-align:left; font-family:Corbel, sans-serif; font-size:25px; font-weight:normal; color:#3E3935; margin-left:18px; margin-right:18px; margin-top:30px;'>A forgotten password request has been made for your account on Starbucks Branded Solutions. </p> <p style='margin-top:0px; margin-right:0px; margin-bottom:0px; margin-left:18px;'><a href='{0}'><img src='http://marlinco.com/eblast/sbs/system_emails/reset_your_password_button.png' width='210' height='35' alt='' style='border:0;' /></a></p> <p style='text-align:left; font-family:Corbel, sans-serif; font-size:18px; font-weight:normal; color:#3E3935; margin-left:18px; margin-right:18px; margin-bottom:30px;'>If you did not initiate this process, you can ignore this email. If concerned, you may wish to notify us of this activity here: <span style='color:#63A70A;'>fsinsidesales@starbucks.com</span>.<br> <br> Thanks again,<br> Starbucks Branded Solutions</p> </td> </tr> </table>  </body> </html>",
                                        resetURL);
                try
                {
                    email.SendEmail(model.EmailAddress, emailFrom, emailSubject, messsage, true);
                }
                catch (Exception e)
                {
                    retValue = "There was an error sending the email. Please try again later. If this problem persists, please contact Starbucks Customer Service. ";
                    ModelState.AddModelError("ForgottenPasswordForm.", retValue);
                    Response.StatusCode = 400;
                    Response.StatusDescription = retValue;
                    retValue = "Invalid Details";
                }
            }
            else
            {
                ModelState.AddModelError("ForgottenPasswordForm.", "No member found with that email address.");
                Response.StatusCode = 400;
                retValue = "Invalid Details";
                return Content(retValue);
            }
            //TempData["ResetSuccess"] = true;
            Response.StatusCode = 200;
            Response.StatusDescription = model.EmailAddress.ToString() ;
            return Content(model.EmailAddress.ToString());
        }

        public ActionResult RenderContactForm()
        {
            ContactViewModel model = new ContactViewModel();
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var profileModel = Members.GetCurrentMemberProfileModel();
                var currentMember = Services.MemberService.GetByEmail(profileModel.Email);
                model.EmailAddress = currentMember.Email;
                model.Name = currentMember.Name;
                model.Phone = currentMember.Properties["phoneNumber"].Value.ToString();
                //model.Phone = currentMember.getProperty("phoneNumber").Value.ToString();

                model.AccountNumber = currentMember.Properties["accountNumber"].Value.ToString(); ;
                if (!Roles.IsUserInRole(currentMember.Email, "Partner") && !Roles.IsUserInRole(currentMember.Email, "PartnerAdmin") && !Roles.IsUserInRole(currentMember.Email, "SuperPartnerAdmin"))
                {
                    model.BusinessName = currentMember.Properties["companyName"].Value.ToString();
                }
                else
                {
                    model.BusinessName = "Starbucks";
                }
                
            }
            model.CurrentForm = Request.RawUrl.ToString();
            return PartialView("ContactFormHandle", model);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult ProcessContactForm(ContactViewModel model, FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                var emailFrom = "no-reply@starbucksfs.com";
                var emailSubject = "Starbucks Branded Solutions Contact Request";
                var accountNumber = "(Nothing Entered)";
                try
                {
                    var challengeField = collection["recaptcha_challenge_field"];
                    var responseField = collection["recaptcha_response_field"];
                    bool actualHuman = ValidateCaptcha(challengeField, responseField);

                    if (!actualHuman)
                    {
                        ModelState.AddModelError("SpamBot", "SpamBot");
                        return CurrentUmbracoPage();
                    }
                    
                }
                catch (Exception e)
                {

                }

                if (!String.IsNullOrEmpty(model.AccountNumber))
                {
                    accountNumber = model.AccountNumber;
                }
                MailService email = new MailService();
                var messsage = string.Format(
                                        "<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'> <html xmlns='http://www.w3.org/1999/xhtml'> <head> <meta http-equiv='Content-Type' content='text/html; charset=UTF-8' /> <title>Customer request for information</title> </head> <body style='-webkit-text-size-adjust: none;'> <table align='center' width='600' border='0' cellspacing='0' cellpadding='0'> <tr> <td style='text-align:left;' colspan='2' bgcolor='#3E3935' height='90'><p style='margin-left:18px; margin-top:18px; margin-bottom:18px; margin-right:18px'><img src='http://marlinco.com/eblast/sbs/system_emails/branded_solutions_logo.png' width='223' height='54'/></p></td> </tr> <tr> <td style='text-align:left;' bgcolor='#FFFFFF' colspan='2'><p style='text-align:left; font-family:Corbel, sans-serif; font-size:25px; font-weight:normal; color:#3E3935; margin-left:18px; margin-right:18px; margin-top:15px; margin-bottom:0px;'>A customer has submitted a request for a representative to contact them in regards to the Starbucks Branded Solutions Website.</p> <p style='text-align:left; font-family:Corbel, sans-serif; font-size:17px; font-weight:normal; color:#3E3935; margin-left:18px; margin-right:18px; margin-top:15px; margin-bottom:15px;'>Please contact the customer listed below within one business day with the needed information.</p> </td> </tr> <tr> <td style='text-align:left;' width='250'> <p style='margin-top:0; margin-bottom:0;padding:0;margin-left:18px; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> CUSTOMER NAME: </p> </td> <td style='text-align:left;' width='350'> <p style='margin-top:0; margin-bottom:0;padding:0;margin-left:18px; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> {0} </td> <tr> <td style='text-align:left;' width='250'> <p style='margin-top:0; margin-bottom:0; padding:0; margin-left:18px; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> CUSTOMER EMAIL: </p> </td> <td style='text-align:left;' width='350'> <p style='margin-top:0; margin-bottom:0;margin-left:18px;padding:0; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> {1} </td> </tr> <tr> <td style='text-align:left;' valign='top' width='250'> <p style='margin-top:0; margin-bottom:0;padding:0;margin-left:18px; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> BUSINESS NAME: </p> </td> <td style='text-align:left;' width='350'> <p style='margin-top:0; margin-bottom:0;padding:0;margin-left:18px; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> {2} </td> </tr> <tr> <td style='text-align:left;' valign='top' width='250'> <p style='margin-top:0; margin-bottom:0;padding:0;margin-left:18px; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> CUSTOMER PHONE: </p> </td> <td style='text-align:left;' width='350'> <p style='margin-top:0; margin-bottom:0;padding:0;margin-left:18px; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> {3} </td> </tr> <tr> <td style='text-align:left;' valign='top' width='250'> <p style='margin-top:0; margin-bottom:0;padding:0;margin-left:18px; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> CUSTOMER ACCOUNT NUMBER: </p> </td> <td style='text-align:left;' width='350'> <p style='margin-top:0; margin-bottom:0;padding:0;margin-left:18px; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> {4} </td> </tr> <tr> <td style='text-align:left;' valign='top' width='250'> <p style='margin-top:0; margin-left:18px; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> REASON FOR REQUEST: </p> </td> <td style='text-align:left;' width='350'> <p style='margin-top:0; margin-left:18px; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;margin-bottom: 15px;'> {5} </td> </tr> </table> </body> </html>",
                                        model.Name, model.EmailAddress, model.BusinessName, model.Phone, accountNumber, model.Message);

                if (model.CurrentForm.Contains("forgot-account"))
                {
                    emailSubject = "Starbucks Branded Solutions Account/Zip Inquiry";
                    messsage = string.Format(
                                        "<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'> <html xmlns='http://www.w3.org/1999/xhtml'> <head> <meta http-equiv='Content-Type' content='text/html; charset=UTF-8' /> <title>Customer request for information</title> </head> <body style='-webkit-text-size-adjust: none;'> <table align='center' width='600' border='0' cellspacing='0' cellpadding='0'> <tr> <td style='text-align:left;' colspan='2' bgcolor='#3E3935' height='90'><p style='margin-left:18px; margin-top:18px; margin-bottom:18px; margin-right:18px'><img src='http://marlinco.com/eblast/sbs/system_emails/branded_solutions_logo.png' width='223' height='54'/></p></td> </tr> <tr> <td style='text-align:left;' bgcolor='#FFFFFF' colspan='2'><p style='text-align:left; font-family:Corbel, sans-serif; font-size:25px; font-weight:normal; color:#3E3935; margin-left:18px; margin-right:18px; margin-top:15px; margin-bottom:0px;'>A customer has submitted a request for his/her account number/zip code information for the Starbucks Branded Solutions Website. </p> <p style='text-align:left; font-family:Corbel, sans-serif; font-size:17px; font-weight:normal; color:#3E3935; margin-left:18px; margin-right:18px; margin-top:15px; margin-bottom:15px;'>Please contact the customer listed below within one business day with the needed information.</p> </td> </tr> <tr> <td style='text-align:left;' width='250'> <p style='margin-top:0; margin-bottom:0;padding:0;margin-left:18px; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> CUSTOMER NAME: </p> </td> <td style='text-align:left;' width='350'> <p style='margin-top:0; margin-bottom:0;padding:0;margin-left:18px; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> {0} </td> <tr> <td style='text-align:left;' width='250'> <p style='margin-top:0; margin-bottom:0; padding:0; margin-left:18px; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> CUSTOMER EMAIL: </p> </td> <td style='text-align:left;' width='350'> <p style='margin-top:0; margin-bottom:0;margin-left:18px;padding:0; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> {1} </td> </tr> <tr> <td style='text-align:left;' valign='top' width='250'> <p style='margin-top:0; margin-bottom:0;padding:0;margin-left:18px; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> BUSINESS NAME: </p> </td> <td style='text-align:left;' width='350'> <p style='margin-top:0; margin-bottom:0;padding:0;margin-left:18px; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> {2} </td> </tr> <tr> <td style='text-align:left;' valign='top' width='250'> <p style='margin-top:0; margin-bottom:0;padding:0;margin-left:18px; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> CUSTOMER PHONE: </p> </td> <td style='text-align:left;' width='350'> <p style='margin-top:0; margin-bottom:0px;padding:0;margin-left:18px; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> {3} </td> </tr> <tr> <td style='text-align:left;' valign='top' width='250'> <p style='margin-top:0; margin-bottom:15px;margin-left:18px; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> REASON FOR REQUEST: </p> </td> <td style='text-align:left;' width='350'> <p style='margin-top:0; margin-bottom:15px;margin-left:18px; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> {4} </td> </tr> </table></body> </html>",
                                        model.Name, model.EmailAddress, model.BusinessName, model.Phone, model.Message);
                }

                try
                {
                    email.SendEmail("fsinsidesales@starbucks.com, sbxsolutions@marlinco.com", emailFrom, emailSubject, messsage, true);
                    TempData["Success"] = true;
                }
                catch (Exception e)
                {
                }

                RedirectToCurrentUmbracoPage();
            }
            return RedirectToCurrentUmbracoPage();
        }

        [WebMethod]
        [System.Web.Script.Services.ScriptMethod(ResponseFormat = System.Web.Script.Services.ResponseFormat.Json)]
        public bool ValidateCaptcha(string challengeValue, string responseValue)
        {
            Recaptcha.RecaptchaValidator captchaValidtor = new Recaptcha.RecaptchaValidator
            {
                PrivateKey = Convert.ToString(ConfigurationManager.AppSettings["ReCaptchaPrivateKey"]), // Get Private key of the CAPTCHA from Web.config file.
                RemoteIP = System.Web.HttpContext.Current.Request.UserHostAddress,
                Challenge = challengeValue,
                Response = responseValue
            };

            Recaptcha.RecaptchaResponse recaptchaResponse = captchaValidtor.Validate(); // Send data about captcha validation to reCAPTCHA site.
            return recaptchaResponse.IsValid; // Get boolean value about Captcha success / failure.
        }


        public ActionResult RenderResetPassword()
        {
            return PartialView("ResetPasswordHandle", new ResetPasswordViewModel());
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult ProcessResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            //Get member from email
            IMember resetMember = Services.MemberService.GetByEmail(model.EmailAddress);
            
            //Ensure we have that member
            if (resetMember != null)
            {
                //Get the querystring GUID
                var refreshQuery = Request.QueryString["resetGUID"];

                //Querystring value check
                if (!string.IsNullOrEmpty(refreshQuery))
                {
                    //See if the query string matches the value on the member property
                    if (resetMember.Properties["resetGUID"].Value.ToString() == refreshQuery)
                    {

                        //Got a match, now check to see if the 15min window hasnt expired
                        DateTime expiryTime = DateTime.ParseExact(refreshQuery, "ddMMyyyyHHmmssFFFF", null);

                        //Check the current time is less than the expiry time
                        DateTime currentTime = DateTime.Now;

                        //Check if date has NOT expired
                        if (currentTime.CompareTo(expiryTime) < 0)
                        {

                            //Got a match, we can allow user to update password
                            //resetMember.RawPasswordValue = model.Password;
                            Services.MemberService.SavePassword(resetMember, model.Password);
                            //Remove the resetGUID value
                            resetMember.SetValue("resetGUID", string.Empty);

                            //Save the member
                            Services.MemberService.Save(resetMember);
                            //resetMember.Save();

                            TempData["ResetSuccess"] = true;
                            return RedirectToCurrentUmbracoPage();
                        }
                        else
                        {
                            //ERROR: Reset GUID has expired
                            ModelState.AddModelError("ResetPasswordForm.", "Password reset has timed out. <a href='/forgotten-password' data-reveal-id='forgotPassModal' data-reveal-ajax='true'>Reset your password again</a>.");
                            return CurrentUmbracoPage();
                        }
                    }
                    else
                    {
                        //ERROR: query string does not match what is stored on member property
                        //Invalid GUID
                        ModelState.AddModelError("ResetPasswordForm.", "Password reset has timed out. <a href='/forgotten-password' data-reveal-id='forgotPassModal' data-reveal-ajax='true'>Reset your password again</a>.");
                        return CurrentUmbracoPage();
                    }
                }
                else
                {
                    //ERROR: No query string present
                    //Invalid GUID
                    ModelState.AddModelError("ResetPasswordForm.", "Password reset has timed out. <a href='/forgotten-password' data-reveal-id='forgotPassModal' data-reveal-ajax='true'>Reset your password again</a>.");
                    return CurrentUmbracoPage();
                }
            }

            return RedirectToCurrentUmbracoPage();
        }

     
        public ActionResult RenderEmailVerification(string verifyGUID)
        {
            //Homepage node
            var home = CurrentPage.AncestorOrSelf("Home");
            var membersWithGUID = Services.MemberService.GetMembersByPropertyValue("emailVerifyGUID", verifyGUID);
            //Member memberByGuid = Member.GetAllAsList().SingleOrDefault(x => x.getProperty("emailVerifyGUID").Value.ToString() == verifyGUID);
            //var db = new uMembership();
            //var membersWithGUID = db.SearchMembersByPropertyValue("emailVerifyGUID", verifyGUID).SingleOrDefault(p => p.MemberProperties["emailVerifyGUID"] == verifyGUID);

            //Ensure we find a member with the verifyGUID
            if (membersWithGUID.Count() > 0)
            {
                foreach (var memberDeet in membersWithGUID)
                {
                    IMember member = Services.MemberService.GetByEmail(memberDeet.Email);
                    member.SetValue("hasVerifiedEmail", true);
                    bool currentValidAccount = Convert.ToBoolean(member.Properties["validatedAccountNumber"].Value);

                    if (!currentValidAccount)
                    {
                        TempData["ValidAccount"] = false;
                    }
                    else
                    {
                        TempData["ValidAccount"] = true;
                    }
                    Services.MemberService.Save(member);
                }
                //IMember member = Services.MemberService.GetByEmail(membersWithGUID.MemberEmail);
                //We got the member, so let's update the verify email checkbox
                

                
                //member.XmlGenerate(new System.Xml.XmlDocument());
                //Save the member
                //member.Save();
                

            }
            else
            {
                //Update success flag (in a TempData key)
                TempData["Success"] = false;

                //Couldn't find them - most likely invalid GUID
                return PartialView("VerifyUserHandle");
            }

            //Update success flag (in a TempData key)
            TempData["Success"] = true;

            //All sorted let's redirect to root/homepage
            return PartialView("VerifyUserHandle");
        }


        public ActionResult RenderAccountAccess(string currentAccount)
        {
            //Member should be logged on for this form
            //if (Member.IsLoggedOn())
            //{
                AccountAccessViewModel model = new AccountAccessViewModel();
                var profileModel = Members.GetCurrentMemberProfileModel();
                var currentMember = Services.MemberService.GetByEmail(profileModel.Email);
                bool isPartner = Roles.IsUserInRole(currentMember.Email, "Partner") || Roles.IsUserInRole(currentMember.Email, "PartnerAdmin") || Roles.IsUserInRole(currentMember.Email, "SuperPartnerAdmin") ? true : false;    
           
                if (!isPartner)
                {
                    var associatedAccounts = _memberRepository.GetAssociatedAccount(currentMember.Id);
                    List<SelectListItem> items = new List<SelectListItem>();
                    bool selected = false;

                    foreach (var account in associatedAccounts)
                    {
                        selected = false;
                        if (account.AccountNumber == currentAccount)
                        {
                            selected = true;
                        }
                        items.Add(new SelectListItem() { Text = account.AccountNumber, Value = account.AccountNumber, Selected = selected });
                    }

                    model.AssociatedAccounts = items;
                }
                else
                {
                    model.isPartner = true;
                    model.SelectedAccount = currentAccount;
                }
                return PartialView("AccountAccessHandle", model);
            //}
        }

        [HttpPost]
        public ActionResult ProcessAccountAccess(AccountAccessViewModel model)
        {
            Session["CurrentAccount"] = model.SelectedAccount;
            Session["OrderInBag"] = null;

            return RedirectToCurrentUmbracoPage();
        }

        public ActionResult CheckEmailIsUsed(string emailAddress)
        {
            //Try and get member by email typed in
            var checkEmail = Services.MemberService.GetByEmail(emailAddress);

            if (checkEmail != null)
            {
                return Json(String.Format("The email address '{0}' is already in use. Please register a different email or contact your Customer Service Representative for assistance.", emailAddress), JsonRequestBehavior.AllowGet);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

       
        public ActionResult RenderMemberSearch()
        {
            return PartialView("MemberSearchHandle");
        }

        [HttpPost]
        public ActionResult SearchMembers(string Query)
        {
            var test = Query;
            var sb = new StringBuilder();
            //var members = db.SearchMembersByPropertyValue("AccountNumber", HttpRequest

            return RedirectToCurrentUmbracoPage();
        }

        /// <summary>
        /// method to get Client ip address
        /// </summary>
        /// <param name="GetLan"> set to true if want to get local(LAN) Connected ip address</param>
        /// <returns></returns>
        public static string GetVisitorIPAddress(bool GetLan = false)
        {
            string visitorIPAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (String.IsNullOrEmpty(visitorIPAddress))
                visitorIPAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            if (string.IsNullOrEmpty(visitorIPAddress))
                visitorIPAddress = System.Web.HttpContext.Current.Request.UserHostAddress;

            if (string.IsNullOrEmpty(visitorIPAddress) || visitorIPAddress.Trim() == "::1")
            {
                GetLan = true;
                visitorIPAddress = string.Empty;
            }

            if (GetLan && string.IsNullOrEmpty(visitorIPAddress))
            {
                //This is for Local(LAN) Connected ID Address
                string stringHostName = Dns.GetHostName();
                //Get Ip Host Entry
                IPHostEntry ipHostEntries = Dns.GetHostEntry(stringHostName);
                //Get Ip Address From The Ip Host Entry Address List
                IPAddress[] arrIpAddress = ipHostEntries.AddressList;

                try
                {
                    visitorIPAddress = arrIpAddress[arrIpAddress.Length - 2].ToString();
                }
                catch
                {
                    try
                    {
                        visitorIPAddress = arrIpAddress[0].ToString();
                    }
                    catch
                    {
                        try
                        {
                            arrIpAddress = Dns.GetHostAddresses(stringHostName);
                            visitorIPAddress = arrIpAddress[0].ToString();
                        }
                        catch
                        {
                            visitorIPAddress = "127.0.0.1";
                        }
                    }
                }

            }


            return visitorIPAddress;
        }

        public ActionResult RenderAccountDetails()
        {
            return PartialView("_AccountDetails");
        }
    }
}