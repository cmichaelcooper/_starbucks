using solutions.starbucks.Model;
using solutions.starbucks.Model.Pocos;
using System.Collections.Generic;

namespace solutions.starbucks.Interfaces
{
    public interface IRecipesRepository
    {
        IEnumerable<RecipeListing> GetCombinedRecipes();
        IEnumerable<RecipeTraits> GetRecipeTraits(string recipeId, string siteId);
        IEnumerable<RecipeIngredientsBySize> GetRecipeIngredients(string recipeId, string siteId);
        IEnumerable<RecipeListing> GetRecipesWithTrait(string traitValue);
        IEnumerable<RecipeListing> GetRecipesWithIngredient(string productId);
    }
}