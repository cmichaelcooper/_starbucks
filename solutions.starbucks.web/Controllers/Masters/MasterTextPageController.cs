using solutions.starbucks.Interfaces;
using solutions.starbucks.Model.Masters;
using solutions.starbucks.Repository.PetaPoco;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web;

namespace solutions.starbucks.web.Controllers.Masters
{
    public class MasterTextPageController : MainMasterController
    {
        protected static IDictionaryItemRepository _dictionaryRepository;

        public MasterTextPageController(IDictionaryItemRepository dictionaryRepository):base(_ipAddressBlockRepository)
        {
            _dictionaryRepository = dictionaryRepository;
            _ipAddressBlockRepository = new IPAddressBlockRepository();
        }

        protected ViewResult View(MasterTextPageModel model)
        {
            return this.View(null, model);
        }

        protected ViewResult View(string view, MasterTextPageModel model)
        {
            model.IntroText = CurrentPage.GetPropertyValue<IHtmlString>("introText");
            model.BodyText = CurrentPage.GetPropertyValue<IHtmlString>("bodyText");
            model.SupportContent = CurrentPage.GetPropertyValue<IHtmlString>("supportContent");
            model.BodyClass = CurrentPage.GetTemplateAlias().ToLower();
            //model.BodyId = CurrentPage.UrlName.ToLower();
            model.BodyId = _dictionaryRepository.GetTermInEnglish(CurrentPage.UrlName);
            return base.View(view, model);
        }
    }
}