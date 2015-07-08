using solutions.starbucks.Interfaces;
using System;
using System.Configuration;
using System.Web.Mvc;
using TheAlchemediaProject.Services;

namespace solutions.starbucks.web.Controllers
{
    public class AutomatedTasksController : Controller
    {
        private readonly IOrdersRepository _orderRepository;

        public AutomatedTasksController(IOrdersRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public ActionResult RunDailyOrderExport()
        {
            string saveFilePath = Server.MapPath("/Output/OrderExport/report" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".xls");
            string saveMarketingPath = Server.MapPath("/Output/OrderExport/marketing" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".xls");
            if (_orderRepository.ExportDailyOrderReport(
                    Server.MapPath("/Output/template.xls"),
                    saveFilePath,
                    Server.MapPath("/Output/template2.xls"),
                    saveMarketingPath
                ))
            {

                new MailService().SendEmail(ConfigurationManager.AppSettings["DailyOrderExportRecipient"], ConfigurationManager.AppSettings["DefaultSystemFromEmailAddress"], "Daily Order Export " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString(), "<H1>File is attached.</H1>", true, saveFilePath);
                var marketingMessage = "Marketing Report is attached.";
                if (System.IO.File.Exists(saveMarketingPath))
                {
                    marketingMessage = "Your marketing materials order report is attached.";
                }
                else
                {
                    marketingMessage = "No marketing materials orders were reported within the last 24 hours.";
                }
                new MailService().SendEmail(ConfigurationManager.AppSettings["DailyMarketingOrderRecipient"], ConfigurationManager.AppSettings["DefaultSystemFromEmailAddress"], "Daily Marketing Export " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString(), "<H1>" + marketingMessage + "</H1>", true, saveMarketingPath);
                return Content("COMPLETED");
            }
            else
            {
                return Content("ERROR");
            }
        }

    }
}
