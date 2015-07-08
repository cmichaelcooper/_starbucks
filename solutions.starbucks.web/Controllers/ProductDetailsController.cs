using Examine;
using solutions.starbucks.Interfaces;
using solutions.starbucks.Model;
using solutions.starbucks.web.Controllers.Masters;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace solutions.starbucks.web.Controllers
{

    public class ProductDetailsController : GenericMasterController
    {

        protected static IProductsRepository _productRepository;

        public ProductDetailsController(IProductsRepository productRepository)
        {
            _productRepository = productRepository;
        }

        //
        // GET: /Program/

        [Authorize]
        public ActionResult ProductDetails(string productId)
        {

            ProductDetailsModel model = new ProductDetailsModel();

            ProductDetails product = _productRepository.GetProductFromCombinedProducts(productId).ToList().First();
            if (product.IsCoffee)
            {
                return Redirect("/products/coffee/details/" + productId);
            }

            var criteria = ExamineManager.Instance.DefaultSearchProvider
       .CreateSearchCriteria("content");
            var filter = criteria.NodeTypeAlias("ProductDetails");
            var result = Umbraco.TypedSearch(filter.Compile()).ToArray();
            if (!result.Any())
            {
                throw new HttpException(404, "No product");
            }

            return View("ProductDetails", CreateRenderModel(result.First()));
        }




    }
}

