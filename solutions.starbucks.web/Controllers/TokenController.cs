using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth2;
using solutions.starbucks.web.Controllers.Masters;
using solutions.starbucks.web.Infrastructure;
using System;
using System.Web.Mvc;

namespace solutions.starbucks.web.Controllers
{
    public class TokenController : MasterAccountPageController
    {
        //
        // GET: /Token/
        private readonly AuthorizationServer server = new AuthorizationServer(new ServerHost());
        public ActionResult Token()
        {
            OutgoingWebResponse resp = null;
            try
            {
                resp = this.server.HandleTokenRequest(this.Request);
            }
            catch (Exception exp)
            {
            }

            return resp.AsActionResult();
        }

    }
}
