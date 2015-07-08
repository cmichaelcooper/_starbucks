using System.Web;

namespace solutions.starbucks.Model.Masters
{
    public class MasterLegalModel : MasterModel
    {
        public IHtmlString Intro { get; set; }

        public IHtmlString Topics { get; set; }

        public IHtmlString BodyText { get; set; }
    }
}