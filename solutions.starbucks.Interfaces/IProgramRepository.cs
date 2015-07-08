using solutions.starbucks.Model;
using System.Collections.Generic;
using Umbraco.Core.Models;

namespace solutions.starbucks.Interfaces
{
    public interface IProgramRepository
    {
        IEnumerable<ProgramTile> MapHeroTiles(IPublishedContent content, string brand);
        IEnumerable<SeasonalMarketing> MapMarketing(IPublishedContent content);
        IEnumerable<SeasonalRecipe> MapRecipes(IPublishedContent content);
        ProgramModel GetProgramAttributes(IPublishedContent content);
        string GetProgramBrand(IPublishedContent content);
    }
}