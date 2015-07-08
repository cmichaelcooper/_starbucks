using solutions.starbucks.Interfaces;
using solutions.starbucks.Model;
using solutions.starbucks.Model.Pocos;
using solutions.starbucks.web.Controllers.Masters;
using System;
using System.Linq;
using System.Web.Mvc;

namespace solutions.starbucks.web.Controllers
{
    public class BagController : GenericMasterController
    {
        private readonly IMemberAttributesRepository _memberRepository;
        private readonly IOrdersRepository _ordersRepository;

        public BagController(IMemberAttributesRepository memberRepository, IOrdersRepository ordersRepository)
        {
            _ordersRepository = ordersRepository;
            _memberRepository = memberRepository;
        }

        [Authorize]
        public ActionResult Bag()
        {
            BagViewModel model = new BagViewModel();
            model.Order = _ordersRepository.GetOrderInBag();
            model.Order.OrderItems.OrderBy(i => i.IsMarketing ? 0 : 1);
            string accountNumber = GetAccountNumber();

            if (!string.IsNullOrEmpty(accountNumber))
            {
                model.CustomerSites = _memberRepository.GetCustomerAttributes(accountNumber).ToList();

                //Set default account if missing
                if (string.IsNullOrEmpty(model.Order.AccountSiteNumber))
                {
                    CustomerAttributes defaultSite = model.CustomerSites.Where(c => c.AccountSiteUsePrimary.ToUpper() == "Y").FirstOrDefault();

                    if (defaultSite != null) 
                    {
                        model.Order.AccountSiteNumber = defaultSite.AccountSiteNumber;
                    }
                    else if (model.CustomerSites != null && model.CustomerSites.Count > 0)
                    {
                        model.Order.AccountSiteNumber = model.CustomerSites.First().AccountSiteNumber;
                    }
                }
            }

            return View("Bag", model);
        }

        [Authorize]
        public ActionResult ConfirmOrder(Guid OrderId)
        {
            BagViewModel model = new BagViewModel();
            model.Order = _ordersRepository.GetOrder(OrderId);
            model.Order.OrderItems.OrderBy(i => i.IsMarketing ? 0 : 1);
            string accountNumber = GetAccountNumber();

            if (!string.IsNullOrEmpty(accountNumber))
            {
                model.CustomerSites = _memberRepository.GetCustomerAttributes(accountNumber).ToList();
                model.SelectedCustomerSite = model.CustomerSites.Where(s => s.AccountSiteNumber == model.Order.AccountSiteNumber).FirstOrDefault();
            }

            return View("ConfirmOrder", model);
        }

        private string GetAccountNumber() 
        {
            string retVal = "";

            if (Session["CurrentAccount"] != null && (Session["CurrentAccount"] ?? "") != "")
            {
                retVal = Session["CurrentAccount"].ToString();
            }

            if (retVal == "" 
                && Request.Cookies["AccountAttributes"] != null
                && ((Request.Cookies["AccountAttributes"]["CurrentAccount"] ?? "") != ""))
            {
                retVal = Request.Cookies["AccountAttributes"]["CurrentAccount"].ToString();
            }

            return retVal ?? "";
        }

    }
}
