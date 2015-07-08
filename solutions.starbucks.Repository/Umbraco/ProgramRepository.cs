using solutions.starbucks.Interfaces;
using solutions.starbucks.Model;
using solutions.starbucks.Repository.PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace solutions.starbucks.Repository.Umbraco
{
    public class ProgramRepository : IProgramRepository
    {
        private static UmbracoRepository _umbracoRepository = new UmbracoRepository();
        private static IProductsRepository _productRepository = new ProductsRepository();
        /// <summary>
        /// Maps Dashboard Tiles to their respective brand (SBX & SBC)
        /// </summary>
        /// <param name="content">Published dashboard tiles that are sub-content of the dashboard.</param>
        /// <returns>The dashboard tiles in an IEnumberable</returns>
        public IEnumerable<ProgramTile> MapHeroTiles(IPublishedContent content, string brand)
        {
            var programTiles = _umbracoRepository.GetPublishedChildItems(content)
                .Where((p => p.GetPropertyValue<string>("brandSelect") == brand.ToLower() && p.GetPropertyValue<string>("brandSelect") != "partner"))
                .Select(S => new ProgramTile
                {
                    RecipeGroupTitle = S.GetPropertyValue<string>("recipeGroupTitle"),
                    FeaturedBox = S.GetPropertyValue<IHtmlString>("featuredBox"),
                    RecipeGroupHelp = S.GetPropertyValue<IHtmlString>("recipeGroupHelp"),
                    Width = S.GetPropertyValue<int>("width"),
                    Height = S.GetPropertyValue<int>("height"),
                    BrandSelect = S.GetPropertyValue<string>("brandSelect"),
                    Title = S.Name,
                    DesktopTileBanner = S.GetPropertyValue<string>("desktopTileBanner"),
                    BigMobileTileBanner = S.GetPropertyValue<string>("bigMobileTileBanner"),
                    SmallMobileTileBanner = S.GetPropertyValue<string>("smallMobileTileBanner"),
                    IsThisARecipeTile = S.GetPropertyValue<bool>("isThisARecipeTile")
               
                });

            return programTiles;
        }

        /// <summary>
        /// Maps Marketing Tiles for a specific program page
        /// </summary>
        /// <param name="content">Published dashboard tiles that are sub-content of the dashboard parent.</param>
        /// <returns>The dashboard tiles in an IEnumberable that are viewable for dual brand users only.</returns>
        public IEnumerable<SeasonalMarketing> MapMarketing(IPublishedContent content)
        {
            var marketingTiles = _umbracoRepository.GetPublishedChildItemsOfChildren(content)
                .Where(m => m.DocumentTypeAlias.ToLower() == "seasonalmarketing").Select(M => new SeasonalMarketing
                {
                    FeaturedBox = M.GetPropertyValue<IHtmlString>("featuredBox"),
                    Width = M.GetPropertyValue<int>("width"),
                    Height = M.GetPropertyValue<int>("height"),
                    Title = M.Name,
                    MarketingImage = M.GetPropertyValue<string>("marketingImage"),
                    ParentValue = M.Parent.Name,
                    ProgramMarketingItems = PopulateItems(M.GetPropertyValue<string>("programMarketingItem"))
                });

            return marketingTiles;
        }

        List<ProgramMarketingItem> PopulateItems(string unparsedItems)
        {
            List<ProgramMarketingItem> newItems = new List<ProgramMarketingItem>();
            if (unparsedItems != null)
            {
                string[] marketingItemValues = null;

                // Get each pipe-delimited string and parse it into a MarketingTile.ProgramMarketingItem
                List<string> pipeDelimitedItems = unparsedItems.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(s => s).ToList();
                foreach (var mi in pipeDelimitedItems)
                {
                    marketingItemValues = mi.Split('|');
                    if (marketingItemValues != null)
                    {
                        //mt.ProgramMarketingItems.Add(new ProgramMarketingItem { SKU = marketingItemValues[0] ?? "", Name = marketingItemValues[1] ?? "", MaxQuantity = marketingItemValues[2] ?? "0" });
                        newItems.Add(new ProgramMarketingItem { SKU = marketingItemValues[0] ?? "", Name = marketingItemValues[1] ?? "", MaxQuantity = marketingItemValues[2] ?? "0" });
                    }
                }
            }
            return newItems;
        }

        /// <summary>
        /// Maps Recipe Tiles for a specific program page
        /// </summary>
        /// <param name="content">Published dashboard tiles that are sub-content of the dashboard parent.</param>
        /// <remarks>
        /// This retrieves recipes and ingredients. The ingredients are pulled based on the parent recipe name.
        /// </remarks>
        /// <returns>The dashboard tiles in an IEnumberable that are viewable for dual brand users only.</returns>
        public IEnumerable<SeasonalRecipe> MapRecipes(IPublishedContent content)
        {
            var recipeTiles = _umbracoRepository.GetPublishedChildItemsOfChildren(content)
                .Where(r => r.DocumentTypeAlias.ToLower() == "seasonalrecipe")
                .Select(R => new SeasonalRecipe
                {
                    FeaturedBox = R.GetPropertyValue<IHtmlString>("featuredBox"),
                    Width = R.GetPropertyValue<int>("width"),
                    Height = R.GetPropertyValue<int>("height"),
                    BrandSelect = R.GetPropertyValue<string>("brandSelect"),
                    RecipeImage = R.GetPropertyValue<string>("recipeImage"),
                    RecipeID = R.Name.Replace(" ", "-"),
                    RecipeName = R.Name, 
                    ParentValue = R.Parent.Name,
                    Ingredients = _umbracoRepository.GetPublishedChildItemsOfChildrensChildren(content).Where(i => i.DocumentTypeAlias.ToLower() == "seasonalingredient" && i.Parent.Name == R.Name).Select(I => new SeasonalIngredient
                    {
                        IngredientName = _productRepository.GetProductNameFromSKU(I.GetPropertyValue<string>("ingredientSKU")),
                        BeverageYield = I.GetPropertyValue<string>("beverageYield"),
                        IngredientSKU = I.GetPropertyValue<string>("ingredientSKU")
                    })

                });

            return recipeTiles;
        }

        public ProgramModel GetProgramAttributes(IPublishedContent content)
        {
            var model = new ProgramModel();
            model.ArchiveProgram = content.GetPropertyValue<bool>("archiveProgram");
            List<IPublishedContent> programs = _umbracoRepository.GetPublishedChildItems(content.Parent).Where(p => p.DocumentTypeAlias.ToLower() == "program").ToList();
            model.CurrentProgramURL = "#";
            DateTime programDate = new DateTime(2000, 1, 1);
            foreach (IPublishedContent i in programs)
            {
                if (i.GetPropertyValue<bool>("archiveProgram")==false)
                {
                    if (i.CreateDate > programDate)
                    {
                        model.CurrentProgramURL = i.Url;
                    }
                }
            }
            model.ProgramBackground = content.GetPropertyValue<string>("programBackground");
            var URL = _umbracoRepository.GetPublishedChildItems(content).Where(p => p.DocumentTypeAlias.ToLower() == "programproducts").Select(p => p.Url);
            if (URL.SingleOrDefault() != null)
            {
                model.ProgramProductsLink = URL.SingleOrDefault().ToString();
            }
            else
            {
                model.ProgramProductsLink = "#";
            }
            model.RecipeCardUpload = content.GetPropertyValue<string>("recipeCardUpload");
            return model;
            
        }

        //public string GetProgramBackground(IPublishedContent content)
        //{

        //    return content.GetPropertyValue<string>("programBackground");
        //}

        //public string GetRecipeCardUpload(IPublishedContent content)
        //{

        //    return content.GetPropertyValue<string>("recipeCardUpload");
        //}

        public string GetProgramBrand(IPublishedContent content)
        {
            //This probably isn't right, defaulting to SBX if not assigned...
            //Values should always be defined for these
            if (content.HasValue("programBrandSelect"))
            {
                return content.GetPropertyValue<string>("programBrandSelect");
            }
            else
            {
                return "sbux";
            }
        }

        //public string GetProductLink(IPublishedContent content)
        //{
        //    var URL = _umbracoRepository.GetPublishedChildItems(content).Where(p => p.DocumentTypeAlias.ToLower() == "programproducts").Select(p => p.Url);
        //    if (URL.SingleOrDefault() != null)
        //    {
        //        return URL.SingleOrDefault().ToString();
        //    }
        //    else
        //    {
        //        return "#";
        //    }
            
        //}
        
    }
}