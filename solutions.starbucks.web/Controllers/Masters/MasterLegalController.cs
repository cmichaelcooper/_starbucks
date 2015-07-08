using solutions.starbucks.Model.Masters;
using solutions.starbucks.Repository.PetaPoco;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web;

namespace solutions.starbucks.web.Controllers.Masters
{
    public class MasterLegalController : MainMasterController
    {
        public MasterLegalController() : base(_ipAddressBlockRepository)
        {
            _ipAddressBlockRepository = new IPAddressBlockRepository();
        }
        protected ViewResult View(MasterLegalModel model)
        {
            return this.View(null, model);
        }

        protected ViewResult View(string view, MasterLegalModel model)
        {
            model.BodyText = CurrentPage.GetPropertyValue<IHtmlString>("bodyText");
            model.Intro = CurrentPage.GetPropertyValue<IHtmlString>("intro");
            model.Topics = CurrentPage.GetPropertyValue<IHtmlString>("topics");
            model.BodyClass = CurrentPage.GetTemplateAlias().ToLower();
            model.BodyId = CurrentPage.UrlName.ToLower();
            return base.View(view, model);
        }
    }
}