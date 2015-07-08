using solutions.starbucks.Model.Masters;
using solutions.starbucks.Model.Pocos;

namespace solutions.starbucks.Model
{

    public class OrderDetailsModel : MasterDashboardModel
    {
        public Orders Order { get; set; }
    }
}