using solutions.starbucks.Model;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using TheAlchemediaProject.Services;
using Umbraco.Web.Mvc;

namespace solutions.starbucks.web.Controllers
{
    public class SignUpSurfaceController : SurfaceController
    {
        private TextPageModel _textPageModel;

        public SignUpSurfaceController(){
        }

        public class SignUpEmailModel
        {
            //public IHtmlString IntroText { get; set; }

            //public IHtmlString BodyText { get; set; }

            //public IHtmlString SupportContent { get; set; }

            //public string BecomeACustomerLinkFooter { get; set; }

            //public IHtmlString BecomeACustomerLinkMid { get; set; }
            public string SignUpFormName { get; set; }
            public string Name { get; set; }
            public string BusinessName { get; set; }
            [Required(ErrorMessage = "Enter your email address")]
            [EmailAddress(ErrorMessage = "Enter a valid email address")]
            public string EmailAddress { get; set; }

            public string PhoneNumber { get; set; }
            public string AccountNumber { get; set; }
            public string ChooseSegment { get; set; }
            public string NumberKits { get; set; }
        }

        public ActionResult RenderSignUpEmailForm(string formName) {
            SignUpEmailModel model = new SignUpEmailModel { SignUpFormName = formName };
            return PartialView("_SignUpEmail", model);
        }

        [HttpPost]
        public ActionResult RenderSignUpEmailForm(SignUpEmailModel model)
        {
            try
            {
                HttpCookie cookie = new HttpCookie(model.SignUpFormName, "1");
                Request.Cookies.Add(cookie);
                MailService email = new MailService();

                string emailTo = ConfigurationManager.AppSettings[model.SignUpFormName] ?? "ccooper@marlinco.com";
                string message = string.Format(
                    "<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'> <html xmlns='http://www.w3.org/1999/xhtml'> <head> <meta http-equiv='Content-Type' content='text/html; charset=UTF-8' /> <title>Customer request for information</title> </head> <body style='-webkit-text-size-adjust: none;'> <table align='center' width='600' border='0' cellspacing='0' cellpadding='0'> <tr> <td style='text-align:left;' colspan='2' bgcolor='#3E3935' height='90'> <p style='margin-left:18px; margin-top:18px; margin-bottom:18px; margin-right:18px'> <img src='http://marlinco.com/eblast/sbs/system_emails/branded_solutions_logo.png' width='223' height='54'/> </p> </td> </tr> <tr> <td style='text-align:left;' bgcolor='#FFFFFF' colspan='2'> <p style='text-align:left; font-family:Corbel, sans-serif; font-size:25px; font-weight:normal; color:#3E3935; margin-left:18px; margin-right:18px; margin-top:15px; margin-bottom:0px;'></p> </td> </tr> <tr> <td style='text-align:left;' width='250'> <p style='margin-top:0; margin-bottom:0;padding:0;margin-left:18px; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> CUSTOMER NAME: </p> </td> <td style='text-align:left;' width='350'> <p style='margin-top:0; margin-bottom:0;padding:0;margin-left:18px; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> {0} </p> </td> </tr><tr> <tr> <td style='text-align:left;' width='250'> <p style='margin-top:0; margin-bottom:0;padding:0;margin-left:18px; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> ACCOUNT NUMBER: </p> </td> <td style='text-align:left;' width='350'> <p style='margin-top:0; margin-bottom:0;padding:0;margin-left:18px; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> {1} </p> </td> </tr><td style='text-align:left;' width='250'> <p style='margin-top:0; margin-bottom:0; padding:0; margin-left:18px; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> CUSTOMER EMAIL: </p> </td> <td style='text-align:left;' width='350'> <p style='margin-top:0; margin-bottom:0;margin-left:18px;padding:0; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> {2} </p> </td> </tr> <tr> <td style='text-align:left;' valign='top' width='250'> <p style='margin-top:0; margin-bottom:0;padding:0;margin-left:18px; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> BUSINESS NAME: </p> </td> <td style='text-align:left;' width='350'> <p style='margin-top:0; margin-bottom:0;padding:0;margin-left:18px; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> {3} </p> </td> </tr> <tr> <td style='text-align:left;' valign='top' width='250'> <p style='margin-top:0; margin-bottom:0;padding:0;margin-left:18px; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> CUSTOMER PHONE: </p> </td> <td style='text-align:left;' width='350'> <p style='margin-top:0; margin-bottom:0px;padding:0;margin-left:18px; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> {4} </p> </td> </tr> <tr> <td style='text-align:left;' valign='top' width='250'> <p style='margin-top:0; margin-bottom:0;padding:0;margin-left:18px; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> BUSINESS SEGMENT: </p> </td> <td style='text-align:left;' width='350'> <p style='margin-top:0; margin-bottom:0px;padding:0;margin-left:18px; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> {5} </p> </td> </tr> <tr> <td style='text-align:left;' valign='top' width='250'> <p style='margin-top:0; margin-bottom:15px;margin-left:18px; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> NUMBER OF STARTER KITS: </p> </td> <td style='text-align:left;' width='350'> <p style='margin-top:0; margin-bottom:15px;margin-left:18px; text-align:left; font-family:Corbel, sans-serif; font-size:15px; font-weight:normal; color:#3E3935; line-height:23px;'> {6} </p> </td> </tr> </table> </body> </html>",
                        model.Name, model.AccountNumber, model.EmailAddress, model.BusinessName, model.PhoneNumber, model.ChooseSegment, model.NumberKits);

                email.SendEmail(emailTo, "sbxservices@marlinco.com",string.Empty, "no-reply@starbucksfs.com", "Spring Opt In Inquiry", message, true, null);
                return null;
            }
            catch
            {
                return PartialView();
            }
        }
    }
}
