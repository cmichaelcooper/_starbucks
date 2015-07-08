using solutions.starbucks.Interfaces;
using solutions.starbucks.Model;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace solutions.starbucks.Repository.Umbraco
{
    public class TextPageRepository : ITextPageRepository
    {
        public TextPageModel GetTextPageProperties(IPublishedContent content)
        {
            var model = new TextPageModel();
            model.BecomeACustomerText = umbraco.library.GetDictionaryItem("BecomeACustomer");
            model.BecomeACustomerLinkFooter = content.GetPropertyValue<string>("becomeACustomerLinkFooter");
            model.BecomeACustomerLinkMid = content.GetPropertyValue<IHtmlString>("becomeACustomerLinkMid");
            model.DesktopHeaderImage = content.GetPropertyValue<string>("desktopHeaderImage");
            model.TabletHeaderImage = content.GetPropertyValue<string>("tabletHeaderImage");
            model.BigMobileHeaderImage = content.GetPropertyValue<string>("bigMobileHeaderImage");
            model.SmallMobileHeaderImage = content.GetPropertyValue<string>("smallMobileHeaderImage");

            return model;
        }
    }
}