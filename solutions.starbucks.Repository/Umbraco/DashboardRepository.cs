using solutions.starbucks.Interfaces;
using solutions.starbucks.Model;
using solutions.starbucks.Model.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace solutions.starbucks.Repository.Umbraco
{
    public class DashboardRepository : IDashboardRepository
    {
        private static UmbracoRepository _umbracoRepository = new UmbracoRepository();

        /// <summary>
        /// Generic tile mapper to retrieve all tiles for the dashboard
        /// </summary>
        /// <param name="content">Published dashboard tiles that are sub-content of the dashboard.</param>
        /// <returns>The dashboard tiles in an IEnumberable</returns>
        public IEnumerable<DashboardTile> MapDashboard(IPublishedContent content)
        {
            var dashboardTiles = _umbracoRepository.GetPublishedChildItems(content)
                .Select(S => new DashboardTile
                {
                    FeaturedBox = S.GetPropertyValue<IHtmlString>("featuredBox"),
                    Width = S.GetPropertyValue<int>("width"),
                    Height = S.GetPropertyValue<int>("height"),
                    BrandSelect = S.GetPropertyValue<string>("brandSelect"),
                    Title = S.Name,
                    TileLink = S.GetPropertyValue<string>("linkUrl"),
                    TileLinkTarget = S.GetPropertyValue<string>("linkUrlTarget"),
                    TileLinkHash = S.GetPropertyValue<string>("urlHash"),
                    TrackCampaign = S.GetPropertyValue<string>("trackingCampaign"),
                    TrackSource = S.GetPropertyValue<string>("trackingSource"),
                    TrackMedium = S.GetPropertyValue<string>("trackingMedium"),
                    TrackContent = S.GetPropertyValue<string>("trackingContent"),
                    AccountType = S.GetPropertyValue<string>("accountType"),
                    PartnerViewable = S.GetPropertyValue<bool>("partnerViewable")
                });

            return dashboardTiles;
        }
        
        /// <summary>
        /// Maps Dashboard Tiles to their respective brand (SBX & SBC)
        /// </summary>
        /// <param name="content">Published dashboard tiles that are sub-content of the dashboard.</param>
        /// <returns>The dashboard tiles in an IEnumberable</returns>
        public IEnumerable<DashboardTile> MapBrandedDashboard(IPublishedContent content, BrandCode brandCode)
        {
            var dashboardTiles = MapDashboard(content).Where((p => p.BrandSelect == brandCode.Code.ToLower() && p.BrandSelect != "partner" && p.BrandSelect != BrandCode.DUAL.Code.ToLower() && p.AccountType != "SBUXBrewed" && p.AccountType != "SBUXFrapp"));
            return dashboardTiles;
        }

        /// <summary>
        /// Maps Dashboard Tiles specifically for Starbucks Partner Users
        /// </summary>
        /// <param name="content">Published dashboard tiles that are sub-content of the dashboard parent.</param>
        /// <returns>The dashboard tiles in an IEnumberable that are viewable for partners only.</returns>
        public IEnumerable<DashboardTile> MapPartnerDashboard(IPublishedContent content)
        {
            var dashboardTiles = MapDashboard(content).Where(p => p.PartnerViewable && p.BrandSelect != BrandCode.DUAL.Code.ToLower());
            return dashboardTiles;
        }

        /// <summary>
        /// Maps Dashboard Tiles specifically for users with Dual Brand accounts (Starbucks and SBC both)
        /// </summary>
        /// <param name="content">Published dashboard tiles that are sub-content of the dashboard parent.</param>
        /// <returns>The dashboard tiles in an IEnumberable that are viewable for dual brand users only.</returns>
        public IEnumerable<DashboardTile> MapDualDashboard(IPublishedContent content)
        {
            var dashboardTiles = MapDashboard(content).Where(p => p.BrandSelect == BrandCode.DUAL.Code.ToLower() && p.AccountType != "SBUXBrewed" && p.AccountType != "SBUXFrapp");
            return dashboardTiles;
        }

        /// <summary>
        /// Maps Dashboard Tiles for SBX Brewed Program
        /// </summary>
        /// <param name="content">Published dashboard tiles that are sub-content of the dashboard.</param>
        /// <returns>The dashboard tiles in an IEnumberable</returns>
        public IEnumerable<DashboardTile> MapSBUXBrewedDashboard(IPublishedContent content)
        {
            var dashboardTiles = MapDashboard(content).Where(p => p.AccountType == "SBUXBrewed");
            return dashboardTiles;
        }

        /// <summary>
        /// Maps Dashboard Tiles for SBX Frappucino Program
        /// </summary>
        /// <param name="content">Published dashboard tiles that are sub-content of the dashboard.</param>
        /// <returns>The dashboard tiles in an IEnumberable</returns>
        public IEnumerable<DashboardTile> MapSBUXFrappucinoDashboard(IPublishedContent content)
        {
            var dashboardTiles = MapDashboard(content).Where(p => p.AccountType == "SBUXFrapp");
            return dashboardTiles;
        }

        /// <summary>
        /// Maps Dashboard Tiles for Branded Espresso
        /// </summary>
        /// <param name="content">Published dashboard tiles that are sub-content of the dashboard.</param>
        /// <returns>The dashboard tiles in an IEnumberable</returns>
        public IEnumerable<DashboardTile> MapEspressoDashboard(IPublishedContent content, BrandCode brandCode)
        {
            var dashboardTiles = MapDashboard(content).Where((p => p.BrandSelect == brandCode.Code.ToLower() && p.BrandSelect != "partner" && p.BrandSelect != BrandCode.DUAL.Code.ToLower() && p.AccountType == "SBUXEspresso"));
            return dashboardTiles;
        }


    }
}