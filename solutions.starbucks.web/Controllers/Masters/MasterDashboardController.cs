using solutions.starbucks.Model.Masters;
using solutions.starbucks.Repository.PetaPoco;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web;

namespace solutions.starbucks.web.Controllers.Masters
{
    public class MasterDashboardController : MainMasterController
    {
        public MasterDashboardController()
            : base(_ipAddressBlockRepository)
        {
            _ipAddressBlockRepository = new IPAddressBlockRepository();
        }

        protected ViewResult View(MasterDashboardModel model)
        {
            return this.View(null, model);
        }

        protected ViewResult View(string view, MasterDashboardModel model)
        {
            model.Body = CurrentPage.GetPropertyValue<IHtmlString>("body");
            model.BodyClass = CurrentPage.GetTemplateAlias().ToLower();
            model.BodyId = CurrentPage.UrlName.ToLower(); // Marlin-5  Translate this value
            //Dictionary.DictionaryItem di = new Dictionary.DictionaryItem(CurrentPage.UrlName);
            //model.BodyId = di.Value(1);
            return base.View(view, model);
        }
    }
}