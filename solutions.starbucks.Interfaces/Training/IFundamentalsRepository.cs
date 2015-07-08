using solutions.starbucks.Model;
using System.Collections.Generic;
using Umbraco.Core.Models;

namespace solutions.starbucks.Interfaces
{
    public interface IFundamentalsRepository
    {
        IEnumerable<ContentBlock> ContentBlocks(IPublishedContent content);
        FundamentalsModel Fundamental(IPublishedContent content);
        
    }
}