using solutions.starbucks.Model.Masters;
using solutions.starbucks.Model.Pocos;
using System.Collections.Generic;
using System.Web;

namespace solutions.starbucks.Model
{
    public class FundamentalsModel : MasterModel
    {
        public IHtmlString BodyText { get; set; }

        public IHtmlString LeftPath { get; set; }

        public IHtmlString MiddlePath { get; set; }

        public IHtmlString RightPath { get; set; }

        public Invites Invite { get; set; }

        public string HeaderContent { get; set; }
   
        public IEnumerable<ContentBlock> ContentBlocks { get; set; }
    }

    public class ContentBlock
    {
        public int Id { get; set; }
        public string ContentBox { get; set; }
        public string CssClass { get; set; }
        public int SortOrder { get; set; }
    }

}