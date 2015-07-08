using System.Web;

namespace solutions.starbucks.Model.Masters
{
    public class MasterTextPageModel : MasterModel
    {
        public IHtmlString IntroText { get; set; }

        public IHtmlString BodyText { get; set; }
        
        public IHtmlString SupportContent { get; set; }
    }

}