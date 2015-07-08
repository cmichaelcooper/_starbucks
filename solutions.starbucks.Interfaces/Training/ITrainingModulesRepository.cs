using solutions.starbucks.Model.Training;
using System.Collections.Generic;
using Umbraco.Core.Models;

namespace solutions.starbucks.Interfaces
{
    public interface ITrainingModulesRepository
    {
        IEnumerable<Module> Modules(IPublishedContent content);
        IEnumerable<Category> Categories(int dataTypeId);
        
    }
}