using Umbraco.Core.Persistence;

namespace solutions.starbucks.Model.Pocos
{
    [TableName("CustomerAttributes")] 
    //Primary key could change
    [PrimaryKey("AccountId")]
    [ExplicitColumns]
    public class CustomerAttributes
    {
        [Column("ID")]
        public string AccountId { get; set; }

        [Column("AccountNumber")]
        public string AccountNumber { get; set; }

        [Column("AccountSiteNumber")]
        public string AccountSiteNumber { get; set; }

        [Column("Nickname")]
        public string Nickname { get; set; }

        [Column("OpportunityName")]
        public string OpportunityName { get; set; }

        [Column("UmbracoUserId")]
        public string UmbracoUserId { get; set; }

        [Column("AccountSiteUsePrimary")]
        public string AccountSiteUsePrimary { get; set; }

        [Column("AccountName")]
        public string AccountName { get; set; }

        [Column("AccountStatus")]
        public string AccountStatus { get; set; }

        [Column("ParentObjectPartyNumber")]
        public string ParentObjectPartyNumber { get; set; }

        [Column("ParentObjectPartyName")]
        public string ParentObjectPartyName { get; set; }

        [Column("PlantoObjectPartyNumber")]
        public string PlantoObjectPartyNumber { get; set; }

        [Column("PlantoObjectPartyName")]
        public string PlantoObjectPartyName { get; set; }

        [Column("CustomerClass")]
        public string CustomerClass { get; set; }

        [Column("Segment")]
        public string Segment { get; set; }

        [Column("Subsegment")]
        public string Subsegment { get; set; }

        [Column("Region")]
        public string Region { get; set; }

        [Column("Brands")]
        public string Brands { get; set; }

        [Column("Address1")]
        public string Address1 { get; set; }

        [Column("Address2")]
        public string Address2 { get; set; }

        [Column("City")]
        public string City { get; set; }

        [Column("State")]
        public string State { get; set; }

        [Column("PostalCode")]
        public string PostalCode { get; set; }

        [Column("Country")]
        public string Country { get; set; }

        [Column("FirstName")]
        public string FirstName { get; set; }

        [Column("LastName")]
        public string LastName { get; set; }

        [Column("CompanyName")]
        public string CompanyName { get; set; }

        [Column("Title")]
        public string Title { get; set; }

        [Column("Phone")]
        public string Phone { get; set; }

        [Column("Email")]
        public string Email { get; set; }

        [Column("IsFrapp")]
        public string IsFrapp { get; set; }
        

        public string DisplayName 
        { 
            get
            {
                string returnVal = string.Empty;
                if (!string.IsNullOrEmpty(this.Nickname))
                {
                    returnVal = this.Nickname;
                }
                else if (!string.IsNullOrEmpty(this.OpportunityName))
                {
                    returnVal = this.OpportunityName;
                }
                else
                {
                    //returnVal = this.AccountName + " - " + this.Address1;
                    returnVal = this.Address1;
                }

                return returnVal;
            }
        }
    }

    [TableName("OpportunityAttributes")]
    [ExplicitColumns]
    public class OpportunityAttributes
    {
        [Column("UmbracoUserId")]
        public int UmbracoUserId { get; set; }

        [Column("OpportunityID")]
        public string OpportunityID { get; set; }

        [Column("IntegrationID")]
        public string IntegrationID { get; set; }

        [Column("AccountNumber")]
        public string AccountNumber { get; set; }

        [Column("SiteID")]
        public string SiteID { get; set; }

        [Column("OpportunityName")]
        public string OpportunityName { get; set; }

        [Column("VenueType")]
        public string VenueType { get; set; }

        [Column("SBUXBrewProgram")]
        public string SBUXBrewProgram { get; set; }

        [Column("SBUXEspressoProgram")]
        public string SBUXEspressoProgram { get; set; }

        [Column("SBCAutoshipmentProgram1")]
        public string SBCAutoshipmentProgram1 { get; set; }

        [Column("SBCAutoshipmentProgram2")]
        public string SBCAutoshipmentProgram2 { get; set; }

        [Column("TeaProgram")]
        public string TeaProgram { get; set; }

        [Column("BlendedProgram")]
        public string BlendedProgram { get; set; }

        [Column("SalesStage")]
        public string SalesStage { get; set; }

        [Column("ActualOpenDate")]
        public string ActualOpenDate { get; set; }

    }
}