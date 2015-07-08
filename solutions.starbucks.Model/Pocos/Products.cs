using Umbraco.Core.Persistence;

namespace solutions.starbucks.Model.Pocos
{
    [TableName("vw_ProductListing")]
    [PrimaryKey("ProductID")]
    [ExplicitColumns]
    public class ProductListing
    {
        [Column("ProductID")]
        public string ProductID { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("Description")]
        public string Description { get; set; }

        [Column("ShortDescription")]
        public string ShortDescription { get; set; }

        [Column("Title")]
        public string Title { get; set; }

        [Column("NutritionFacts")]
        public string NutritionFacts { get; set; }

        [Column("SortOrder")]
        public string SortOrder { get; set; }

        [Column("Suppress")]
        public string Suppress { get; set; }

        [Column("InventoryStatus")]
        public string InventoryStatus { get; set; }

        [Column("FileName")]
        public string FileName { get; set; }

        [ResultColumn]
        public string SiteId { get; set; }

    }
    [TableName("vw_SiteProductTraits")]
    [PrimaryKey("ID")]
    [ExplicitColumns]
    public class ProductTraits
    {
        [Column("ProductID")]
        public string ProductID { get; set; }

        [Column("ID")]
        public int TraitID { get; set; }

       
        [Column("Trait")]
        public string Trait { get; set; }

        [Column("Value")]
        public string Value { get; set; }

        [Column("TypeID")]
        public string TypeID { get; set; }

       

    }
    [TableName("vw_Products_SBUX")]
    [PrimaryKey("ProductID")]
    [ExplicitColumns]
    public class SBUXProductsForOutput
    {
        [Column("ProductID")]
        public string ProductID { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("Description")]
        public string Description { get; set; }

        [Column("ShortDescription")]
        public string ShortDescription { get; set; }

        [Column("Title")]
        public string Title { get; set; }

        [Column("NutritionFacts")]
        public string NutritionFacts { get; set; }

        [Column("SortOrder")]
        public string SortOrder { get; set; }

        [Column("Suppress")]
        public string Suppress { get; set; }

        [Column("InventoryStatus")]
        public string InventoryStatus { get; set; }

        [Column("FileName")]
        public string FileName { get; set; }

        [ResultColumn]
        public string SiteId { get; set; }

    }

    [TableName("vw_Products_SBC")]
    [PrimaryKey("ProductID")]
    [ExplicitColumns]
    public class SBCProductsDeets
    {
        [Column("ProductID")]
        public string ProductID { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("Description")]
        public string Description { get; set; }

        [Column("ShortDescription")]
        public string ShortDescription { get; set; }

        [Column("Title")]
        public string Title { get; set; }

        [Column("NutritionFacts")]
        public string NutritionFacts { get; set; }

        [Column("SortOrder")]
        public string SortOrder { get; set; }

        [Column("Suppress")]
        public string Suppress { get; set; }

        [Column("InventoryStatus")]
        public string InventoryStatus { get; set; }

        [Column("FileName")]
        public string FileName { get; set; }
        
        [ResultColumn]
        public string SiteId { get; set; }


    }

    [TableName("vw_ProductImages_SBC")]
    [PrimaryKey("ProductID")]
    [ExplicitColumns]
    public class SBCProductImage
    {
        [Column("ProductID")]
        public string ProductID { get; set; }

        [Column("Type")]
        public string Type { get; set; }

        [Column("FileName")]
        public string FileName { get; set; }
    }

    [TableName("vw_ProductImages_SBUX")]
    [PrimaryKey("ProductID")]
    [ExplicitColumns]
    public class SBUXProductImage
    {
        [Column("ProductID")]
        public string ProductID { get; set; }

        [Column("Type")]
        public string Type { get; set; }

        [Column("FileName")]
        public string FileName { get; set; }
    }

