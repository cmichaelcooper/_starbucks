using solutions.starbucks.Interfaces;
using solutions.starbucks.Model.Enums;
using solutions.starbucks.Model.Pocos;
using System;
using System.Linq;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace solutions.starbucks.web.Controllers
{
    public class ProgramOrderSurfaceController : SurfaceController
    {
        private readonly IMemberAttributesRepository _memberRepository;
        private readonly IOrdersRepository _orderRepository;
        protected static IProgramRepository _programRepository;

        public ProgramOrderSurfaceController(IMemberAttributesRepository memberRepository, IOrdersRepository orderRepository, IProgramRepository programRepository)
        {
            _memberRepository = memberRepository;
            _orderRepository = orderRepository;
            _programRepository = programRepository;

        }

        [HttpPost]
        public ActionResult ProcessYearRoundToAllProductsPage(FormCollection collection)
        {
            foreach (var p in collection.AllKeys.Where(p => p.Contains("SKU")))
            {
                string[] formValues = Request.Form.GetValues(p);
                int sum = 0;
                for (int i = 0; i < formValues.Length; i++)
                {
                    try
                    {
                        sum += int.Parse(formValues[i]);
                    }
                    catch (Exception e)
                    {
                    }

                }

                TempData[p] = sum;
            }

            // Add the Marketing SKUs.  The other items are added on the next page.
            foreach (var m in collection.AllKeys.Where(p => p.Contains("MKT-")))
            {
                string[] formValues = Request.Form.GetValues(m);
                int sum = 0;
                for (int i = 0; i < formValues.Length; i++)
                {
                    try
                    {
                        sum += int.Parse(formValues[i]);
                    }
                    catch (Exception e)
                    {
                    }
                }

                TempData[m] = sum;

            }

            foreach (var t in collection.AllKeys.Where(p => p.Contains("nextPage")))
            {
                var RedirectValue = collection[t];
                return new RedirectResult(RedirectValue);
            }

            return Redirect("/dashboard");
        }

        [HttpPost]
        public ActionResult ProcessOrder(FormCollection collection)
        {
            foreach (var p in collection.AllKeys.Where(p => p.Contains("SKU")))
            {
                string[] formValues = Request.Form.GetValues(p);
                int sum = 0;
                for (int i = 0; i < formValues.Length; i++)
                {
                    try
                    {
                        sum += int.Parse(formValues[i]);
                    }
                    catch (Exception e)
                    {
                    }
                    
                }
               
                TempData[p] = sum;

            }
            
            Orders orderInBag = _orderRepository.GetOrderInBag();
            // Add the Marketing SKUs.  The other items are added on the next page.
            if (!string.IsNullOrEmpty(orderInBag.AccountNumber))
            {
                foreach (var m in collection.AllKeys.Where(p => p.Contains("MKT-")))
                {
                    string[] formValues = Request.Form.GetValues(m);
                    int sum = 0;
                    for (int i = 0; i < formValues.Length; i++)
                    {
                        try
                        {
                            sum += int.Parse(formValues[i]);
                        }
                        catch (Exception e)
                        {
                        }
                    }

                    TempData[m] = sum;

                    if (sum > 0)
                    {
                        _orderRepository.AddToOrder(orderInBag, new OrderItems(orderInBag.OrderID, m.Replace("MKT-", ""), sum, BrandCode.SBUX.Code, true));
                    }

                }
            }

            foreach (var t in collection.AllKeys.Where(p => p.Contains("nextPage")))
            {
                var RedirectValue = collection[t];
                return new RedirectResult(RedirectValue);
            }
            
            return Redirect("/dashboard");
        }
    }
}
