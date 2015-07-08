using Examine;
using solutions.starbucks.web.Controllers.Masters;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace solutions.starbucks.web.Controllers
{
    public class RecipeDetailsController : GenericMasterController
    {
        public RecipeDetailsController() { }

        [Authorize]
        public ActionResult RecipeDetails(string recipeId)
        {
            var criteria = ExamineManager.Instance.DefaultSearchProvider
                .CreateSearchCriteria("content");
            var filter = criteria.NodeTypeAlias("RecipeDetails");
            var result = Umbraco.TypedSearch(filter.Compile()).ToArray();
            if (!result.Any())
            {
                throw new HttpException(404, "No recipe");
            }
            return View("RecipeDetails", CreateRenderModel(result.First()));

        }
    }
}