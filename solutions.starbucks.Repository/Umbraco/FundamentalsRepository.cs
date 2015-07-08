using solutions.starbucks.Interfaces;
using solutions.starbucks.Model;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace solutions.starbucks.Repository.Umbraco
{
    public class FundamentalsRepository : IFundamentalsRepository
    {

        private static UmbracoRepository _umbracoRepository = new UmbracoRepository();

        /// <summary>
        /// Maps Fundamental section from Umbraco
        /// </summary>
        /// <param name="content">Content item to lookup Fundamental item attributes.</param>
        /// <returns>The model for the Fundamental item.</returns>
        public FundamentalsModel Fundamental(IPublishedContent content)
        {
            FundamentalsModel fundamental = new FundamentalsModel { 
                                                HeaderContent = content.GetPropertyValue<string>("HeaderContent")
                                            };
            return fundamental;
        }

        public IEnumerable<ContentBlock> ContentBlocks(IPublishedContent content)
        {
            // Look up modules for given Fundamental
            List<ContentBlock> contentBlocks = _umbracoRepository.GetPublishedChildItems(content)
                            .Where(s => s.DocumentTypeAlias == "ContentBlock")
                            .Select(M => new ContentBlock
                            {
                                Id = M.Id,
                                ContentBox = M.GetPropertyValue<string>("ContentBox"),
                                CssClass = M.GetPropertyValue<string>("CssClass"),
                                SortOrder = M.SortOrder
                            }).OrderBy(S => S.SortOrder).ToList();
            return contentBlocks;   
        }

       
    }
}