using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core;

namespace solutions.starbucks.web.CustomUmbraco
{
    public class RegisterEvents : ApplicationEventHandler
    {
        //This happens everytime the Umbraco Application starts
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {

            // This routes the Login link for training
            RouteTable.Routes.MapRoute(
                name: "CourseViewer",
                url: "support/training/my-training/{action}",
                defaults: new { controller = "CourseViewer"}
            );

            ////Get the Umbraco Database context
            //var db = applicationContext.DatabaseContext.Database;

            ////Check if the DB table does NOT exist
            //if (!db.TableExist("CustomerAttributes"))
            //{
            //    //Create DB table - and set overwrite to false
            //    db.CreateTable<CustomerAttributes>(false);
            //}

            //Example of other events (such as before publish)
            //Document.BeforePublish += Document_BeforePublish;
        }

        //Example Before Publish Event
        //private void Document_BeforePublish(Document sender, PublishEventArgs e)
        //{
        //    //Do what you need to do. In this case logging to the Umbraco log
        //    Log.Add(LogTypes.Debug, sender.Id, "the document " + sender.Text + " is about to be published");

        //    //cancel the publishing if you want.
        //    e.Cancel = true;
        //}
    }
}