using System;
using Umbraco.Core.Persistence;

namespace solutions.starbucks.Model.Pocos
{
    [TableName("vw_DictionaryItems")]
    [PrimaryKey("LanguagePK")]
    [ExplicitColumns]
    public class DictionaryItems
    {
        [Column("LanguagePK")]
        public int LanguagePK { get; set; }

        [Column("LanguageID")]
        public int LanguageID { get; set; }

        [Column("DictionaryId")]
        public Guid DictionaryId { get; set; }

        [Column("ISOCode")]
        public string ISOCode { get; set; }

        [Column("Culture")]
        public string Culture { get; set; }

        [Column("Value")]
        public string Value { get; set; }

    }
}