using solutions.starbucks.Model;
using solutions.starbucks.Model.Enums;
using System.Collections.Generic;
using Umbraco.Core.Models;

namespace solutions.starbucks.Interfaces
{
    public interface IDashboardRepository
    {
        IEnumerable<DashboardTile> MapBrandedDashboard(IPublishedContent content, BrandCode brandCode);
        IEnumerable<DashboardTile> MapPartnerDashboard(IPublishedContent content);
        IEnumerable<DashboardTile> MapDualDashboard(IPublishedContent content);
        IEnumerable<DashboardTile> MapSBUXBrewedDashboard(IPublishedContent content);
        IEnumerable<DashboardTile> MapSBUXFrappucinoDashboard(IPublishedContent content);
        IEnumerable<DashboardTile> MapDashboard(IPublishedContent content);
    }
}