    [TableName("vw_SKUS_SBUX")]
    [PrimaryKey("SKUNumber")]
    [ExplicitColumns]
    public class SBUXSku
    {
        [Column("SKUNumber")]
        public string SKUNumber { get; set; }

        [Column("ProductID")]
        public string ProductID { get; set; }

        [Column("UOM")]
        public string UOM { get; set; }

        [Column("Unit")]
        public string Unit { get; set; }

        [Column("ShortDescription")]
        public string ShortDescription { get; set; }

        [Column("Suppress")]
        public bool Suppress { get; set; }

        [Column("InventoryStatus")]
        public string InventoryStatus { get; set; }

        [Column("SortOrder")]
        public int SortOrder { get; set; }

        [Column("InventoryStatusMessage")]
        public string InventoryStatusMessage { get; set; }

        [Column("Trait")]
        public string Trait { get; set; }

        [Column("Value")]
        public string Value { get; set; }
    }

    [TableName("vw_SKUS_SBC")]
    [PrimaryKey("SKUNumber")]
    [ExplicitColumns]
    public class SBCSku
    {
        [Column("SKUNumber")]
        public string SKUNumber { get; set; }

        [Column("ProductID")]
        public string ProductID { get; set; }

        [Column("UOM")]
        public string UOM { get; set; }

        [Column("Unit")]
        public string Unit { get; set; }

        [Column("ShortDescription")]
        public string ShortDescription { get; set; }

        [Column("Suppress")]
        public bool Suppress { get; set; }

        [Column("InventoryStatus")]
        public string InventoryStatus { get; set; }

        [Column("SortOrder")]
        public int SortOrder { get; set; }

        [Column("InventoryStatusMessage")]
        public string InventoryStatusMessage { get; set; }

        [Column("Trait")]
        public string Trait { get; set; }

        [Column("Value")]
        public string Value { get; set; }
    }

    [TableName("vw_SKUS")]
    [PrimaryKey("SKUNumber")]
    [ExplicitColumns]
    public class SKUS
    {
        [Column("SKUNumber")]
        public string SKUNumber { get; set; }

        [Column("ProductID")]
        public string ProductID { get; set; }

        [Column("UOM")]
        public string UOM { get; set; }

        [Column("Unit")]
        public string Unit { get; set; }

        [Column("ShortDescription")]
        public string ShortDescription { get; set; }

        [Column("Suppress")]
        public bool Suppress { get; set; }

        [Column("InventoryStatus")]
        public string InventoryStatus { get; set; }

        [Column("SortOrder")]
        public int SortOrder { get; set; }

        [Column("Trait")]
        public string Trait { get; set; }

        [Column("Value")]
        public string Value { get; set; }
    }

    [TableName("vw_SKUToTraitMap_SBC")]
    [PrimaryKey("ID")]
    [ExplicitColumns]
    public class SBCSkuToTraitMap
    {
        [Column("ID")]
        public int ID { get; set; }

        [Column("SKUNumber")]
        public string SKUNumber { get; set; }

        [Column("Trait")]
        public string Trait { get; set; }

        [Column("Value")]
        public string Value { get; set; }
    }

    [TableName("vw_SKUToTraitMap_SBUX")]
    [PrimaryKey("ID")]
    [ExplicitColumns]
    public class SBUXSkuToTraitMap
    {
        [Column("ID")]
        public int ID { get; set; }

        [Column("SKUNumber")]
        public string SKUNumber { get; set; }

        [Column("Trait")]
        public string Trait { get; set; }

        [Column("Value")]
        public string Value { get; set; }
    }

    [TableName("vw_SKUToTraitMap")]
    [PrimaryKey("ID")]
    [ExplicitColumns]
    public class SKUToTraitMap
    {
        [Column("ID")]
        public int ID { get; set; }

        [Column("SKUNumber")]
        public string SKUNumber { get; set; }

        [Column("Trait")]
        public string Trait { get; set; }

        [Column("Value")]
        public string Value { get; set; }
    }
}