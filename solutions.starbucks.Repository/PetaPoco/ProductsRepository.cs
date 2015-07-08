using solutions.starbucks.Interfaces;
using solutions.starbucks.Model;
using solutions.starbucks.Model.Pocos;
using solutions.starbucks.Repository.Umbraco;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace solutions.starbucks.Repository.PetaPoco
{
    public class ProductsRepository : IProductsRepository
    {
        protected UmbracoDatabase _db = ApplicationContext.Current.DatabaseContext.Database;
        private static UmbracoRepository _umbracoRepository = new UmbracoRepository();

        public ProductListing GetProduct(string productId)
        {
            //var queryText = Sql.Builder.Append("SELECT * FROM vw_Products_SBUX WHERE ProductID=@0 AND Suppress=0", productID);
            var queryText = Sql.Builder.Append("SELECT TOP 1 * FROM vw_ProductListing WHERE vw_ProductListing.Suppress=@0 AND vw_ProductListing.ProductID=@2", false, "Web_Full_Size", productId);
            var query = _db.Query<ProductListing>(queryText);
            return query.Single();
        }

        //public IEnumerable<ProductValues> GetProductWithProductId(string productId)
        //{
        //    //var queryText = Sql.Builder.Append("SELECT * FROM vw_Products_SBUX WHERE ProductID=@0 AND Suppress=0", productID);
        //    var query = from c in GetAllProducts(productId).Where(p => p.ProductID == productId) select c;
        //    //var query = GetAllProducts().Where(p => p.ProductID == productId);
        //    return query;
        //}

        public IEnumerable<ProductDetails> GetProductFromCombinedProducts(string productId)
        {
            var queryText = Sql.Builder.Append("SELECT CB.ProductID, CB.SiteId FROM vw_CombinedBrandProducts CB WHERE ProductID = @0 ", productId);

            var query = from cb in _db.Query<ProductListing>(queryText).ToList().Select(p => new ProductDetails
            {
                ProductID = p.ProductID,
                SiteId = p.SiteId.Split(','),
                ProductInformation = GetProduct(p.ProductID),
                ProductTraitValues = GetProductTraitsByProductId(p.ProductID),
                SkuInformation = GetSkusForProduct(p.ProductID),
            })
                        select cb;

            return query;
        }

        public IEnumerable<ProductDetails> GetCombinedProducts()
        {
            var queryText = Sql.Builder.Append("SELECT DISTINCT CB.ProductID, CB.SiteId, PL.Name FROM vw_CombinedBrandProducts AS CB LEFT OUTER JOIN vw_ProductListing AS PL ON CB.ProductID = PL.ProductID ORDER BY CB.ProductId");

            var query = from cb in _db.Query<ProductListing>(queryText).ToList().Select(p => new ProductDetails
            {
                ProductID = p.ProductID,
                SiteId = p.SiteId.Split(','),
                ProductInformation = GetProduct(p.ProductID),
                ProductTraitValues = GetProductTraitsByProductId(p.ProductID),
                SkuInformation = GetSkusForProduct(p.ProductID),
            }) select cb;

            return query;
        }

        public IEnumerable<ProductTraits> GetProductTraitsByProductId(string productId)
        {
            var queryText = Sql.Builder.Append("SELECT * FROM vw_SiteProductTraits WHERE vw_SiteProductTraits.ProductID=@0 ", productId);
            var query = _db.Query<ProductTraits>(queryText);

            return query;
        }

        public IEnumerable<ProductDetails> GetProductsWithType(string typeId)
        {
            var queryText = Sql.Builder.Append("SELECT top 2 percent * FROM vw_CombinedBrandProducts CB LEFT JOIN vw_SiteProductTraits SPT ON CB.ProductID = SPT.ProductID WHERE SPT.TypeID = @0 ORDER BY CB.ProductID", typeId);
            var query = from cb in _db.Query<ProductListing>(queryText).ToList().Select(p => new ProductDetails
            {
                ProductID = p.ProductID,
                SiteId = p.SiteId.Split(','),
                ProductInformation = GetProduct(p.ProductID),
                ProductTraitValues = GetProductTraitsByProductId(p.ProductID),
                SkuInformation = GetSkusForProduct(p.ProductID),
            })
                        select cb;

            return query;
        }

        //public IEnumerable<ProductFilters> GetProductTraitsByProductIdAndType(string typeId)
        //{
        //    var queryText = Sql.Builder.Append("SELECT * FROM vw_SiteProductTraits WHERE vw_SiteProductTraits.TypeID = @0", typeId);
        //    var query = _db.Query<ProductTraits>(queryText).Select(p => new ProductFilters
        //    {
        //        Trait = p.Trait,
        //        Value = p.Value,
        //        TypeID = p.TypeID


        //    });

        //    return query;
        //}

        public IEnumerable<SKUS> GetSkusForProduct(string productId)
        {
            var queryText = Sql.Builder.Append("SELECT DISTINCT vw_SKUS.SKUNumber, vw_SKUS.UOM, vw_SKUS.ProductID, vw_SKUToTraitMap.Trait, vw_SKUToTraitMap.Value FROM vw_SKUS LEFT JOIN  vw_SKUToTraitMap ON vw_SKUS.SKUNumber = vw_SKUToTraitMap.SKUNumber WHERE vw_SKUS.ProductID=@0 AND Suppress=@1 AND SiteId <> 'SBXOCS'", productId, 0);
            var query = _db.Query<SKUS>(queryText);
                
            //    .Select(s => new SKU
            //{
            //    SKUNumber = s.SKUNumber,
            //    UOM = s.UOM,
            //    ProductID = s.ProductID,
            //    Trait = s.Trait,
            //    Value = s.Value.Trim()
            //    //Trait = s.trvw_SKUToTraitMap
            //    //Unit = s.Unit,
            //    //ShortDescription = s.ShortDescription,
            //    //Suppress = s.Suppress,
            //    //InventoryStatus = s.InventoryStatus,
            //    //SortOrder = s.SortOrder

            //});
            return query;
        }
        //public List<ProductFilters> GetAllProductFilters()
        //{
        //    //var queryText = Sql.Builder.Append("SELECT * FROM vw_Products_SBUX WHERE ProductID=@0 AND Suppress=0", productID);
        //    var queryText = Sql.Builder.Append("SELECT DISTINCT Trait FROM vw_SiteProductTraits ");
        //    var query = _db.Query<ProductTraits>(queryText).Select(p => new ProductFilters
        //    {
        //        Trait = p.Trait
              
        //    });
        //    return query.ToList();
        //}
        public ProductValues GetProgramProducts(string productID, string brand)
        {
            //var queryText = Sql.Builder.Append("SELECT * FROM vw_Products_SBUX WHERE ProductID=@0 AND Suppress=0", productID);
            var queryText = Sql.Builder.Append("SELECT * FROM vw_Products_SBUX WHERE vw_Products_SBUX.ProductID=@0 AND vw_Products_SBUX.Suppress=@1", productID, false);
            var query = _db.Query<SBUXProductsForOutput>(queryText).Select(p => new ProductValues
            {
                ProductID = p.ProductID,
                Name = p.Name,
                Description = p.Description,
                ShortDescription = p.ShortDescription,
                Title = p.Title,
                NutritionFacts = p.NutritionFacts,
                SortOrder = p.SortOrder,
                Suppress = p.Suppress,
                InventoryStatus = p.InventoryStatus,
                FileName = p.FileName,
                SiteId = p.SiteId
            }).SingleOrDefault();
            return query;
        }

        public string GetProductID(string skuNumber)
        {
            var queryText = Sql.Builder.Append("SELECT ProductID FROM vw_SKUS_SBUX WHERE SKUNumber=@0 AND Suppress=0", skuNumber);
            var query = _db.Query<SBUXSku>(queryText).Select(p => p.ProductID);

            return query.ToString();
        }

        public SKU GetSKUDetails(string skuNumber, string brand )
        {
            var leftTable = "vw_SKUS_" + brand;
            var rightTable = "vw_SKUToTraitMap_" + brand;
            //var queryText = Sql.Builder.Append("SELECT * FROM vw_SKUS_SBUX LEFT JOIN vw_SKUToTraitMap_SBUX ON vw_SKUS_SBUX.SKUNumber = vw_SKUToTraitMap_SBUX.SKUNumber WHERE vw_SKUS_SBUX.SKUNumber=@0 AND vw_SKUS_SBUX.Suppress=@1 AND vw_SKUToTraitMap_SBUX.Trait=@2", skuNumber, false, "Size");
            var queryText = Sql.Builder
                .Select("*")
                .From(leftTable)
                .LeftJoin(rightTable)
                .On(leftTable + ".SKUNumber = " + rightTable + ".SKUNumber")
                .Where(leftTable + ".SKUNumber=@0 AND " + leftTable + ".Suppress=@1 AND " + rightTable + ".Trait=@2", skuNumber, false, "Size");
            if (brand == "SBUX")
            {
                var query = _db.Query<SBUXSku>(queryText).Select(s => new SKU
                    {
                        SKUNumber = s.SKUNumber,
                        Trait = s.Trait,
                        UOM = s.UOM,
                        Value = s.Value,
                        ProductID = s.ProductID

                    }).SingleOrDefault();
                return query;
            }
            else
            {
                var query = _db.Query<SBCSku>(queryText).Select(s => new SKU
                {
                    SKUNumber = s.SKUNumber,
                    Trait = s.Trait,
                    Value = s.Value,
                    UOM = s.UOM,
                    ProductID = s.ProductID

                }).SingleOrDefault();

                return query;
            }
            
            //var a = _db.SingleOrDefault<SBUXSku>(queryText);
        }

        public ProductValues GetProductsFromSKU(string productID, string brand)
        {
            //var queryText = Sql.Builder.Append("SELECT * FROM vw_Products_SBUX LEFT JOIN vw_ProductImages_SBUX ON vw_Products_SBUX.ProductID = vw_ProductImages_SBUX.ProductID WHERE vw_Products_SBUX.ProductID=@0 AND vw_Products_SBUX.Suppress=@1 AND vw_ProductImages_SBUX.Type=@2", productID, false, "Web_Full_Size");
            var queryText = Sql.Builder.Append("SELECT * FROM vw_Products_SBUX WHERE vw_Products_SBUX.ProductID=@0 AND vw_Products_SBUX.Suppress=@1", productID, false);
            if (brand == "SBC")
                queryText = Sql.Builder.Append("SELECT * FROM vw_Products_SBC WHERE vw_Products_SBC.ProductID=@0 AND vw_Products_SBC.Suppress=@1", productID, false);
            //var leftTable = "vw_Products_" + brand;
            //var rightTable = "vw_ProductImages_" + brand;

            //var queryText = Sql.Builder
            //    .Select("*")
            //    .From(leftTable)
            //    .LeftJoin(rightTable)
            //    .On(leftTable + ".ProductID = " + rightTable + ".ProductID")
            //    .Where(leftTable + ".ProductID=@0 AND " + leftTable + ".Suppress=@1 AND " + rightTable + ".Type=@2", productID, false, "Web_Full_Size");
            if (brand == "SBUX")
            {
                var query = _db.Query<SBUXProductsForOutput>(queryText).Select(p => new ProductValues
                {
                    ProductID = p.ProductID,
                    Name = p.Name,
                    Description = p.Description,
                    ShortDescription = p.ShortDescription,
                    Title = p.Title,
                    NutritionFacts = p.NutritionFacts,
                    SortOrder = p.SortOrder,
                    Suppress = p.Suppress,
                    InventoryStatus = p.InventoryStatus,
                    FileName = p.FileName, 
                    SiteId = p.SiteId
                }).SingleOrDefault();
                return query;
            }
            else
            {
                var query = _db.Query<SBCProductsDeets>(queryText).Select(p => new ProductValues
                {
                    ProductID = p.ProductID,
                    Name = p.Name,
                    Description = p.Description,
                    ShortDescription = p.ShortDescription,
                    Title = p.Title,
                    NutritionFacts = p.NutritionFacts,
                    SortOrder = p.SortOrder,
                    Suppress = p.Suppress,
                    InventoryStatus = p.InventoryStatus,
                    FileName = p.FileName,
                    SiteId = p.SiteId
                }).SingleOrDefault();
                return query;
            }

            //var a = _db.SingleOrDefault<SBUXProductsForOutput>(queryText);
        }

        public string GetProductNameFromSKU(string skuNumber)
        {
            var queryText = Sql.Builder.Append("SELECT TOP 1 ProductID FROM vw_SKUS WHERE SKUNumber=@0 AND Suppress=0", skuNumber);
            var query = _db.Query<SKUS>(queryText).Select(p => p.ProductID).SingleOrDefault();
            var prodQueryText = Sql.Builder.Append("SELECT TOP 1 Name FROM vw_ProductListing WHERE ProductID=@0", query.ToString());
            var prodQuery = _db.Query<ProductListing>(prodQueryText).Select(p => p.Name).SingleOrDefault();
            return prodQuery.ToString();
        }

        public IEnumerable<SeasonalIngredient> MapIngredientsToProducts(IPublishedContent content)
        {
            var recipeTiles = _umbracoRepository.GetPublishedChildItemsOfChildrensChildren(content.Parent).Where(i => i.DocumentTypeAlias.ToLower() == "seasonalingredient").OrderBy(p => p.SortOrder).Select(
                I => new SeasonalIngredient
                    {
                        IngredientName = I.Name,
                        BeverageYield = I.GetPropertyValue<string>("beverageYield"),
                        IngredientSKU = I.GetPropertyValue<string>("ingredientSKU"),
                        ProductPageSortOrder = I.GetPropertyValue<string>("productPageSortOrder"),
                    }).OrderBy(p => p.ProductPageSortOrder);
            return recipeTiles;
        }
        public IEnumerable<SeasonalIngredient> MapAdditionalIngredientsToProducts(IPublishedContent content)
        {
            // get the id of the type (by alias) 
            IContentTypeService contentTypeService = ApplicationContext.Current.Services.ContentTypeService;
            IContentType mytype = contentTypeService.GetContentType("seasonalingredient");

            // get all content by type
            IContentService contentService = ApplicationContext.Current.Services.ContentService;
            IEnumerable<IContent> items = contentService.GetContentOfContentType(mytype.Id);
            var recipeTiles = contentService.GetContentOfContentType(mytype.Id).Where(p => p.ParentId == content.Parent.Id).Select(
                I => new SeasonalIngredient
                {
                    IngredientName = I.Name,
                    BeverageYield = I.GetValue<string>("beverageYield"),
                    IngredientSKU = I.GetValue<string>("ingredientSKU"),
                    ProductPageSortOrder = I.GetValue<string>("productPageSortOrder"),
                });
            
            return recipeTiles;
        }
    }
}