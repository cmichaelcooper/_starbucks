using solutions.starbucks.Interfaces;
using solutions.starbucks.Model.Training;
using solutions.starbucks.Repository.PetaPoco;
using solutions.starbucks.web.Controllers.Masters;
using System.Web.Mvc;
using Umbraco.Web;

namespace solutions.starbucks.web.Controllers
{
    
    public class TrainingController : MainMasterController
    {

        private readonly IInvitesRepository _invitesRepository;
        private readonly ICourseBuilderRepository _courseBuilderRepository;
        //
        // GET: /CourseBuilder/

        public TrainingController(IInvitesRepository invitesRepository, ICourseBuilderRepository courseBuilderRepository)
            : base(_ipAddressBlockRepository)
        {
            _invitesRepository = invitesRepository;
            _courseBuilderRepository = courseBuilderRepository;
            _ipAddressBlockRepository = new IPAddressBlockRepository();
        }

        /// <summary>
        /// Training page controller to show for user that is logged in as the training operator
        /// </summary>
        /// <returns>Training page if user is logged in. Home if the user isn't logged in. </returns>
        /// <remarks>
        /// This page serves as the landing page for training for users that are logged in.
        /// </remarks>
        [Authorize]
        public ActionResult Index()
        {

            TrainingModel model = new TrainingModel();
            
            model.BodyClass = CurrentPage.GetTemplateAlias().ToLower();
            model.BodyId = CurrentPage.UrlName.ToLower();
                        
            return View(model);
        }

       
    }
}
