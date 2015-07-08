using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using solutions.starbucks.Interfaces;
using solutions.starbucks.web.Controllers;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core;

namespace solutions.starbucks.web.Events
{
    public class ApplicationEvents : IApplicationEventHandler
    {
        public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {

        }

        public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            //Adding the following two lines for json serialization of output in API
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);
            //Building IOC Container
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(ProgramController).Assembly,
              typeof(ProgramProductsController).Assembly,
              typeof(AccountSurfaceController).Assembly,
              typeof(DashboardController).Assembly,
              typeof(HomeController).Assembly,
              typeof(OrderHistoryController).Assembly,
              typeof(OrderHistoryDetailsController).Assembly,
              typeof(ProfileSurfaceController).Assembly,
              typeof(ProgramOrderSurfaceController).Assembly,
              typeof(RegisterSurfaceController).Assembly,
              typeof(SiteSurfaceController).Assembly,
              typeof(TextPageController).Assembly,
              typeof(TextPageAuthorizedController).Assembly,
              typeof(LegalTextPageController).Assembly);

            builder.RegisterType<Repository.Umbraco.ProgramRepository>().As<IProgramRepository>();
            builder.RegisterType<Repository.Umbraco.TextPageRepository>().As<ITextPageRepository>();
            builder.RegisterType<Repository.PetaPoco.DictionaryItemRepository>().As<IDictionaryItemRepository>();
            builder.RegisterType<Repository.PetaPoco.ProductsRepository>().As<IProductsRepository>();
            builder.RegisterType<Repository.PetaPoco.IPAddressBlockRepository>().As<IIPAddressBlockRepository>();
            builder.RegisterType<Repository.PetaPoco.MemberAttributesRepository>().As<IMemberAttributesRepository>();
            builder.RegisterType<Repository.PetaPoco.PartnerAccountsRepository>().As<IPartnerAccountsRepository>();
            builder.RegisterType<Repository.PetaPoco.OrdersRepository>().As<IOrdersRepository>();
            builder.RegisterType<Repository.Umbraco.DashboardRepository>().As<IDashboardRepository>();
            builder.RegisterType<Repository.Umbraco.LegalTextPageRepository>().As<ILegalTextPageRepository>();
            //Need a new container for API objects
            builder.RegisterType<Repository.PetaPoco.InvitesRepository>().As<IInvitesRepository>();
            builder.RegisterType<Repository.PetaPoco.InvitesSubjectsRepository>().As<IInvitesSubjectsRepository>();
            builder.RegisterType<Repository.PetaPoco.AccessLogRepository>().As<IAccessLogRepository>();
            builder.RegisterType<Repository.PetaPoco.QuizResponsesRepository>().As<IQuizResponsesRepository>();
            builder.RegisterType<Repository.Umbraco.CourseBuilderRepository>().As<ICourseBuilderRepository>();
            builder.RegisterType<Repository.Umbraco.FundamentalsRepository>().As<IFundamentalsRepository>();
            builder.RegisterType<Repository.Umbraco.CourseViewerRepository>().As<ICourseViewerRepository>();
            builder.RegisterType<Repository.Umbraco.TrainingModulesRepository>().As<ITrainingModulesRepository>();
            var apiBuilder = new ContainerBuilder();
            apiBuilder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).Where(t =>
               !t.IsAbstract && typeof(ApiController).IsAssignableFrom(t))
            .InstancePerMatchingLifetimeScope(
               AutofacWebApiDependencyResolver.ApiRequestTag);
            apiBuilder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            apiBuilder.RegisterType<Repository.PetaPoco.ProductsRepository>().As<IProductsRepository>();
            apiBuilder.RegisterType<Repository.PetaPoco.RecipesRepository>().As<IRecipesRepository>();

            var container = builder.Build();

            var apiContainer = apiBuilder.Build();
            //var resolver = new AutofacWebApiDependencyResolver(container);
            //   config.DependencyResolver = resolver;
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(apiContainer);
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            //Create a custom route
            RouteTable.Routes.MapRoute(
                "CoffeeDetailsRoute",
                "products/coffee/details/{productId}",
                new
                {
                    controller = "CoffeeDetails",
                    action = "CoffeeDetails",
                    productId = UrlParameter.Optional
                });

            
            RouteTable.Routes.MapRoute(
                "ProductDetailsRoute",
                "products/details/{productId}",
                new
                {
                    controller = "ProductDetails",
                    action = "ProductDetails",
                    productId = UrlParameter.Optional
                });
            
            RouteTable.Routes.MapRoute(
                "RecipeCatalogRoute",
                "resources/recipes/catalog/{category}",
                new
                {
                    controller = "RecipeCatalog",
                    action = "RecipeCatalog",
                    category = UrlParameter.Optional
                });
            RouteTable.Routes.MapRoute(
                "RecipeDetailsRoute",
                "resources/recipes/details/{recipeId}",
                new
                {
                    controller = "RecipeDetails",
                    action = "RecipeDetails",
                    recipeId = UrlParameter.Optional
                });


            RouteTable.Routes.MapRoute(
                "DailyOrderExport",
                "AutomatedTasks/RunDailyOrderExport",
                new
                {
                    controller = "AutomatedTasks",
                    action = "RunDailyOrderExport"
                });
            //DependencyResolver.SetResolver(new AutofacWebApiDependencyResolver(apiContainer));
        }

        public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext) { }
    }
}