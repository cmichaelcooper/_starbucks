using solutions.starbucks.Model.Pocos;
using System.Collections.Generic;
using System.Web;

namespace solutions.starbucks.Model.Masters
{
    public class MasterDashboardModel : MasterModel
    {
        public IHtmlString Body { get; set; }

        public IEnumerable<Orders> UserOrders { get; set; }
    }

}