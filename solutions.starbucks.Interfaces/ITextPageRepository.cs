using solutions.starbucks.Model;
using Umbraco.Core.Models;

namespace solutions.starbucks.Interfaces
{
    public interface ITextPageRepository
    {
        TextPageModel GetTextPageProperties(IPublishedContent content);
    }
}