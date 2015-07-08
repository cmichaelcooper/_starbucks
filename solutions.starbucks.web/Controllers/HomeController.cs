using solutions.starbucks.Interfaces;
using solutions.starbucks.Model;
using solutions.starbucks.Repository.PetaPoco;
using solutions.starbucks.web.Controllers.Masters;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using umbraco.cms.businesslogic.member;
using Umbraco.Web;

namespace solutions.starbucks.web.Controllers
{
    public class HomeController : MainMasterController
    {
        private readonly IMemberAttributesRepository _memberRepository;

        public HomeController(IMemberAttributesRepository memberRepository):base(_ipAddressBlockRepository)
        {
            _memberRepository = memberRepository;
            _ipAddressBlockRepository = new IPAddressBlockRepository();
        }

        [HttpGet]
        public ActionResult Home(string ReturnUrl)
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {

                HomeModel model = new HomeModel();
                
                model.BodyText = CurrentPage.GetPropertyValue<IHtmlString>("bodyText");
                model.LeftPath = CurrentPage.GetPropertyValue<IHtmlString>("leftPath");
                model.RightPath = CurrentPage.GetPropertyValue<IHtmlString>("rightPath");
                model.MiddlePath = CurrentPage.GetPropertyValue<IHtmlString>("middlePath");
                model.BodyClass = CurrentPage.GetTemplateAlias().ToLower();

                //model.BodyId = CurrentPage.UrlName.ToLower();
                model.BodyId = "home";
                if (!String.IsNullOrEmpty(ReturnUrl))
                {
                    TempData["LoginRequired"] = true;

                    if (ReturnUrl.Contains("SBUXFS"))
                    {
                        Session["ReturnToSiteSection"] = ReturnUrl.Replace("SBUXFS", "");

                        return Redirect("/registration-required");
                    }
                    else if (ReturnUrl.Contains("SBCFS"))
                    {
                        Session["ReturnToSiteSection"] = ReturnUrl.Replace("SBCFS", "");

                        return Redirect("/registration-required");
                    }
                }

                return View(model);
            }

            Member currentMember = Member.GetCurrentMember();
            if (currentMember != null)
            {
                bool isPartner = Roles.IsUserInRole(currentMember.Email, "Partner") || Roles.IsUserInRole(currentMember.Email, "PartnerAdmin") || Roles.IsUserInRole(currentMember.Email, "SuperPartnerAdmin") ? true : false;
                bool invalidCustomer = Roles.IsUserInRole(currentMember.Email, "Inactive");
                if (invalidCustomer)
                {
                    return new RedirectResult("/umbraco/Surface/AccountSurface/Logout");
                }
            }
            else
            {
                return new RedirectResult("/umbraco/Surface/AccountSurface/Logout");
            }

            if (ReturnUrl != null)
            {
                return new RedirectResult(ReturnUrl);
            }
            return new RedirectResult("/dashboard");
        }
    }
}