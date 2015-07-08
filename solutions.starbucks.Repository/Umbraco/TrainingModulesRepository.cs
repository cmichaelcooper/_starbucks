using solutions.starbucks.Interfaces;
using solutions.starbucks.Model.Training;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace solutions.starbucks.Repository.Umbraco
{
    public class TrainingModulesRepository : ITrainingModulesRepository
    {

        private static UmbracoRepository _umbracoRepository = new UmbracoRepository();


        /// <summary>
        /// Training modules 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public IEnumerable<Module> Modules(IPublishedContent content)
        {
            var trainingModulesContent = content.AncestorsOrSelf("Home").Single().Descendant("TrainingModules");

            // Look up modules for given category
            List<Module> modules = _umbracoRepository.GetPublishedChildItems(trainingModulesContent)
                            .Select(M => new Module
                            {
                                Id = M.Id,
                                Type = Module.GetTypeFromAlias(M.DocumentTypeAlias),
                                Title = M.GetPropertyValue<string>("ModuleTitle"),
                                ShortDescription = M.GetPropertyValue<string>("ShortDescription"),
                                LongDescription = M.GetPropertyValue<string>("LongDescription"),
                                CategoryNames = M.GetPropertyValue<string>("Categories").Split(new char[] { ',' }).ToList(),
                                Video = Module.GetTypeFromAlias(M.DocumentTypeAlias) == Module.Types.Video ? 
                                                                new Video(M.GetPropertyValue<string>("VideoEmbed")) :
                                                                null,
                                Pdf = Module.GetTypeFromAlias(M.DocumentTypeAlias) == Module.Types.Pdf ?
                                                                new Pdf(M.GetPropertyValue<string>("FileChooser"), M.GetPropertyValue<string>("SpanishFileChooser")) : 
                                                                null,
                                SortOrder = M.SortOrder
                            }).OrderBy(S => S.SortOrder).ToList();
            return modules;
        }

        /// <summary>
        /// Returns the categories by which training modules are organized. These are created by using datatype in the developer section of the umbraco cms.
        /// </summary>
        /// <param name="dataTypeId">The id used to look up the categories from the umbraco datatype library</param>
        /// <returns>List of categories for organizing training modules</returns>
        public IEnumerable<Category> Categories(int dataTypeId)
        {
            // Look up categories from datatypes
            List<Category> categories = new List<Category>();
            XPathNodeIterator preValueRootElementIterator = umbraco.library.GetPreValues(dataTypeId);
            preValueRootElementIterator.MoveNext(); //move to first
            XPathNodeIterator preValueIterator = preValueRootElementIterator.Current.SelectChildren("preValue", "");
            while (preValueIterator.MoveNext())
            {
                categories.Add(new Category(Convert.ToInt32(preValueIterator.Current.GetAttribute("id", "")), preValueIterator.Current.Value));
            }
            if (categories.Count() == 0) categories = null;
            return categories;
        }


    }
}