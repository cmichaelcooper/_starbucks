using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using solutions.starbucks.Interfaces;
using solutions.starbucks.web.Classes;
using System;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.WebApi;

namespace solutions.starbucks.web.Controllers
{
    public class RecipeApiController : UmbracoApiController
    {
        private readonly IRecipesRepository _recipesRepository;
        private readonly IProductsRepository _productsRepository;
        private static object lockSerializer = new Object();

        public RecipeApiController(IRecipesRepository recipesRepository, IProductsRepository productsRepository)
        {
            _recipesRepository = recipesRepository;
            _productsRepository = productsRepository;
        }

        public HttpResponseMessage GetAllRecipes()
        {
            string directory = HttpContext.Current.Server.MapPath("/data_cache");
            string file = directory + "/all_recipes.js";
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
            lock (lockSerializer)
            {
                if (!System.IO.File.Exists(file) || (System.IO.File.GetLastWriteTime(file) < DateTime.Now.AddHours(-2)))
                {
                    //var allProducts = _recipesRepository.GetAllProducts().Where(ap => ap.SkuInformation != null);
                    var allRecipes = _recipesRepository.GetCombinedRecipes();

                    //var productUnion = combinedProducts.Union(allProducts).GroupBy(cb => cb.ProductID);
                    JsonNetResult jsonNetResult = new JsonNetResult();
                    jsonNetResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    jsonNetResult.Formatting = Formatting.Indented;
                    jsonNetResult.Data = new
                    {
                        allRecipes
                    };


                    string jsonData = JsonConvert.SerializeObject(new { allRecipes });

                    System.IO.File.WriteAllText(file, jsonData);
                }
            }

            JToken json = JObject.Parse(System.IO.File.ReadAllText(file));
            return new HttpResponseMessage()
            {
                Content = new JsonContent(json)
            };

        }

        public JsonNetResult GetRecipeWithId(string recipeId, string siteId)
        {
            var recipeWithId = _recipesRepository.GetCombinedRecipes().Where(p => p.RecipeID == recipeId && p.SiteID == siteId).FirstOrDefault();

            var relatedRecipes = _recipesRepository.GetRecipesWithTrait(recipeWithId.TypeID[0]);
            var relatedProducts = _productsRepository.GetCombinedProducts().Where(p => recipeWithId.RecipeIngredients.Select(s => s.ProductID).Contains(p.ProductID));

            JsonNetResult jsonNetResult = new JsonNetResult();
            jsonNetResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonNetResult.Formatting = Formatting.Indented;
            jsonNetResult.Data = new
            {
                recipeWithId,
                relatedRecipes,
                relatedProducts
            };

            return jsonNetResult;
        }

    }
}
