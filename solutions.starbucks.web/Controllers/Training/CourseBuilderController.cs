using solutions.starbucks.Interfaces;
using solutions.starbucks.Model.Training;
using solutions.starbucks.Repository.PetaPoco;
using solutions.starbucks.web.Controllers.Masters;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web;

namespace solutions.starbucks.web.Controllers
{
    
    public class CourseBuilderController : MainMasterController
    {

        private readonly IInvitesRepository _invitesRepository;
        private readonly ICourseBuilderRepository _courseBuilderRepository;
        //
        // GET: /CourseBuilder/

        public CourseBuilderController(IInvitesRepository invitesRepository, ICourseBuilderRepository courseBuilderRepository)
            : base(_ipAddressBlockRepository)
        {
            _invitesRepository = invitesRepository;
            _courseBuilderRepository = courseBuilderRepository;
            _ipAddressBlockRepository = new IPAddressBlockRepository();
        }

        /// <summary>
        /// Course builder controller that the training operator uses to send invitations
        /// </summary>
        /// <returns>Course builder (invites) page if user is logged in. Home if the user isn't logged in. </returns>
        /// <remarks>
        /// This page serves as the place for sending invitations to training for users that are logged in.
        /// </remarks>
        [Authorize]
        public ActionResult Index()
        {   

            CourseBuilderModel model = new CourseBuilderModel();
            model.Subjects = _courseBuilderRepository.Subjects(CurrentPage.Parent);

            model.BodyClass = CurrentPage.GetTemplateAlias().ToLower();
            model.BodyId = CurrentPage.UrlName.ToLower();
                        
            var userEmail = HttpContext.User.Identity.Name;
            var currentMember = Services.MemberService.GetByUsername(userEmail);
            model.OperatorUmbracoUserID = currentMember.Id;
            
            return View(model);
        }

       
    }
}
