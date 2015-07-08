using solutions.starbucks.Model;
using Umbraco.Core.Models;

namespace solutions.starbucks.Interfaces
{
    public interface ILegalTextPageRepository
    {
        LegalTextPageModel GetLegalPageProperties(IPublishedContent content);
    }
}