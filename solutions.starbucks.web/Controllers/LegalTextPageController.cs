using solutions.starbucks.Interfaces;
using solutions.starbucks.Model;
using solutions.starbucks.web.Controllers.Masters;
using System.Web;
using System.Web.Mvc;

namespace solutions.starbucks.web.Controllers
{
    public class LegalTextPageController : MasterLegalController
    {
        private readonly ILegalTextPageRepository _legalTextPageRepository;

        public LegalTextPageController(ILegalTextPageRepository legalTextPageRepository)
        {
            _legalTextPageRepository = legalTextPageRepository;
        }

        [HttpGet]
        public ActionResult LegalTextPage(string ReturnUrl)
        {
            if (HttpContext.User.Identity.IsAuthenticated && Request.RawUrl.ToLower().Contains("registration-required"))
            {
                return Redirect("/dashboard");
            }
            LegalTextPageModel model = new LegalTextPageModel();
            model = _legalTextPageRepository.GetLegalPageProperties(CurrentPage);
            if (ReturnUrl != null)
            {
                Session["ReturnUrl"] = ReturnUrl;
            }
            return View(model);
        }
    }
}