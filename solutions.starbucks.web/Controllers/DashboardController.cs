using solutions.starbucks.Interfaces;
using solutions.starbucks.Model;
using solutions.starbucks.Model.Enums;
using solutions.starbucks.Model.Pocos;
using solutions.starbucks.web.Controllers.Masters;
using solutions.starbucks.web.Helpers;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using umbraco.NodeFactory;
using Umbraco.Web;

namespace solutions.starbucks.web.Controllers 
{
    public class DashboardController : MasterDashboardController 
    {
        //protected MemberAttributesRepository _memberRepository;
        //protected DashboardRepository _dashboardRepository;

        private readonly IOrdersRepository _ordersRepository;
        private readonly IDashboardRepository _dashboardRepository;
        private readonly IMemberAttributesRepository _memberRepository;

        public DashboardController(IOrdersRepository ordersRepository, IDashboardRepository dashboardRepository, IMemberAttributesRepository memberRepository) 
        {
            _ordersRepository = ordersRepository;
            _dashboardRepository = dashboardRepository;
            _memberRepository = memberRepository;
        }

        /// <summary>
        /// Dashboard controller for page to show when user logs in
        /// </summary>
        /// <returns>User Profile if user is logged in, has verified email and unverified account/zip combination. Dashboard if user is logged in, has verified email and verified account/zip combination. Home if the user isn't logged in. </returns>
        /// <remarks>
        /// This page serves as the homepage after a user logs in. We are serving up specific content based on the user type and brand affiliation. 
        /// </remarks>
        [Authorize]
        public ActionResult Dashboard() 
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return new RedirectResult("/");
            }

            var userEmail = HttpContext.User.Identity.Name;
            var currentMember = Services.MemberService.GetByUsername(userEmail);

            if (currentMember == null)
            {
                return new RedirectResult("/umbraco/Surface/AccountSurface/Logout");
            }

            //Ensure user has a verified account/zip combination
            bool unverifiedCustomer = Roles.IsUserInRole(currentMember.Email, "Unverified Customer");
            if (unverifiedCustomer)
            {
                return new RedirectResult("/Profile/Edit");
            }

            //User has been marked "invalid", need to log them out and ensure clean session
            //They will receive message on next attempted login 
            bool invalidCustomer = Roles.IsUserInRole(currentMember.Email, "Inactive");
            if (invalidCustomer)
            {
                return new RedirectResult("/umbraco/Surface/AccountSurface/Logout");
            }

            //Ensure validated account number
            if (!Convert.ToBoolean(currentMember.Properties["validatedAccountNumber"].Value))
            {
                TempData["InvalidAccount"] = true;
                return new RedirectResult("/Profile/Edit");
            }

            DashboardModel dashboardModel = new DashboardModel();
            
            var nodesOutput = Node.GetCurrent().ChildrenAsList;
            int currentId = Node.GetCurrent().Id;

            dashboardModel.Title = currentMember.Name + "'s Dashboard";
            dashboardModel.BodyClass = CurrentPage.GetTemplateAlias().ToLower();
            dashboardModel.BodyId = CurrentPage.UrlName.ToLower();

            int umbracoUserID = currentMember.Id;
            Session["CurrentMemberID"] = umbracoUserID;

            bool isPartner = AccountHelpers.DetermineIsPartner(currentMember.Email);
            dashboardModel.isPartner = isPartner;

            var associatedMemberAccounts = new AssociatedMemberAccount();
            var associatedCustomerAccounts = new CustomerAttributes();
            dashboardModel.NumberOfAccounts = 0;

