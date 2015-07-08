using solutions.starbucks.Interfaces;
using solutions.starbucks.Model.Enums;
using solutions.starbucks.Model.Pocos;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace solutions.starbucks.Repository.PetaPoco
{
    public class OrdersRepository : IOrdersRepository
    {
        protected UmbracoDatabase _db = ApplicationContext.Current.DatabaseContext.Database;
        private readonly IMemberAttributesRepository _memberRepository = new MemberAttributesRepository();

        public IEnumerable<Orders> GetOrdersByAccount(string accountNumber)
        {
            var queryText = Sql.Builder.Append("SELECT * FROM vw_Orders WHERE AccountNumber=@0", accountNumber);

            return _db.Query<Orders>(queryText);
        }

        public IEnumerable<Orders> GetOrdersByCustomer(string accountNumber, string zip)
        {
            var queryText = Sql.Builder.Append("SELECT * FROM vw_Orders WHERE AccountNumber=@0", accountNumber)
                .Append(" AND ShipToPostalCode LIKE @0", "%" + zip + "%");
            return _db.Query<Orders>(queryText);

        }

        public Orders GetOrderInBag()
        {
            Orders orderInCart = new Orders();
            string accountNumber = (HttpContext.Current.Session["CurrentAccount"] == null ? "" : HttpContext.Current.Session["CurrentAccount"].ToString());
            if ((accountNumber ?? "") == "")
            {
                accountNumber = (HttpContext.Current.Request.Cookies["AccountAttributes"]["CurrentAccount"] == null ? "" : HttpContext.Current.Request.Cookies["AccountAttributes"]["CurrentAccount"].ToString());
            }
            if ((accountNumber ?? "") != "")
            {
                var queryText = Sql.Builder
                    .Append("SELECT TOP 1 * ")
                    .Append("FROM Orders WITH (NOLOCK) ")
                    .Append("WHERE AccountNumber=@0", accountNumber)
                    .Append("   AND OrderStatus = 1 ")
                    .Append("ORDER BY DateCreated DESC ");

                orderInCart = _db.SingleOrDefault<Orders>(queryText);

                //Nothing in cart, better make this happen
                if (orderInCart == null)
                {
                    Guid newOrderId = Guid.NewGuid();
                    orderInCart = new Orders();
                    orderInCart.OrderID = newOrderId;
                    orderInCart.AccountNumber = accountNumber;
                    InsertOrder(orderInCart);
                    orderInCart = GetOrder(newOrderId);
                }

                if (orderInCart != null && orderInCart.OrderID != null)
                {
                    orderInCart.OrderItems = GetOrderDetails(orderInCart.OrderID);
                }
            }
            HttpContext.Current.Session["OrderInBag"] = orderInCart;
            return orderInCart;
        }

        private bool IsItemInCart(Guid orderNumber, string skuNumber)
        {
            bool itemInCart = GetOrderDetails(orderNumber).Where(p => p.SKUNumber == skuNumber).Any();

            return itemInCart;
        }

        public void SaveOrder(Orders order)
        {
            //see if there is an order in the DB already
            Orders existingOrder = GetOrder(order.OrderID);

            if (existingOrder == null || existingOrder.OrderID == null)
            {
                InsertOrder(order);
            }
            else
            {
                UpdateOrder(order);
            }
        }

        public void AddToOrder(Orders order, OrderItems orderItemToAdd)
        {
            if (order.OrderStatus == OrderStatusType.NotCheckedOut)
            {
                if (orderItemToAdd.Quantity > 0)
                {
                    //see if this item is in the cart already
                    OrderItems existingOrderItem = order.FindOrderItem(orderItemToAdd.SKUNumber);
                    if (existingOrderItem != null)
                    {
                        existingOrderItem.Quantity += orderItemToAdd.Quantity;
                        UpdateOrderItem(existingOrderItem);
                    }
                    else
                    {
                        InsertOrderItem(orderItemToAdd);
                    }

                }
            }
        }

        public bool ExportDailyOrderReport(string templatePath, string outputPath, string marketingTemplatePath, string marketingOutputPath)
        {
            List<Orders> orders = GetUnexportedOrders();
            List<Orders> marketing = GetUnexportedOrders(true);
            if (marketing.Count() > 0)
            {
                ExportMarketingToExcel(marketingTemplatePath, marketingOutputPath, marketing);
            }
            ExportOrderListToExcel(templatePath, outputPath, orders);

            DateTime dateExported = DateTime.Now;
            MarkOrdersExported(orders, dateExported);
            MarkOrdersExported(marketing, dateExported);

            return true;
        }


        private List<Orders> GetUnexportedOrders(bool getMarketing = false)
        {
            //Get Orders
            var queryText = Sql.Builder
                .Append("SELECT orders.* FROM vw_Orders orders ")
                .Append("WHERE DateExported IS NULL AND OrderStatus = @0", OrderStatusType.Completed)
                .Append("   AND EXISTS ( SELECT TOP 1 1  FROM vw_OrderItems orderItems WHERE orderItems.OrderId = orders.OrderId AND orderItems.IsMarketing = @0 ) ", getMarketing);
            List<Orders> orders = _db.Fetch<Orders>(queryText);

            //Get OrderItems
            queryText = Sql.Builder
                .Append("SELECT orderItems.* FROM vw_OrderItems orderItems ")
                .Append("INNER JOIN vw_Orders orders ON orders.OrderID = orderItems.OrderID ")
                .Append("   AND orders.DateExported IS NULL AND orders.OrderStatus = @0", OrderStatusType.Completed)
                .Append("WHERE orderItems.IsMarketing = @0 ", getMarketing);

            if (getMarketing)
            {
                queryText.Append(" AND orders.DateOrdered > @0 ", DateTime.Now.AddDays(-1));
            }
            
            List<OrderItems> orderItems = _db.Fetch<OrderItems>(queryText);

            orders.ForEach(o => o.OrderItems = orderItems.Where(oi => oi.OrderID == o.OrderID).ToList());

            return orders;
        }

        public bool ExportOrderListToExcel(string templatePath, string outputPath, IList<Orders> orders)
        {
            //New instance of XlsIO is created.[Equivalent to launching MS Excel with no workbooks open].
            //The instantiation process consists of two steps.

            //Step 1 : Instantiate the spreadsheet creation engine.
            ExcelEngine excelEngine = new ExcelEngine();
            //using (ExcelEngine excelEngine = new ExcelEngine())
            //{
            //Step 2 : Instantiate the excel application object.
            IApplication application = excelEngine.Excel;

            // Creating new workbook

            IWorkbook workbook = application.Workbooks.Open(templatePath);
            IWorksheet header = workbook.Worksheets[0];
            header.Name = "Header";
            IWorksheet lines = workbook.Worksheets[1];
            lines.Name = "Lines";

            int lineItemPosition = 3;
            orders = orders.OrderBy(p => p.DateOrdered).ToList();

            if (orders != null)
            {
                for (var i = 0; i < orders.Count; i++)
                {

                    string u = (i + 4).ToString();
                    header.Range["A" + u].Text = orders[i].OrderNumber == null ? "N/A" : orders[i].OrderNumber.ToString();
                    header.Range["B" + u].Text = orders[i].AccountNumber ?? "";
                    header.Range["C" + u].Text = orders[i].PurchaseOrderNumber ?? "";
                    header.Range["D" + u].Text = orders[i].Notes ?? "";
                    header.Range["E" + u].Text = orders[i].ShipToAddress1 ?? "";
                    header.Range["F" + u].Text = "Y";
                    header.Range["G" + u].Text = orders[i].EmailAddress ?? "";
                    header.Range["H" + u].Text = orders[i].Telephone ?? "";

                    if (orders[i].OrderNotes != null)
                    {
                        IList<OrderNotes> notes = orders[i].OrderNotes.ToList<OrderNotes>();
                        if (notes != null)
                        {
                            header.Range["I" + u].Text = "";
                            foreach (OrderNotes note in notes.OrderByDescending(p => p.DateAdded))
                            {
                                header.Range["I" + u].Text += note.AddedBy + " - " + note.DateAdded.ToString() + "\r\n" + note.Details + "\r\n\r\n";
                            }

                            header.Range["I" + u].WrapText = true;
                            header.Range["I" + u].ColumnWidth = 100D;

                        }
                    }

                    List<OrderItems> items = orders[i].OrderItems.ToList<OrderItems>();
                    for (var j = 0; j < items.Count; j++)
                    {
                        lines.Range["A" + lineItemPosition.ToString()].Text = orders[i].OrderNumber == null ? "N/A" : orders[i].OrderNumber.ToString();
                        lines.Range["B" + lineItemPosition.ToString()].Text = items[j].SKUNumber;
                        lines.Range["C" + lineItemPosition.ToString()].Text = items[j].SiteId;
                        try
                        {
                            lines.Range["D" + lineItemPosition.ToString()].Text =
                                    items[j].Name + " - " +
                                    items[j].ShortDescription;
                        }
                        catch (Exception exp) { lines.Range["D" + lineItemPosition.ToString()].Text = "Error Retrieving Product Information"; }

                        lines.Range["E" + lineItemPosition.ToString()].Text = items[j].Quantity.ToString();
                        try
                        {
                            lines.Range["F" + lineItemPosition.ToString()].Text = items[j].UOM;
                        }
                        catch (Exception exp) { }

                        lineItemPosition++;
                    }
                }
            }

            header.Columns[4].WrapText = true;
            header.Columns[5].WrapText = true;
            foreach (IRange r in header.Columns)
            {
                r.VerticalAlignment = ExcelVAlign.VAlignTop;
            }


            string savefile = outputPath;
            FileStream fs = new FileStream(savefile, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            workbook.SaveAs(fs);
            fs.Close();
            workbook.Close();
            excelEngine.Dispose();

            return true;

        }

        public bool ExportMarketingToExcel(string templatePath, string outputPath, IList<Orders> orders)
        {
            DateTime dateExported = DateTime.Now;
            ExcelEngine excelEngine = new ExcelEngine();
            IApplication application = excelEngine.Excel;

            // Creating new workbook
            IWorkbook workbook = application.Workbooks.Open(templatePath);
            IWorksheet lines = workbook.Worksheets[0];
            lines.Name = "Lines";

            int lineItemPosition = 3;
            
            orders = orders.OrderBy(p => p.DateCreated).ToList();
            if (orders.Count() > 0)
            {
                for (var i = 0; i < orders.Count; i++)
                {

                    string u = (i + 4).ToString();

                    var marketing = orders[i].Marketing ?? "";
                    List<OrderItems> items = orders[i].OrderItems.ToList<OrderItems>();
                    for (var k = 0; k < items.Count; k++)
                    {
                        lines.Range["A" + lineItemPosition.ToString()].Text = orders[i].OrderNumber == null ? "N/A" : orders[i].OrderNumber.ToString();
                        lines.Range["B" + lineItemPosition.ToString()].Text = orders[i].CustomerName == null ? "N/A" : orders[i].CustomerName.ToString();
                        lines.Range["C" + lineItemPosition.ToString()].Text = orders[i].EmailAddress == null ? "N/A" : orders[i].EmailAddress.ToString();
                        lines.Range["D" + lineItemPosition.ToString()].Text = orders[i].Telephone == null ? "N/A" : orders[i].Telephone.ToString();
                        lines.Range["E" + lineItemPosition.ToString()].Text = orders[i].AccountNumber == null ? "N/A" : orders[i].AccountNumber.ToString();
                        lines.Range["F" + lineItemPosition.ToString()].Text = orders[i].OperationName == null ? "N/A" : orders[i].OperationName.ToString();
                        lines.Range["G" + lineItemPosition.ToString()].Text = orders[i].ShipToAddress1 == null ? "N/A" : orders[i].ShipToAddress1.ToString();
                        lines.Range["H" + lineItemPosition.ToString()].Text = orders[i].ShipToAddress2 == null ? "N/A" : orders[i].ShipToAddress2.ToString();

                        string[] cityStateZip = new string[] { orders[i].ShipToCity ?? "", orders[i].ShipToState ?? "", orders[i].ShipToPostalCode ?? "" };
                        string cityStateZipCSV = string.Join(",", cityStateZip);
                        lines.Range["I" + lineItemPosition.ToString()].Text = cityStateZipCSV == null ? "N/A" : cityStateZipCSV;

                        lines.Range["J" + lineItemPosition.ToString()].Text = items[k].SKUNumber.ToString();
                        lines.Range["K" + lineItemPosition.ToString()].Text = items[k].Name == null ? "N/A" : items[k].Name;
                        lines.Range["L" + lineItemPosition.ToString()].Text = items[k].Quantity.ToString();

                        lineItemPosition++;
                    }

                }

                lines.Columns[4].WrapText = true;
                lines.Columns[5].WrapText = true;
                foreach (IRange r in lines.Columns)
                {
                    r.VerticalAlignment = ExcelVAlign.VAlignTop;
                }

                string savefile = outputPath;
                FileStream fs = new FileStream(savefile, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                workbook.SaveAs(fs);
                fs.Close();
                workbook.Close();
                excelEngine.Dispose();

                return true;
            }

            return false;
        }

        public void MarkOrdersExported(IList<Orders> orders, DateTime dateExported) 
        {
            if (orders.Count > 0)
            {
                List<string> orderIds = new List<string>();
                orders.ForEach(o => orderIds.Add(o.OrderID.ToString()));

                var sql = Sql.Builder
                    .Append("UPDATE Orders ")
                    .Append("SET DateExported = @0 ", dateExported)
                    .Append("WHERE OrderId IN ('" + string.Join("','", orderIds) + "')");

                _db.Execute(sql);
            }
        }

        public Orders GetOrder(Guid orderID)
        {
            Orders order = _db.SingleOrDefault<Orders>(Sql.Builder.Append("SELECT * FROM Orders WHERE OrderId=@0", orderID));
            order.OrderItems = GetOrderDetails(order.OrderID);
            return order;
        }

        private List<OrderItems> GetOrderDetails(Guid orderID)
        {
            var queryText = Sql.Builder
                .Append("SELECT * ")
                .Append("FROM vw_OrderItems WITH (NOLOCK) ")
                .Append("WHERE OrderId=@0", orderID);

            return _db.Fetch<OrderItems>(queryText);

        }

        private void InsertOrder(Orders order)
        {

            var sql = Sql.Builder
                .Append("IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[Orders] WHERE OrderID = @0) ", order.OrderID)
                .Append("BEGIN ")
                .Append("   INSERT INTO [Orders] (OrderID,DateCreated,DateOrdered,DateOrderedSequence,AccountNumber,OperationName,ShipToAddress1,ShipToAddress2,ShipToCity,ShipToState, ")
                .Append("       ShipToPostalCode,EmailAddress,Telephone,PurchaseOrderNumber,OrderStatus,OrderNumber,Notes,DateExported,Attn,CustomerName,Marketing,AccountSiteNumber) ")
                .Append("   VALUES (")
                .Append("       @0, ", order.OrderID)
                .Append("       GETDATE(), ")
                .Append("       @0, ", order.DateOrdered)
                .Append("       @0, ", order.DateOrderedSequence)
                .Append("       @0, ", order.AccountNumber)
                .Append("       @0, ", order.OperationName)
                .Append("       @0, ", order.ShipToAddress1)
                .Append("       @0, ", order.ShipToAddress2)
                .Append("       @0, ", order.ShipToCity)
                .Append("       @0, ", order.ShipToState)
                .Append("       @0, ", order.ShipToPostalCode)
                .Append("       @0, ", order.EmailAddress)
                .Append("       @0, ", order.Telephone)
                .Append("       @0, ", order.PurchaseOrderNumber)
                .Append("       @0, ", order.OrderStatus)
                .Append("       @0, ", order.OrderNumber)
                .Append("       @0, ", order.Notes)
                .Append("       @0, ", order.DateExported)
                .Append("       @0, ", order.Attn)
                .Append("       @0, ", order.CustomerName)
                .Append("       @0, ", order.Marketing)
                .Append("       @0) ", order.AccountSiteNumber)
                .Append("END ");

            _db.Execute(sql);
        }



        public void UpdateOrder(Orders order)
        {
            var sql = Sql.Builder
                .Append("UPDATE Orders ")
                .Append("SET DateOrdered = @0,", order.DateOrdered)
                .Append("   DateOrderedSequence = @0,", order.DateOrderedSequence)
                .Append("   AccountNumber = @0,", order.AccountNumber)
                .Append("   OperationName = @0,", order.OperationName)
                .Append("   ShipToAddress1 = @0,", order.ShipToAddress1)
                .Append("   ShipToAddress2 = @0,", order.ShipToAddress2)
                .Append("   ShipToCity = @0,", order.ShipToCity)
                .Append("   ShipToState = @0,", order.ShipToState)
                .Append("   ShipToPostalCode = @0,", order.ShipToPostalCode)
                .Append("   EmailAddress = @0,", order.EmailAddress)
                .Append("   Telephone = @0,", order.Telephone)
                .Append("   PurchaseOrderNumber = @0,", order.PurchaseOrderNumber)
                .Append("   OrderStatus = @0,", order.OrderStatus)
                .Append("   OrderNumber = @0,", order.OrderNumber)
                .Append("   Notes = @0,", order.Notes)
                .Append("   DateExported = @0,", order.DateExported)
                .Append("   Attn = @0,", order.Attn)
                .Append("   CustomerName = @0,", order.CustomerName)
                .Append("   Marketing = @0,", order.Marketing)
                .Append("   AccountSiteNumber = @0", order.AccountSiteNumber)
                .Append("WHERE OrderId = @0 ", order.OrderID);
            _db.Execute(sql);
        }

        private void DeleteOrder(Orders order)
        {
            var sql = Sql.Builder.Append(";EXEC [dbo].[sp_Delete_Order] @@orderID = @0 ", order.OrderID);
            _db.Execute(sql);
        }


        private void InsertOrderItem(OrderItems orderItem)
        {
            var sql = Sql.Builder
                .Append(";EXEC dbo.[sp_Insert_OrderItem] ")
                .Append("   @@orderID = @0, ", orderItem.OrderID)
                .Append("   @@skuNumber = @0,", orderItem.SKUNumber)
                .Append("   @@quantity = @0,", orderItem.Quantity)
                .Append("   @@ismarketingitem = @0,", orderItem.IsMarketing)
                .Append("   @@siteId = @0 ", orderItem.SiteId.Replace("SBUX", "SBX"));

            _db.Execute(sql);
        }

        public void UpdateOrderItem(OrderItems orderItem)
        {
            var sql = Sql.Builder
                .Append("UPDATE OrderItems ")
                .Append("SET Quantity = @0,", orderItem.Quantity)
                .Append("   IsMarketing = @0 ", orderItem.IsMarketing)
                .Append("WHERE OrderId = @0 ", orderItem.OrderID)
                .Append("   AND SkuNumber = @0 ", orderItem.SKUNumber);

            _db.Execute(sql);
        }

        public void DeleteOrderItem(OrderItems orderItem)
        {
            var sql = Sql.Builder
                .Append("DELETE OrderItems  ")
                .Append("WHERE OrderId = @0 ", orderItem.OrderID)
                .Append("   AND SkuNumber = @0 ", orderItem.SKUNumber);
            _db.Execute(sql);
        }

        public int GetNextDailyOrderSequence()
        {
            var sql = Sql.Builder.Append("SELECT dbo.fn_GetNextOrderSequence()  ");
            return _db.FirstOrDefault<int>(sql);
        }

        public int GetNextOrderNumber()
        {
            var sql = Sql.Builder.Append("SELECT dbo.fn_GetNextOrderNumber()  ");
            return _db.FirstOrDefault<int>(sql);
        }

        public int TotalItemsInCart()
        {
            int retVal = 0;
            string acctNum = (HttpContext.Current.Session["CurrentAccount"] == null ? "" : HttpContext.Current.Session["CurrentAccount"].ToString());
            if (!string.IsNullOrEmpty(acctNum))
            {
                retVal = GetOrderInBag().OrderItems.Count();
            }
            return retVal;

        }

    }
}