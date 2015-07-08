using solutions.starbucks.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Persistence;

namespace solutions.starbucks.Model.Pocos
{
    [TableName("Orders")]
    [PrimaryKey("OrderID")]
    [ExplicitColumns]
    public class Orders
    {

        #region Constructors
        public Orders()
        {
            this.OrderID = Guid.NewGuid();
            this.DateCreated = DateTime.Now;
            this.OrderStatus = OrderStatusType.NotCheckedOut;
            this.OrderItems = new List<OrderItems>();
        }

        public Orders(string accountNumber)
        {
            this.OrderID = Guid.NewGuid();
            this.DateCreated = DateTime.Now;
            this.OrderStatus = OrderStatusType.NotCheckedOut;
            this.OrderItems = new List<OrderItems>();

            this.AccountNumber = accountNumber;
        }
        #endregion

        #region Properties

        [Column("OrderID")]
        public Guid OrderID { get; set; }

        [Column("DateCreated")]
        public DateTime DateCreated { get; set; }

        [Column("OperationName")]
        public string OperationName { get; set; }

        [Column("OrderNumber")]
        public int OrderNumber { get; set; }

        [Column("CustomerName")]
        public string CustomerName { get; set; }

        [Column("DateOrdered")]
        public DateTime DateOrdered { get; set; }

        [Column("DateOrderedSequence")]
        public int DateOrderedSequence { get; set; }

        [Column("AccountNumber")]
        public string AccountNumber { get; set; }

        [Column("EmailAddress")]
        public string EmailAddress { get; set; }

        [Column("Telephone")]
        public string Telephone { get; set; }

        [Column("PurchaseOrderNumber")]
        public string PurchaseOrderNumber { get; set; }

        [Column("OrderStatus")]
        public OrderStatusType OrderStatus { get; set; }

        [Column("DateExported")]
        public DateTime DateExported { get; set; }

        [Column("Notes")]
        public string Notes { get; set; }

        [Column("ShipToPostalCode")]
        public string ShipToPostalCode { get; set; }

        [Column("ShipToState")]
        public string ShipToState { get; set; }

        [Column("ShipToCity")]
        public string ShipToCity { get; set; }

        [Column("ShipToAddress2")]
        public string ShipToAddress2 { get; set; }

        [Column("ShipToAddress1")]
        public string ShipToAddress1 { get; set; }

        [Column("Attn")]
        public string Attn { get; set; }

        [Column("Marketing")]
        public string Marketing { get; set; }

        [Column("AccountSiteNumber")]
        public string AccountSiteNumber { get; set; }

        [Column("OrderItems")]
        public List<OrderItems> OrderItems { get; set; }

        [Column("OrderNotes")]
        public List<OrderNotes> OrderNotes { get; set; }

        #endregion Properties

        #region Methods
        public OrderItems FindOrderItem(string skuNumber)
        {
            if (this.OrderItems != null)
            {
                return (from si in this.OrderItems
                        where si.SKUNumber == skuNumber
                        select si).SingleOrDefault();
            }
            return null;

        }
        #endregion Methods

    }
}