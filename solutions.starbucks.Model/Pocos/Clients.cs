using System;
using Umbraco.Core.Persistence;

namespace solutions.starbucks.Model.Pocos
{
    [TableName("Clients")]
    [PrimaryKey("ClientID")]
    [ExplicitColumns]
    public class Clients
    {
        [Column("ClientID")]
        public Guid ClientId { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("ClientSecret")]
        public string ClientSecret { get; set; }

        [Column("DefaultCallback")]
        public string DefaultCallback { get; set; }

        [Column("ClientType")]
        public string ClientType { get; set; }


    }
    [TableName("ClientAuthorizations")]
    [PrimaryKey("ClientId", autoIncrement = false)]
    [ExplicitColumns]
    public class ClientAuthorizations
    {
        [Column("ClientId")]
        public Guid ClientId { get; set; }

        [Column("UserId")]
        public string UserId { get; set; }

        [Column("IssueDate")]
        public DateTime IssueDate { get; set; }

        [Column("ExpirationDateUtc")]
        public DateTime? ExpirationDateUtc { get; set; }


    }

    [TableName("ClientAuthorizationScopes")]
    [PrimaryKey("ClientId", autoIncrement = false)]
    [ExplicitColumns]
    public class ClientAuthorizationScopes
    {
        [Column("ClientId")]
        public Guid ClientId { get; set; }

        [Column("UserId")]
        public string UserId { get; set; }

        [Column("Scope")]
        public string Scope { get; set; }

       

    }

}