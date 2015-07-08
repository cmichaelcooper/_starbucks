using solutions.starbucks.Model.Enums;
using solutions.starbucks.Model.Masters;
using solutions.starbucks.Model.Pocos;
using System.Collections.Generic;
using System.Web;
using Umbraco.Core.Models;

namespace solutions.starbucks.Model
{
    public class DashboardModel : MasterDashboardModel
    {
        public IEnumerable<CustomerAttributes> CustomerAttributes { get; set; }

        public IEnumerable<AssociatedMemberAccount> AssociatedAccounts { get; set; }

        public BrandCode CustomerBrand { get; set; }

        public string CurrentAccount { get; set; }
        
        public string CurrentAccountName { get; set; }

        public bool isPartner { get; set; }

        public int NumberOfAccounts { get; set; }

        public IEnumerable<IContent> DashboardTiles { get; set; }

        public IEnumerable<DashboardTile> TileContents { get; set; }

        public bool isFrappuccino { get; set; }

    }

    public class DashboardTile : DashboardModel
    {
        //Featured field for this tile
        public IHtmlString FeaturedBox { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }

        public string BrandSelect { get; set; }

        public bool PartnerViewable { get; set; }

        public string TileLink { get; set; }

        public string TileLinkTarget { get; set; }

        public string TileLinkHash { get; set; }

        public string TrackCampaign { get; set; }

        public string TrackMedium { get; set; }

        public string TrackSource { get; set; }

        public string TrackContent { get; set; }

        public string AccountType { get; set; }

    }
}