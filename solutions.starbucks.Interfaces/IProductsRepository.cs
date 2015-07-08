using solutions.starbucks.Model;
using solutions.starbucks.Model.Pocos;
using System.Collections.Generic;
using Umbraco.Core.Models;

namespace solutions.starbucks.Interfaces
{
    public interface IProductsRepository
    {
        ProductValues GetProgramProducts(string productID, string brand);
        string GetProductID(string skuNumber);
        SKU GetSKUDetails(string skuNumber, string brand);
        ProductValues GetProductsFromSKU(string productID, string brand);
        IEnumerable<SeasonalIngredient> MapIngredientsToProducts(IPublishedContent content);
        IEnumerable<SeasonalIngredient> MapAdditionalIngredientsToProducts(IPublishedContent content);
        ProductListing GetProduct(string productId);
        //List<ProductFilters> GetAllProductFilters();
        IEnumerable<ProductTraits> GetProductTraitsByProductId(string productId);
        string GetProductNameFromSKU(string skuNumber);
        IEnumerable<ProductDetails> GetProductFromCombinedProducts(string productId);
        IEnumerable<ProductDetails> GetCombinedProducts();
        //IEnumerable<ProductValues> GetProductWithProductId(string productId);
        IEnumerable<ProductDetails> GetProductsWithType(string typeId);
    }
}