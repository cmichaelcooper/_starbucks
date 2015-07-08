using solutions.starbucks.Model.Masters;
using solutions.starbucks.Model.Pocos;
using System.Collections.Generic;
using System.Linq;

namespace solutions.starbucks.Model
{
    public class ProgramProductDetailsModel : GenericMasterModel
    {
        //public List<SBUXProductsForOutput> SBUXProducts { get; set; }

        //public List<SBUXProductImage> SBUXProductImages { get; set; }

        //public List<SBUXSku> SBUXSkus { get; set; }

        //public List<SBCProductsDeets> SBCProducts { get; set; }

        //public List<SBCProductImage> SBCProductImages { get; set; }

        //public List<SBCSku> SBCSkus { get; set; }

        public List<ProductValues> Products { get; set; }

        public List<ProductImage> ProductImages { get; set; }

        public List<SKU> Skus { get; set; }

        public string ProductBrandUrl { get; set; }

        public Dictionary<string, int> ProgramProductsDictionary { get; set; }

        //public IEnumerable<SBUXSkuToTraitMap> SBUXSkuToTraits { get; set; }

        //public IEnumerable<SBUXProductImage> SBUXImages { get; set; }

        //public IEnumerable<SBCSkuToTraitMap> SBCSkuToTraits { get; set; }

        //public IEnumerable<SBCProductImage> SBCImages { get; set; }

        public IEnumerable<SKUToTraitMap> SkuToTraits { get; set; }

    }

    public class ProductValues
    {
        public string ProductID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ShortDescription { get; set; }

        public string Title { get; set; }

        public string NutritionFacts { get; set; }

        public string SortOrder { get; set; }

        public string Suppress { get; set; }

        public string InventoryStatus { get; set; }

        public string FileName { get; set; }
        
        public string SiteId { get; set; }


    }


    public class ProductDetails
    {
        public string ProductID { get; set; }

        public string[] SiteId { get; set; }

        public ProductListing ProductInformation { get; set; }

        public IEnumerable<ProductTraits> ProductTraitValues { get; set; }

        public IEnumerable<SKUS> SkuInformation { get; set; }

        public bool IsCoffee 
        {
            get
            {
                bool retVal = false;
                this.ProductTraitValues.ToList().ForEach(t => 
                {
                    switch (t.TypeID) 
                    {
                        case "Iced_Coffee":
                        case "Espresso":
                        case "Instant_Coffee":
                        case "Drip_Brewed":
                            retVal = true;
                            break;
                        default: 
                            break;
                    }
                });
                return retVal;
            }
        }

    }

    public class ProductFilters
    {
        public string Trait { get; set; }

        public string Value { get; set; }

        public string TypeID { get; set; }

    }

    //public class ProductTraits
    //{
    //    public string ProductId { get; set; }

    //    public string Trait { get; set; }

    //    public string Value { get; set; }

    //}
    public class ProductImage
    {
        public string ProductID { get; set; }

        public string Type { get; set; }

        public string FileName { get; set; }
    }

    public class SKU
    {
        public string SKUNumber { get; set; }

        public string ProductID { get; set; }

        public string UOM { get; set; }

        public string Unit { get; set; }

        public string ShortDescription { get; set; }

        public bool Suppress { get; set; }

        public string InventoryStatus { get; set; }

        public int SortOrder { get; set; }

        public string InventoryStatusMessage { get; set; }

        public string Trait { get; set; }

        public IEnumerable<SkuTraits> SkuTraits { get; set; }

        public string Value { get; set; }
    }

    public class SkuTraits
    {
        public int ID { get; set; }

        public string SKUNumber { get; set; }

        public string Trait { get; set; }

        public string Value { get; set; }
    }
}
