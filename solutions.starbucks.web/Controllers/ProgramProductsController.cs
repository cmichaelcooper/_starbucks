using solutions.starbucks.Interfaces;
using solutions.starbucks.Model;
using solutions.starbucks.Model.Pocos;
using solutions.starbucks.Repository.Umbraco;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace solutions.starbucks.web.Controllers
{
    public class ProgramProductsController : ProgramController
    {
        private readonly IProductsRepository _productsRepository;
        private readonly IOrdersRepository _orderRepository;
        private readonly IMemberAttributesRepository _memberRepository;
        public ProgramProductsController(IProductsRepository productsRepository, IOrdersRepository orderRepository, IMemberAttributesRepository memberRepository)
            : base(_programRepository)
        {
            _programRepository = new ProgramRepository();
            _productsRepository = productsRepository;
            _orderRepository = orderRepository;
            _memberRepository = memberRepository;
        }

        //
        // GET: /Program/

        [Authorize]
        public ActionResult ProgramProducts()
        {
            Orders orderInBag = _orderRepository.GetOrderInBag();

            ProgramProductDetailsModel model = new ProgramProductDetailsModel();

            //Proceed with screwed up logic to find images
            string brandValue = _programRepository.GetProgramBrand(CurrentPage.Parent);
            string brandUrl = System.Configuration.ConfigurationManager.AppSettings["SBUXBrandUrl"];
            model.ProductBrandUrl = brandUrl + "/images/product/";
            if (brandValue.ToUpper() == "SBC")
            {
                brandUrl = System.Configuration.ConfigurationManager.AppSettings["SBCBrandUrl"];
                model.ProductBrandUrl = brandUrl + "/images/products/";
            }
            model.Skus =  new List<SKU>();

            var IngredientListing = _productsRepository.MapIngredientsToProducts(CurrentPage).Where(p => p.IngredientSKU != null).Select(p => p.IngredientSKU).Distinct();
            foreach (var ingredient in IngredientListing)
            {
                SKU SkuValue = _productsRepository.GetSKUDetails(ingredient, brandValue.ToUpper());
                if (SkuValue != null)
                {
                    model.Skus.Add(SkuValue);
                }
            }

            var additionalIngListing = _productsRepository.MapAdditionalIngredientsToProducts(CurrentPage).Where(p => p.IngredientSKU != null).Select(p => p.IngredientSKU).Distinct();
            foreach (var additionalIngredient in additionalIngListing)
            {

                SKU SkuValue = _productsRepository.GetSKUDetails(additionalIngredient, brandValue.ToUpper());
                if (SkuValue != null)
                {
                    model.Skus.Add(SkuValue);
                }
            }

            model.Products = new List<ProductValues>();
            foreach (var skuVal in model.Skus)
            {
                ProductValues ProductsList = _productsRepository.GetProductsFromSKU(skuVal.ProductID, brandValue.ToUpper());
                model.Products.Add(ProductsList);
            }

            model.ProductImages = new List<ProductImage>();
            
            return View(model);

        }
        public override bool Equals(object obj)
        {
            SBUXProductsForOutput q = obj as SBUXProductsForOutput;
            return q != null  ;
        }

    }
}