using solutions.starbucks.Interfaces;
using solutions.starbucks.Model;
using solutions.starbucks.Model.Pocos;
using solutions.starbucks.Repository.PetaPoco;
using solutions.starbucks.web.Controllers.Masters;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;

namespace solutions.starbucks.web.Controllers
{
    public class ConfirmOrderController : GenericMasterController
    {
        private readonly IMemberAttributesRepository _memberRepository;
        private readonly IOrdersRepository _ordersRepository;

        public ConfirmOrderController(IMemberAttributesRepository memberRepository, IOrdersRepository ordersRepository)
        {
            _ordersRepository = ordersRepository;
            _memberRepository = memberRepository;
        }
        
        [Authorize]
        public ActionResult ConfirmOrder(Guid OrderId)
        {
            CartViewModel model = new CartViewModel();
            model.Order = _ordersRepository.GetOrder(OrderId);

            return View("ConfirmOrder", model);
        }

    }
}
