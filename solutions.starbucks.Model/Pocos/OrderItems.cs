using solutions.starbucks.Model.Enums;
using System;
using Umbraco.Core.Persistence;

namespace solutions.starbucks.Model.Pocos
{
    [TableName("OrderItems")]
    [PrimaryKey("OrderID")]
    [ExplicitColumns]
    public class OrderItems
    {

        #region Constructors
        public OrderItems()
        {
        }

        public OrderItems(Guid OrderID, string sku, int quantity, string siteId, bool isMarketing = false)
        {
            this.OrderID = OrderID;
            this.SKUNumber = sku;
            this.Quantity = quantity;
            this.IsMarketing = isMarketing;
            this.SiteId = siteId;
        }
        #endregion

        [Column("OrderID")]
        public Guid OrderID { get; set; }

        [Column("SKUNumber")]
        public string SKUNumber { get; set; }

        [Column("Quantity")]
        public int Quantity { get; set; }

        [Column("DateAdded")]
        public DateTime DateAdded { get; set; }

        [Column("IsMarketing")]
        public bool IsMarketing { get; set; }

        [Column("SiteId")]
        public string SiteId { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("Description")]
        public string Description { get; set; }

        [Column("UOM")]
        public string UOM { get; set; }

        [Column("ShortDescription")]
        public string ShortDescription { get; set; }

        [Column("ProductID")]
        public string ProductID { get; set; }

        [Column("Size")]
        public string Size { get; set; }

        [Column("DisplayName")]
        public string DisplayName { get; set; }

        public BrandCode BrandCode 
        {
            get
            {
                BrandCode returnVal = BrandCode.SBUX;
                if (this.SiteId.ToUpper() == BrandCode.SBC.Code) 
                {
                    returnVal = BrandCode.SBC;
                }
                return returnVal;
            }
        }

        public string ProductURL 
        {
            get
            {
                return "/Products/Details/" + this.ProductID;
            }
        }

        public string ThumbImgURL
        {
            get
            {
                if (IsMarketing)
                {
                    return "/img/marketing_generic_icon_165x165.png";
                }
                else
                {
                    return "/img/Products/200/" + this.ProductID + ".jpg";

                }
            }
        }

        public string FullSizeImgURL
        {
            get
            {
                return "/img/Products/640/" + this.ProductID + ".jpg";
            }
        }

    }
}