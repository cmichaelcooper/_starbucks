using solutions.starbucks.Interfaces;
using solutions.starbucks.Model;
using solutions.starbucks.Model.Pocos;
using solutions.starbucks.Repository.Umbraco;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace solutions.starbucks.Repository.PetaPoco
{
    public class RecipesRepository : IRecipesRepository
    {
        protected UmbracoDatabase _db = ApplicationContext.Current.DatabaseContext.Database;
        private static UmbracoRepository _umbracoRepository = new UmbracoRepository();


        public IEnumerable<RecipeListing> GetCombinedRecipes()
        {
            string[] defaultType = new string[] { "" };
            //var queryText = Sql.Builder.Append("SELECT vw_Recipes.RecipeID, Title, Description, PreparationSteps, Options, DrinkID, MarketingAvailable, Suppress, TypeID, vw_Recipes.Image, vw_Recipes.CardIllustration, MAX(SiteID) AS SiteId FROM vw_Recipes LEFT JOIN vw_CombinedRecipeTypes ON vw_Recipes.RecipeID = vw_CombinedRecipeTypes.RecipeID GROUP BY vw_Recipes.RecipeID, Title, Description, PreparationSteps, Options, DrinkID, MarketingAvailable, Suppress, vw_Recipes.Image, vw_Recipes.CardIllustration, TypeID");
            var queryText = Sql.Builder.Append("SELECT [RecipeID],[Title],[Description],[PreparationSteps],[Options],[DrinkID],[MarketingAvailable],[Suppress],[TypeID],[SiteID] FROM vw_CombinedRecipes ORDER BY [Title]");
            var query = from r in _db.Query<Recipes>(queryText).ToList().Select(rl => new RecipeListing
            {
                RecipeID = rl.RecipeID,
                Title = rl.Title,
                Description = rl.Description,
                PreparationSteps = rl.PreparationSteps,
                Options = rl.Options,
                DrinkID = rl.DrinkID,
                Keywords = rl.Keywords,
                MarketingAvailable = rl.MarketingAvailable,
                Suppress = rl.Suppress,
                SiteID = rl.SiteID,
                TypeID = !string.IsNullOrEmpty(rl.TypeID) ? rl.TypeID.Split(',').Select(p => p.Trim()).ToArray() : defaultType,
                RecipeTraits = GetRecipeTraits(rl.RecipeID, rl.SiteID),
                RecipeIngredients = GetRecipeIngredients(rl.RecipeID, rl.SiteID)

            }) select r;
            return query;
        }

        public IEnumerable<RecipeTraits> GetRecipeTraits(string recipeId, string siteId)
        {
            var queryText = Sql.Builder.Append("SELECT TraitID, Value FROM vw_RecipeTraits WHERE Recipe=@0 AND Site=@1", recipeId, siteId);
            var query = from r in _db.Query<RecipeTraits>(queryText) select r;

            return query;
        }

        public IEnumerable<RecipeIngredientsBySize> GetRecipeIngredients(string recipeId, string siteId)
        {
            //var queryText = Sql.Builder.Append("SELECT IngredientName, SizeName, Measure, MeasureALT, SortOrder, ProductID, Title FROM vw_RecipeIngredientsBySize WHERE RecipeID=@0 AND SiteId=@1  ORDER BY SortOrder", recipeId, siteId);
            var queryText = Sql.Builder.Append("SELECT SiteId, IngredientName, SizeName, Measure, MeasureALT, SortOrder, ProductID, Title FROM vw_RecipeIngredientsBySize WHERE RecipeID  = @0 AND SiteId=@1 GROUP BY ProductID, IngredientName, SizeName, Measure, MeasureALT, Title, SiteId, SortOrder ORDER BY SortOrder", recipeId, siteId);
            var query = from r in _db.Query<RecipeIngredientsBySize>(queryText) select r;

            return query;
        }

        public IEnumerable<RecipeIngredientsBySize> GetRecipeProducts(string recipeId, string siteId)
        {
            //var queryText = Sql.Builder.Append("SELECT IngredientName, SizeName, Measure, MeasureALT, SortOrder, ProductID, Title FROM vw_RecipeIngredientsBySize WHERE RecipeID=@0 AND SiteId=@1  ORDER BY SortOrder", recipeId, siteId);
            var queryText = Sql.Builder.Append("SELECT SiteId, IngredientName, SizeName, Measure, MeasureALT, SortOrder, ProductID, Title FROM vw_RecipeIngredientsBySize WHERE RecipeID  = @0 AND SiteId=@1 GROUP BY ProductID, IngredientName, SizeName, Measure, MeasureALT, Title, SiteId, SortOrder ORDER BY SortOrder", recipeId, siteId);
            var query = from r in _db.Query<RecipeIngredientsBySize>(queryText) select r;

            return query;
        }

        public IEnumerable<RecipeListing> GetRecipesWithTrait(string traitValue)
        {
            var queryText = Sql.Builder.Append("SELECT DISTINCT TOP 4 newid(), vw_Recipes.RecipeID, Title, Description FROM vw_Recipes LEFT JOIN vw_CombinedRecipeTypes ON vw_Recipes.RecipeID = vw_CombinedRecipeTypes.RecipeID WHERE vw_CombinedRecipeTypes.TypeID LIKE @0 ORDER BY vw_Recipes.RecipeID", "%" + traitValue + "%");
            var query = from r in _db.Query<Recipes>(queryText).ToList().Select(rl => new RecipeListing
            {
                RecipeID = rl.RecipeID,
                Title = rl.Title,
                Description = rl.Description

            })
                        select r;

            return query;
        }

        public IEnumerable<RecipeListing> GetRecipesWithIngredient(string productId)
        {
            var queryText = Sql.Builder.Append("SELECT DISTINCT vw_RecipeIngredientsBySize.SiteId, vw_RecipeIngredientsBySize.RecipeID, vw_Recipes.Title AS Title FROM vw_RecipeIngredientsBySize INNER JOIN vw_Recipes ON vw_RecipeIngredientsBySize.RecipeID = vw_Recipes.RecipeID WHERE ProductID = @0 ORDER BY Title", productId);

            var query = from r in _db.Query<Recipes>(queryText).ToList().Select(rl => new RecipeListing
            {
                RecipeID = rl.RecipeID,
                Title = rl.Title,
                Description = rl.Description

            })
            select r;

            return query;
        }
        
    }
}