using System;
using Umbraco.Core.Persistence;

namespace solutions.starbucks.Model.Pocos.Training
{
    [TableName("AccessLog")]
    [PrimaryKey("AccessLogID")]
    [ExplicitColumns]
    public class AccessLog
    {

        public enum Access_Type
        {
            Landing = 1,
            Subject = 2
        }

        [Column("AccessType")]
        public Access_Type AccessType { get; set; }

        [Column("AccessLogID")]
        public int AccessLogID { get; set; }

        [Column("InviteID")]
        public int InviteID { get; set; }

        [Column("InviteSubjectID")]
        public int? InviteSubjectID { get; set; }

        [Column("DateAccessed")]
        public DateTime DateAccessed { get; set; }



    }
}