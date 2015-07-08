using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth2;
using solutions.starbucks.Model;
using solutions.starbucks.web.Controllers.Masters;
using solutions.starbucks.web.Infrastructure;
using solutions.starbucks.web.Infrastructure.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace solutions.starbucks.web.Controllers
{
    public class AuthorizationController : MasterAccountPageController
    {
        private readonly AuthorizationServer server = new AuthorizationServer(new ServerHost());


        [Authorize]
        public ActionResult Authorization()
        {
            var request = this.server.ReadAuthorizationRequest();
            if (request == null)
                throw new HttpException((int)HttpStatusCode.BadRequest, "There's been a problem. Please try refreshing the page. If this message persists, contact us for assistance.");

            var client = DataStore.Instance.Clients
                                .First(c => c.ClientIdentifier == request.ClientIdentifier);


            var authorization = client.ClientAuthorizations.SingleOrDefault(p => p.UserId == Membership.GetUser().ProviderUserKey.ToString());

            if (authorization == null)
            {
                authorization = new solutions.starbucks.web.Infrastructure.Store.ClientAuthorization
                {
                    Scope = new HashSet<string>(new string[] { "Read.Profile" }),
                    UserId = Membership.GetUser().ProviderUserKey.ToString(),
                    IssueDate = DateTime.UtcNow, 
                    ExpirationDateUtc = DateTime.Now.AddDays(7)
                };

                DataStore.Instance.AddAuth(client.ClientIdentifier, authorization);
            }


            if (client != null && request.Scope.Count == 1 && request.Scope.Contains("Read.Profile"))
            {

                var response = this.server.PrepareApproveAuthorizationRequest(request, authorization.UserId, authorization.Scope, request.Callback);

                return this.server.Channel.PrepareResponse(response).AsActionResult();
            }


            var model = new AuthorizationRequest
            {
                ClientApp = DataStore.Instance.Clients
                                .First(c => c.ClientIdentifier == request.ClientIdentifier).Name,
                Scope = request.Scope,
                Request = request,
            };


            return View(model);

        }

    }
}
