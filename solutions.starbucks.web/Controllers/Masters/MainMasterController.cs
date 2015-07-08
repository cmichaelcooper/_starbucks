using solutions.starbucks.Interfaces;
using solutions.starbucks.Model.Masters;
using solutions.starbucks.web.Classes;
using System;
using System.Globalization;
using System.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace solutions.starbucks.web.Controllers.Masters
{
    public class MainMasterController : RenderMvcController
    {
        protected static IIPAddressBlockRepository _ipAddressBlockRepository;

        public MainMasterController(IIPAddressBlockRepository ipAddressBlockRepository) 
        {
            _ipAddressBlockRepository = ipAddressBlockRepository;
        }

        protected ViewResult View(MasterModel model)
        {
            return this.View(null, model);
        }
        protected ViewResult View(string view, MasterModel model)
        {
            var root = CurrentPage.AncestorOrSelf(1);

            string actualIP;
            var proxyAddress = Request.UserHostAddress;

            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    actualIP = addresses[0];
                    model.IPCountry = _ipAddressBlockRepository.GetCountryForIPAddress(actualIP);
                }
            }

            if (model.IPCountry == null)
                model.IPCountry = _ipAddressBlockRepository.GetCountryForIPAddress(proxyAddress);

            model.Site = "Starbucks Branded Solutions";

            model.Title = model.Title != null ? model.Title : CurrentPage.Name;
            if (Request.Cookies["OrderID"] != null)
            {
                if (Request.Cookies["OrderBrand"] != null)
                {
                    model.OrderBrand = Request.Cookies["OrderBrand"].Value;
                }
                if (!String.IsNullOrEmpty(Request.Cookies["OrderID"].Value))
                {
                    model.OrderID = SecurityMethods.Encrypt(Request.Cookies["OrderID"].Value);
                    model.UnencryptedItem = SecurityMethods.Decrypt(model.OrderID);

                }
            }
            

            
            return base.View(view, model);
        }

        public RenderModel CreateRenderModel(IPublishedContent content)
        {
            var model = new RenderModel(content, CultureInfo.CurrentUICulture);

            //add an umbraco data token so the umbraco view engine executes
            RouteData.DataTokens["umbraco"] = model;

            return model;
        }
    }
}
