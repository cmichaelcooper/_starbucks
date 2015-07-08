using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;


namespace solutions.starbucks.Repository.Umbraco
{
    public class UmbracoRepository
    {
        /// <summary>
        /// Maps IPublishedContent children
        /// </summary>
        /// <param name="content">Published content.</param>
        /// <returns>The published content</returns>
        /// <remarks>
        /// Maps children of content for use in the dashboard, seasonal program pages, etc. 
        /// </remarks>
        public IEnumerable<IPublishedContent> GetPublishedChildItems(IPublishedContent content)
        {
            var publishedContent = from c in content.Children
                                   orderby c.SortOrder
                                   select c;
            return publishedContent;
        }


        public IEnumerable<IPublishedContent> GetPublishedChildItemsOfChildren(IPublishedContent content)
        {
            var publishedContent = from c in content.Children
                                   orderby c.SortOrder
                                   from p in c.Children
                                   select p;
            return publishedContent;
        }

        public IEnumerable<IPublishedContent> GetPublishedChildItemsOfChildrensChildren(IPublishedContent content)
        {
            var publishedContent = from c in content.Children
                                   orderby c.SortOrder
                                   from p in c.Children
                                   orderby p.SortOrder
                                   from s in p.Children
                                   orderby s.SortOrder
                                   select s;
            return publishedContent;
        }
        /// <summary>
        /// Maps IPublishedContent parent items
        /// </summary>
        /// <param name="content">Published content.</param>
        /// <returns>The published content</returns>
        /// <remarks>
        /// Maps children of content for use in the dashboard, seasonal program pages, etc. 
        /// </remarks>
        //public IEnumerable<IPublishedContent> GetPublishedParentItems(IPublishedContent content)
        //{
        //    var publishedContent = from p in content.Parent
        //                           orderby p.SortOrder
        //                           where p.IsVisible()
        //                           select p;
        //    return publishedContent;
        //}

    }
}