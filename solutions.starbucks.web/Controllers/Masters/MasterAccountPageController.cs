using solutions.starbucks.Model.Masters;
using solutions.starbucks.Repository.PetaPoco;
using System.Web.Mvc;
using Umbraco.Web;

namespace solutions.starbucks.web.Controllers.Masters
{
    public class MasterAccountPageController : MainMasterController
    {
        public MasterAccountPageController() : base(_ipAddressBlockRepository)
        {
            _ipAddressBlockRepository = new IPAddressBlockRepository();
        }
        protected ViewResult View(MasterAccountPageModel model)
        {
            return this.View(null, model);
        }

        protected ViewResult View(string view, MasterAccountPageModel model)
        {

            model.BodyClass = CurrentPage.GetTemplateAlias().ToLower();
            model.BodyId = CurrentPage.UrlName.ToLower();
            return base.View(view, model);
        }
    }

}
