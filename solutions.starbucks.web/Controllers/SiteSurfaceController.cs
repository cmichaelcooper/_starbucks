using solutions.starbucks.Interfaces;
using System.Linq;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace solutions.starbucks.web.Controllers
{
    public class SiteSurfaceController : SurfaceController
    {
        private readonly IOrdersRepository _ordersRepository;

        public SiteSurfaceController(IOrdersRepository ordersRepository)
        {
            _ordersRepository = ordersRepository;
        }

        public ActionResult RenderNavigation()
        {
            ViewData["ItemsExistInBag"] = _ordersRepository.TotalItemsInCart() > 0;

            var home = Umbraco.TypedContentAtRoot().SingleOrDefault(x => x.Name == Umbraco.GetDictionaryValue("Home"));
            var pages = home.Children;

            //Return our collection of pages to the view
            return PartialView("_MainNavigation", pages);
        }
    }
}