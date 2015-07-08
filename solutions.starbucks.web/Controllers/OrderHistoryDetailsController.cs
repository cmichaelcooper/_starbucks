using solutions.starbucks.Interfaces;
using solutions.starbucks.Model;
using solutions.starbucks.web.Controllers.Masters;
using System;
using System.Web.Mvc;

namespace solutions.starbucks.web.Controllers
{
    public class OrderHistoryDetailsController : MasterDashboardController
    {
        private readonly IMemberAttributesRepository _memberRepository;
        private readonly IOrdersRepository _ordersRepository;

        public OrderHistoryDetailsController(IMemberAttributesRepository memberRepository, IOrdersRepository ordersRepository)
        {
            _ordersRepository = ordersRepository;
            _memberRepository = memberRepository;
        }

        [Authorize]
        public ActionResult OrderHistoryDetails(string orderID)
        {           
            OrderDetailsModel model = new OrderDetailsModel();
            model.Order = _ordersRepository.GetOrder(new Guid(orderID));
            return View(model);
        }
    }
}