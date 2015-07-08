using Examine;
using solutions.starbucks.web.Controllers.Masters;
using System.Linq;
using System.Web.Mvc;

namespace solutions.starbucks.web.Controllers
{

    public class RecipeCatalogController : GenericMasterController
    {

        public RecipeCatalogController() { }

        [Authorize]
        public ActionResult RecipeCatalog(string category)
        {
            //System.Diagnostics.Debugger.Launch();
            //ProgramModel model = new ProgramModel();
            var categoryName = "";

            switch (category.ToLower())
            {
                case "hot_beverages":
                    categoryName = "Hot Beverages";
                    break;
                case "cold_beverages":
                    categoryName = "Cold Beverages";
                    break;
                case "frozen_blended_smoothies":
                    categoryName = "Frozen/Blended Smoothies";
                    break;
                default:
                    categoryName = "";
                    break;
            }

            var criteria = ExamineManager.Instance.DefaultSearchProvider.CreateSearchCriteria("content");
            var filter = criteria.NodeTypeAlias("RecipeCatalog");
            var result = Umbraco.TypedSearch(filter.Compile()).ToArray();
            TempData["Category"] = category;
            TempData["CategoryName"] = categoryName;
            if (!result.Any())
            {
                throw new System.Web.HttpException(404, "No recipe");
            }

            return View("RecipeCatalog", CreateRenderModel(result.First()));


        }


    }
}
