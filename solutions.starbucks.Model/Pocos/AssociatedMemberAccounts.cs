using Umbraco.Core.Persistence;

namespace solutions.starbucks.Model.Pocos
{
    [TableName("AssociatedMemberAccounts")]
    [PrimaryKey("AccountId")]
    [ExplicitColumns]
    public class AssociatedMemberAccount
    {
        [Column("ID")]
        public int AccountID { get; set; }

        [Column("UmbracoUserID")]
        public int UmbracoUserID { get; set; }

        [Column("AccountNumber")]
        public string AccountNumber { get; set; }

        [Column("Zip")]
        public string Zip { get; set; }
        
        public string AccountName { get; set; }
    }
}