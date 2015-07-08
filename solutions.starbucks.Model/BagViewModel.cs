using solutions.starbucks.Model.Masters;
using solutions.starbucks.Model.Pocos;
using System.Collections.Generic;

namespace solutions.starbucks.Model
{
    public class BagViewModel : GenericMasterModel
    {
        public Orders Order { get; set; }

        public List<CustomerAttributes> CustomerSites { get; set; }

        public CustomerAttributes SelectedCustomerSite { get; set; }

        public bool BagIsEmpty 
        {
            get 
            {
                return (this.Order == null || this.Order.OrderItems == null || this.Order.OrderItems.Count == 0);
            }
        }

    }
}