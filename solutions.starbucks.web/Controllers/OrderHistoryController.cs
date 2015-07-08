using solutions.starbucks.Interfaces;
using solutions.starbucks.Model.Masters;
using solutions.starbucks.web.Controllers.Masters;
using System.Web.Mvc;
using System.Web.Security;
using umbraco.cms.businesslogic.member;

namespace solutions.starbucks.web.Controllers
{
    public class OrderHistoryController : MasterDashboardController
    {
        private readonly IMemberAttributesRepository _memberRepository;
        private readonly IOrdersRepository _ordersRepository;


        public OrderHistoryController(IMemberAttributesRepository memberRepository, IOrdersRepository ordersRepository)
        {
            _ordersRepository = ordersRepository;
            _memberRepository = memberRepository;
        }
        
        [Authorize]
        public ActionResult OrderHistory()
        {
            MasterDashboardModel model = new MasterDashboardModel();
            Member currentMember = Member.GetCurrentMember();
            int umbracoUserID = currentMember.Id;
            var associatedMemberAccounts = _memberRepository.GetFirstAccountReference(umbracoUserID);
            var accountNumber = "";
            bool isPartner = Roles.IsUserInRole(currentMember.Email, "Partner") || Roles.IsUserInRole(currentMember.Email, "PartnerAdmin") || Roles.IsUserInRole(currentMember.Email, "SuperPartnerAdmin") ? true : false;


            if (Session["CurrentAccount"] != null)
            {
                accountNumber = Session["CurrentAccount"].ToString();
            }
            else
            {
                if (!isPartner)
                {
                    accountNumber = associatedMemberAccounts.AccountNumber;
                }
                else
                {
                    return new RedirectResult("/dashboard");
                }
            }

            model.UserOrders = _ordersRepository.GetOrdersByAccount(accountNumber);

            return View(model);
        }
    }
}