using Umbraco.Core.Persistence;

namespace solutions.starbucks.Model.Pocos
{
    [TableName("vw_Recipes")]
    [PrimaryKey("RecipeID")]
    [ExplicitColumns]
    public class Recipes
    {
        [Column("RecipeID")]
        public string RecipeID { get; set; }

        [Column("Title")]
        public string Title { get; set; }

        [Column("Description")]
        public string Description { get; set; }

        //[Column("Image")]
        //public string Image { get; set; }

        //[Column("Thumbnail")]
        //public string Thumbnail { get; set; }

        [Column("PreparationSteps")]
        public string PreparationSteps { get; set; }

        [Column("Options")]
        public string Options { get; set; }

        [Column("DrinkID")]
        public string DrinkID { get; set; }

        [Column("Keywords")]
        public string Keywords { get; set; }

        [Column("MarketingAvailable")]
        public bool MarketingAvailable { get; set; }

        [Column("Suppress")]
        public bool Suppress { get; set; }

        [Column("SiteID")]
        public string SiteID { get; set; }

        [Column("Image")]
        public string Image { get; set; }

        [Column("CardIllustration")]
        public string CardIllustration { get; set; }

        [ResultColumn]
        public string TypeID { get; set; }
    }

    [TableName("vw_RecipeTraits")]
    [PrimaryKey("Recipe")]
    [ExplicitColumns]
    public class RecipeTraits
    {
        [Column("TraitID")]
        public string TraitID { get; set; }

        [Column("Value")]
        public string Value { get; set; }
    }

    [TableName("vw_RecipeIngredientsBySize")]
    [PrimaryKey("Recipe")]
    [ExplicitColumns]
    public class RecipeIngredientsBySize
    {

        [Column("IngredientName")]
        public string IngredientName { get; set; }

        [Column("SizeName")]
        public string SizeName { get; set; }

        [Column("Measure")]
        public string Measure { get; set; }

        [Column("MeasureALT")]
        public string MeasureALT { get; set; }

        [Column("SortOrder")]
        public int SortOrder { get; set; }

        [Column("ProductID")]
        public string ProductID { get; set; }

        [Column("Title")]
        public string Title { get; set; }
    }
}