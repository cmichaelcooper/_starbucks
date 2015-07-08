using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using solutions.starbucks.Interfaces;
using solutions.starbucks.web.Classes;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Umbraco.Web.WebApi;

namespace solutions.starbucks.web.Controllers
{
    public class ProductApiController : UmbracoApiController
    {
        private readonly IProductsRepository _productsRepository;
        private readonly IRecipesRepository _recipesRepository;
        private static object lockSerializer = new Object();

        public ProductApiController(IProductsRepository productsRepository,IRecipesRepository recipesRepository)
        {
            _productsRepository = productsRepository;
            _recipesRepository = recipesRepository;
        }
        // GET api/productapi
        //public IEnumerable<ProductValues> GetAllProducts()
        //{
        //    var allProducts = _productsRepository.GetAllProducts();
        //    return allProducts;
        //}

        public HttpResponseMessage GetAllProducts()
        {
            string directory = HttpContext.Current.Server.MapPath("/data_cache");
            string file = directory + "/all_products.js";

            if(!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
            lock (lockSerializer)
            {
                if (!System.IO.File.Exists(file) || (System.IO.File.GetLastWriteTime(file) < DateTime.Now.AddHours(-2)))
                {
                    //var allProducts = _productsRepository.GetAllProducts().Where(ap => ap.SkuInformation != null);
                    var allProducts = _productsRepository.GetCombinedProducts();

                    //var productUnion = combinedProducts.Union(allProducts).GroupBy(cb => cb.ProductID);
                    JsonNetResult jsonNetResult = new JsonNetResult();
                    jsonNetResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    jsonNetResult.Formatting = Formatting.Indented;
                    jsonNetResult.Data = new
                    {
                        allProducts
                    };


                    string jsonData = JsonConvert.SerializeObject(new { allProducts });

                    System.IO.File.WriteAllText(file, jsonData);
                }
            }

            JToken json = JObject.Parse(System.IO.File.ReadAllText(file));
            return new HttpResponseMessage()
            {
                Content = new JsonContent(json)
            };

        }

        //public JsonNetResult GetProductTypes()
        //{
        //    var allProductTypes = _productsRepository.GetAllProductFilters();
        //    JsonNetResult jsonNetResult = new JsonNetResult();
        //    jsonNetResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        //    jsonNetResult.Formatting = Formatting.Indented;
        //    jsonNetResult.Data = new
        //    {
        //        allProductTypes
        //    };

        //    return jsonNetResult;
        //}

        public JsonNetResult GetProductWithId(string productId)
        {
            //var allProducts = _productsRepository.GetAllProducts().Where(ap => ap.SkuInformation != null);
            var allProducts = _productsRepository.GetProductFromCombinedProducts(productId).Where(p => p.ProductID == productId);
            var productTraits = allProducts.Select(p => p.ProductTraitValues.Where(t => t.Trait == "Type")).FirstOrDefault();

            var type = productTraits.Select(p => p.TypeID).FirstOrDefault();

            var relatedProducts = _productsRepository.GetProductsWithType(type);
            var relatedRecipes = _recipesRepository.GetRecipesWithIngredient(productId);
            //foreach (var prod in allProducts)
            //{
            //    foreach (var trait in prod.ProductTraitValues)
            //    {
            //        trait.Value;
            //        trait.TypeID;
            //        c++;
            //    }
            //}
            //var relatedProducts = _productsRepository.GetCombinedProducts().Where(p => p.ProductTraitValues.Valu
            //var productUnion = combinedProducts.Union(allProducts).GroupBy(cb => cb.ProductID);

            JsonNetResult jsonNetResult = new JsonNetResult();
            jsonNetResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonNetResult.Formatting = Formatting.Indented;
            jsonNetResult.Data = new
            {
                allProducts,
                relatedProducts,
                relatedRecipes
            };

            return jsonNetResult;
        }


        // GET api/productapi/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/productapi
        public void Post([FromBody]string value)
        {
        }

        // PUT api/productapi/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/productapi/5
        public void Delete(int id)
        {
        }
    }

    public class JsonContent : HttpContent
    {
        private readonly JToken _value;

        public JsonContent(JToken value)
        {
            _value = value;
            Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }

        protected override Task SerializeToStreamAsync(Stream stream,
            TransportContext context)
        {
            var jw = new JsonTextWriter(new StreamWriter(stream))
            {
                Formatting = Formatting.Indented
            };
            _value.WriteTo(jw);
            jw.Flush();
            return Task.FromResult<object>(null);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }
    }

}
