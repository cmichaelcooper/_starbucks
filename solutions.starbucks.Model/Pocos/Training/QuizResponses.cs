using System;
using Umbraco.Core.Persistence;

namespace solutions.starbucks.Model.Pocos
{
    [TableName("QuizResponses")]
    [PrimaryKey("QuizResponseID")]
    [ExplicitColumns]
    public class QuizResponses
    {
        [Column("QuizResponseID")]
        public int QuizResponseID { get; set; }

        [Column("InviteSubjectID")]
        public int InviteSubjectID { get; set; }

        [Column("QuestionID")]
        public int QuestionID { get; set; }

        [Column("Value")]
        public string Value { get; set; }

        [Column("DateCreated")]
        public DateTime DateCreated { get; set; }

        [Column("IsDeleted")]
        public bool IsDeleted { get; set; }

    }
}