            //Partners and dual brand customers can impersonate or view other dashboards
            bool isEspresso = true;
            if (Session["CurrentAccount"] != null)
            {
                dashboardModel.CurrentAccount = Session["CurrentAccount"].ToString();
                associatedCustomerAccounts = _memberRepository.GetFirstCustomerAccountReference(Session["CurrentAccount"].ToString());
                if (associatedCustomerAccounts != null)
                {

                    dashboardModel.CurrentAccountName = associatedCustomerAccounts.AccountName;
                    dashboardModel.isFrappuccino = Convert.ToBoolean(associatedCustomerAccounts.IsFrapp);
                    dashboardModel.CustomerBrand = AccountHelpers.DetermineBrand(associatedCustomerAccounts.Brands);

                    dashboardModel.NumberOfAccounts = _memberRepository.GetAssociatedAccount(umbracoUserID).Count();
                            
                    if (isPartner)
                    {
                        dashboardModel.UserOrders = _ordersRepository.GetOrdersByAccount(Session["CurrentAccount"].ToString());
                    }

                    var brewedAccount = _memberRepository.GetStarbucksBrewedAccount(associatedCustomerAccounts.AccountNumber);
                    if (brewedAccount != null)
                    {
                        isEspresso = false;
                        dashboardModel.TileContents = _dashboardRepository.MapSBUXBrewedDashboard(CurrentPage);
                    }
                    else if (dashboardModel.CustomerBrand == BrandCode.DUAL)
                    {
                        dashboardModel.TileContents = _dashboardRepository.MapDualDashboard(CurrentPage);
                    }
                    else if (dashboardModel.isFrappuccino)
                    {
                        dashboardModel.TileContents = _dashboardRepository.MapSBUXFrappucinoDashboard(CurrentPage);
                    }
                    else
                    {
                        dashboardModel.TileContents = _dashboardRepository.MapBrandedDashboard(CurrentPage, dashboardModel.CustomerBrand);
                    }
                }
                else
                {
                    TempData["IncorrectAccountNumber"] = true;
                }
            }
            else
            {
                if (!isPartner)
                {
                    //Set associated accounts
                    associatedMemberAccounts = _memberRepository.GetFirstAccountReference(umbracoUserID);
                    var currentAccount = _memberRepository.GetFirstCustomerAccountReference(associatedMemberAccounts.AccountNumber);
                    dashboardModel.CustomerBrand = AccountHelpers.DetermineBrand(currentAccount.Brands);
                    Session["CurrentAccount"] = associatedMemberAccounts.AccountNumber;

                    dashboardModel.CurrentAccount = associatedMemberAccounts.AccountNumber;
                    dashboardModel.NumberOfAccounts = _memberRepository.GetAssociatedAccount(umbracoUserID).Count();
                    //Brewed accounts for SBX only
                    var brewedAccount = _memberRepository.GetStarbucksBrewedAccount(associatedMemberAccounts.AccountNumber);
                    if (brewedAccount != null)
                    {
                        isEspresso = false;
                        dashboardModel.TileContents = _dashboardRepository.MapSBUXBrewedDashboard(CurrentPage);
                    }
                    else if (dashboardModel.CustomerBrand == BrandCode.DUAL)
                    {
                        dashboardModel.TileContents = _dashboardRepository.MapDualDashboard(CurrentPage);
                    }
                    else
                    {
                        dashboardModel.TileContents = _dashboardRepository.MapBrandedDashboard(CurrentPage, dashboardModel.CustomerBrand);
                    }
                }
                else
                {
                    dashboardModel.CustomerBrand = BrandCode.DUAL;
                    dashboardModel.TileContents = _dashboardRepository.MapPartnerDashboard(CurrentPage);
                }
            }
                   
            //Ensure member account isn't null
            if (associatedCustomerAccounts != null && associatedMemberAccounts != null)
            {
                if (associatedCustomerAccounts.AccountNumber != null)
                {
                           
                    if (!isPartner && Session["CurrentAccount"] != null)
                    {
                        dashboardModel.UserOrders = _ordersRepository.GetOrdersByAccount(associatedCustomerAccounts.AccountNumber);
                    }
                }
                if (associatedMemberAccounts.AccountNumber != null)
                {
                           

                    if (!isPartner && Session["CurrentAccount"] != null)
                    {
                        dashboardModel.UserOrders = _ordersRepository.GetOrdersByAccount(associatedMemberAccounts.AccountNumber);
                    }
                }
            }
            else
            {
                //Incorrectly typed account number (should only ever occur for a partner)
                dashboardModel.CustomerBrand = BrandCode.DUAL;
                dashboardModel.TileContents = _dashboardRepository.MapPartnerDashboard(CurrentPage);
                //dashboardModel.SecondFeaturedProduct = CurrentPage.GetPropertyValue<IHtmlString>("sbcFeaturedProduct");
                TempData["IncorrectAccountNumber"] = true;
                return View(dashboardModel);
            }

            AccountHelpers.SetCurrentAccountSessionVars(dashboardModel.CurrentAccount, dashboardModel.CustomerBrand.Code, dashboardModel.isPartner, isEspresso, dashboardModel.isFrappuccino);

            return View(dashboardModel);
        }
    }
}
