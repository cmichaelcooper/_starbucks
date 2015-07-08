using Umbraco.Core.Persistence;

namespace solutions.starbucks.Model.Pocos
{
    [TableName("SymmetricCryptoKeys")]
    [ExplicitColumns]
    public class SymmetricCryptoKeys
    {
        [Column("Bucket")]
        public string Bucket { get; set; }

        [Column("Handle")]
        public string Handle { get; set; }

        [Column("Key")]
        public byte[] Key { get; set; }

        [Column("ExpiresUTC")]
        public System.DateTime ExpiresUTC { get; set; }

        [Column("Ticks")]
        public long Ticks { get; set; }

        [Column("DataLength")]
        public int DataLength { get; set; }
    }
}