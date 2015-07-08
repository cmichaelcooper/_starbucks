using Umbraco.Core.Persistence;

namespace solutions.starbucks.Model.Pocos
{
    [TableName("PartnerAdmin")]
    [PrimaryKey("ID")]
    [ExplicitColumns]
    public class PartnerAdmin
    {
        [Column("ID")]
        public int ID { get; set; }

        [Column("PartnerName")]
        public string PartnerName { get; set; }

        [Column("PartnerEmail")]
        public string AccountNumber { get; set; }

    }

    [TableName("SuperPartnerAdmin")]
    [PrimaryKey("ID")]
    [ExplicitColumns]
    public class SuperPartnerAdmin
    {
        [Column("ID")]
        public int ID { get; set; }

        [Column("PartnerName")]
        public string PartnerName { get; set; }

        [Column("PartnerEmail")]
        public string AccountNumber { get; set; }

    }
}