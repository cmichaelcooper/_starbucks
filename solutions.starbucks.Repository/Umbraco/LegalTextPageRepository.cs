using solutions.starbucks.Interfaces;
using solutions.starbucks.Model;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace solutions.starbucks.Repository.Umbraco
{
    public class LegalTextPageRepository : ILegalTextPageRepository
    {
        public LegalTextPageModel GetLegalPageProperties(IPublishedContent content)
        {
            var model = new LegalTextPageModel();

            model.becomeACustomerLinkFooter = content.GetPropertyValue<string>("becomeACustomerLinkFooter");
            model.BodyText = content.GetPropertyValue<IHtmlString>("bodyText");
            model.Topics = content.GetPropertyValue<IHtmlString>("topics");
            model.Intro = content.GetPropertyValue<IHtmlString>("intro");
            model.UmbracoDictionaryItem = umbraco.library.GetDictionaryItem("BecomeACustomer");

            return model;
        }
    }
}