using solutions.starbucks.Model.Masters;
using solutions.starbucks.Model.Pocos;
using System.Collections.Generic;

namespace solutions.starbucks.Model
{
    public class RecipeCatalogModel : GenericMasterModel
    {
        public string Category { get; set; }

        public string CategoryName { get; set; } 
    }

    public class RecipeListing
    {
        public string RecipeID { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string PreparationSteps { get; set; }

        public string Options { get; set; }

        public string DrinkID { get; set; }

        public string Keywords { get; set; }

        public bool MarketingAvailable { get; set; }

        public bool Suppress { get; set; }

        public string SiteID { get; set; }

        public string[] TypeID { get; set; }

        public IEnumerable<RecipeTraits> RecipeTraits { get; set; }

        public IEnumerable<RecipeIngredientsBySize> RecipeIngredients { get; set; }
    }
}