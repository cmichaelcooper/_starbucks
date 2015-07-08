using solutions.starbucks.Model.Masters;
using System.Web;

namespace solutions.starbucks.Model
{
    public class HomeModel : MasterModel
    {
        public IHtmlString BodyText { get; set; }
        
        public IHtmlString LeftPath { get; set; }
        
        public IHtmlString MiddlePath { get; set; }
        
        public IHtmlString RightPath { get; set; }
    }
}