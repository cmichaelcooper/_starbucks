using Examine;
using solutions.starbucks.Interfaces;
using solutions.starbucks.Model;
using solutions.starbucks.web.Controllers.Masters;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace solutions.starbucks.web.Controllers
{

    public class CoffeeDetailsController : GenericMasterController
    {

        protected static IProductsRepository _productRepository;

        public CoffeeDetailsController(IProductsRepository productRepository)
        {
            _productRepository = productRepository;
        }

        //
        // GET: /Program/

        [Authorize]
        public ActionResult CoffeeDetails(string productId)
        {

            ProductDetailsModel model = new ProductDetailsModel();
           
            var criteria = ExamineManager.Instance.DefaultSearchProvider
       .CreateSearchCriteria("content");
            var filter = criteria.NodeTypeAlias("ProductDetails");
            var result = Umbraco.TypedSearch(filter.Compile()).ToArray();
            if (!result.Any())
            {
                throw new HttpException(404, "No product");
            }

            return View("CoffeeDetails", CreateRenderModel(result.First()));
        }




    }
}

