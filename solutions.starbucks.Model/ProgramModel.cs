using solutions.starbucks.Model.Masters;
using System.Collections.Generic;
using System.Web;


namespace solutions.starbucks.Model
{
    public class ProgramModel : GenericMasterModel
    {
        public IEnumerable<ProgramTile> TileContents { get; set; }
        public IEnumerable<SeasonalRecipe> RecipeTiles { get; set; }
        public IEnumerable<SeasonalMarketing> MarketingTiles { get; set; }
        public IEnumerable<SeasonalIngredient> IngredientTiles { get; set; }
        public string ProgramBackground { get; set; }
        public string ProgramBrandSelect { get; set; }
        public string ProgramProductsLink { get; set; }
        public string RecipeCardUpload { get; set; }
        public bool ArchiveProgram { get; set; }
        public string CurrentProgramURL { get; set; }

    }

    public class ProgramTile : ProgramModel
    {
        public bool IsThisARecipeTile { get; set; }
        public string BrandSelect { get; set; }
        public string RecipeGroupTitle { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public IHtmlString FeaturedBox { get; set; }
        public IHtmlString RecipeGroupHelp { get; set; }
        public IEnumerable<SeasonalRecipe> RecipeContents { get; set; }
        public IEnumerable<SeasonalMarketing> MarketingContents { get; set; }
        public string DesktopTileBanner { get; set; }
        public string BigMobileTileBanner { get; set; }
        public string SmallMobileTileBanner { get; set; }
    }

    public class SeasonalRecipe : ProgramTile
    {
        public string RecipeID { get; set; }

        public string RecipeName { get; set; }

        public string ParentValue { get; set; }

        public string RecipeImage { get; set; }
        
        public IEnumerable<SeasonalIngredient> Ingredients { get; set; }
    }

    public class ProgramMarketingItem 
    {
        public string SKU { get; set; }

        public string Name { get; set; }

        public string MaxQuantity { get; set; }

    }

    public class SeasonalMarketing : ProgramTile
    {
        public string MarketingID { get; set; }

        public string MarketingDescription { get; set; }

        public string MarketingItemsUnparsed { get; set; }

        public List<ProgramMarketingItem> ProgramMarketingItems { get; set; }

        public string ParentValue { get; set; }

        public string MarketingImage { get; set; }
        
    }

    public class SeasonalIngredient : SeasonalRecipe
    {
        public string IngredientName { get; set; }

        public string BeverageYield { get; set; }

        public string IngredientSKU { get; set; }

        public string ProductPageSortOrder { get; set; }
    }
}