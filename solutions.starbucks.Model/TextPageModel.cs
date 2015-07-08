using solutions.starbucks.Model.Masters;
using System.Web;

namespace solutions.starbucks.Model
{
    public class TextPageModel : MasterTextPageModel
    {
        public string BecomeACustomerLinkFooter { get; set; }

        public string BecomeACustomerText { get; set; }

        public IHtmlString BecomeACustomerLinkMid { get; set; }

        public string DesktopHeaderImage { get; set; }

        public string TabletHeaderImage { get; set; }

        public string BigMobileHeaderImage { get; set; }

        public string SmallMobileHeaderImage { get; set; }
    }
}