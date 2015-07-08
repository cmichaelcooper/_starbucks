using solutions.starbucks.Interfaces;
using solutions.starbucks.Model.Enums;
using solutions.starbucks.Model.Pocos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace solutions.starbucks.web.Controllers
{
    public class BagSurfaceController : SurfaceController
    {
        private readonly IMemberAttributesRepository _memberRepository;
        private readonly IOrdersRepository _ordersRepository;

        public string UmbracoDictionaryItem { get; set; }

        public BagSurfaceController(IMemberAttributesRepository memberRepository, IOrdersRepository ordersRepository)
        {
            _ordersRepository = ordersRepository;
            _memberRepository = memberRepository;
        }

        private Orders GetOrderInBag(bool overwrite = false)
        {
            if (Session["CurrentAccount"] != null)
            {
                if (Session["OrderInBag"] == null || overwrite)
                {
                    Session["OrderInBag"] = _ordersRepository.GetOrderInBag();
                }
            }
            return (Orders)Session["OrderInBag"];
        }

        //[Authorize]
        [HttpPost]
        public ActionResult QuickAdd(string sku, int qty, string siteId)
        {
            Orders orderInBag = _ordersRepository.GetOrderInBag();
            _ordersRepository.AddToOrder(orderInBag, new OrderItems(orderInBag.OrderID, sku, qty, siteId));

            return Json(new { success = true, message = "" });
        }

        [HttpPost]
        public ActionResult AddItemsToBag(FormCollection formCollection)
        {
            Orders orderInBag = _ordersRepository.GetOrderInBag();
            List<OrderItems> newOrderItems = new List<OrderItems>();

            string userBrand = BrandCode.SBUX.Code;
            if (Request.Cookies["AccountAttributes"]["CustomerBrand"] != null)
            {
                userBrand = Request.Cookies["AccountAttributes"]["CustomerBrand"];
            }

            foreach (var qtyField in formCollection.AllKeys)
            {
                if (!qtyField.ToString().ToLower().Contains("siteid")) 
                {
                    string Sku = qtyField.ToString().Replace("SKU-", "").Replace("MKT-", "");
                    string Qty = Request.Form.GetValues(qtyField).ToList().FirstOrDefault();
                    bool IsMarketing = qtyField.Contains("MKT-");

                    if (!string.IsNullOrEmpty(Qty) && (Convert.ToInt16(Qty) > 0))
                    {
                        //If they're a dual user, check to see if this is SBC only
                        string siteId = BrandCode.SBUX.Code;
                        if (userBrand == BrandCode.DUAL.Code)
                        {
                            var siteIdVal = Request.Form.GetValues(qtyField + "-SiteId");
                            if (siteIdVal != null)
                            {
                                siteId = Request.Form.GetValues(qtyField + "-SiteId").ToList().FirstOrDefault();
                            }
                        }

                        newOrderItems.Add(new OrderItems(orderInBag.OrderID, Sku, Convert.ToInt16(Qty), siteId, IsMarketing));
                    }
                }
            }

            string redirect = "";
            if (newOrderItems.Count > 0)
            {
                newOrderItems.ForEach(i => _ordersRepository.AddToOrder(orderInBag, i));
                redirect = "/my-bag";
            }

            return Json(new { success = true, message = "", redirect = redirect });
        }

        [HttpDelete]
        public ActionResult DeleteOrderItem(Guid orderID, string sku)
        {
            _ordersRepository.DeleteOrderItem(new OrderItems(orderID, sku, 0, ""));
            
            return Json(new { success = true, message = "" });

        }

        [HttpPost]
        public ActionResult SaveOrder(Orders order)
        {
            Orders existingBagOrder = (Orders)Session["OrderInBag"];
            if (existingBagOrder == null)
            {
                existingBagOrder = _ordersRepository.GetOrder(order.OrderID);
            }

            if (existingBagOrder.OrderStatus == OrderStatusType.NotCheckedOut && order.OrderID == existingBagOrder.OrderID)
            {
                foreach (OrderItems item in order.OrderItems)
                {
                    OrderItems existingItem = existingBagOrder.FindOrderItem(item.SKUNumber);
                    if (item.Quantity > 0)
                    {
                        existingItem.Quantity = item.Quantity;
                        _ordersRepository.UpdateOrderItem(existingItem);
                    }
                    else
                    {
                        _ordersRepository.DeleteOrderItem(existingItem);
                    }
                }

                //Set Customer Attributes on order
                existingBagOrder.AccountSiteNumber = order.AccountSiteNumber;
                existingBagOrder.PurchaseOrderNumber = order.PurchaseOrderNumber;

                _ordersRepository.SaveOrder(existingBagOrder);
            }
            
            return Json(new { success = true, message = "" });
        }

        [HttpPost]
        public ActionResult SubmitOrder(Orders order)
        {
            Orders existingBagOrder = (Orders)Session["OrderInBag"];
            if (existingBagOrder == null)
            {
                existingBagOrder = _ordersRepository.GetOrder(order.OrderID);
            }

            if (existingBagOrder.OrderStatus == OrderStatusType.NotCheckedOut && order.OrderID == existingBagOrder.OrderID)
            {
                foreach (OrderItems item in order.OrderItems)
                {
                    OrderItems existingItem = existingBagOrder.FindOrderItem(item.SKUNumber);
                    if (item.Quantity > 0)
                    {
                        existingItem.Quantity = item.Quantity;
                        _ordersRepository.UpdateOrderItem(existingItem);
                    }
                    else
                    {
                        _ordersRepository.DeleteOrderItem(existingItem);
                    }
                }

                //Set Customer Attributes on order
                existingBagOrder.AccountSiteNumber = order.AccountSiteNumber;
                CustomerAttributes siteAttributes = _memberRepository.GetCustomerAttributes(existingBagOrder.AccountNumber).Where(s => s.AccountSiteNumber == existingBagOrder.AccountSiteNumber).FirstOrDefault();

                var profileModel = Members.GetCurrentMemberProfileModel();
                var currentMember = Services.MemberService.GetByEmail(profileModel.Email);

                if (!currentMember.Email.Contains("@starbucks.com"))
                {
                    existingBagOrder.OperationName = currentMember.Properties["companyName"].Value.ToString();
                    existingBagOrder.ShipToCity = currentMember.Properties["city"].Value.ToString();
                    existingBagOrder.ShipToState = currentMember.Properties["state"].Value.ToString();
                }

                if (siteAttributes != null)
                {
                    existingBagOrder.ShipToPostalCode = (siteAttributes.PostalCode ?? "");
                    existingBagOrder.ShipToAddress1 = (siteAttributes.Address1 ?? "");
                    existingBagOrder.ShipToAddress2 = (siteAttributes.Address2 ?? "");
                    existingBagOrder.Telephone = (siteAttributes.Phone ?? "");
                    existingBagOrder.EmailAddress = currentMember.Email;
                    existingBagOrder.CustomerName = currentMember.Name;

                }

                //Finalize order
                existingBagOrder.OrderStatus = Model.Enums.OrderStatusType.Completed;
                existingBagOrder.DateOrdered = DateTime.Now;
                existingBagOrder.OrderNumber = _ordersRepository.GetNextOrderNumber();
                existingBagOrder.DateOrderedSequence = _ordersRepository.GetNextDailyOrderSequence();
                existingBagOrder.PurchaseOrderNumber = order.PurchaseOrderNumber;

                _ordersRepository.SaveOrder(existingBagOrder);
                Session["OrderInBag"] = null;

                return Json(new { success = true, message = "", Order = existingBagOrder });
            }
            else
            {
                return Json(new { success = false, message = "Order previously submitted." });
            }
        }

    }
}
