using solutions.starbucks.Interfaces;
using solutions.starbucks.Model;
using solutions.starbucks.Model.Pocos;
using solutions.starbucks.Model.Pocos.Training;
using solutions.starbucks.Model.Training;
using solutions.starbucks.Repository.PetaPoco;
using solutions.starbucks.web.Classes;
using solutions.starbucks.web.Controllers.Masters;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Umbraco.Web;

namespace solutions.starbucks.web.Controllers
{
    public class CourseViewerController : MainMasterController
    {
        private readonly IInvitesRepository _invitesRepository;
        private readonly IInvitesSubjectsRepository _invitesSubjectsRepository;
        private readonly ICourseBuilderRepository _courseBuilderRepository;
        private readonly IAccessLogRepository _accessLogRepository;

        public CourseViewerController(IInvitesRepository invitesRepository, 
                                        IInvitesSubjectsRepository invitesSubjectsRepository, 
                                        ICourseBuilderRepository courseBuilderRepository,
                                        IAccessLogRepository accessLogRepository)
            : base(_ipAddressBlockRepository)
        {
            _invitesRepository = invitesRepository;
            _invitesSubjectsRepository = invitesSubjectsRepository;
            _courseBuilderRepository = courseBuilderRepository;
            _accessLogRepository = accessLogRepository;
            _ipAddressBlockRepository = new IPAddressBlockRepository();
        }
        
        public ActionResult Login(string token)
        {
            bool valid = false;
            string accessToken = string.Empty;
            EncryptedQueryString args = new EncryptedQueryString(token);
            if (args != null && args["user"] != null && args["token"] != null) {
                string traineeEmail = args["user"];
                accessToken = args["token"];
                
                // Lookup invite and verify email address in token
                Invites invite = _invitesRepository.GetByAccessToken(accessToken);
                if (invite.TraineeEmail == traineeEmail) {
                    //Session[accessToken] = true;
                    Utilities.Training.CustomAuthorization.SetAccessToken(accessToken);
                    valid = true;
                }
            }

            if (valid) {
                return Redirect("~/resources/training/my-training?token=" + token);
            }
            else { 
                return View();
            }
        }


        [Utilities.Training.CustomAuthorization.CustomAuthorize]
        public ActionResult Index(string token = null)
        {
            CourseViewerModel model = new CourseViewerModel();

            List<Subject> subjects = null; 

            Invites invite = Utilities.Training.InviteFromQueryStringToken(token, _invitesRepository);
            if (invite != null) {
                List<InvitesSubjects> subjectInvites = _invitesSubjectsRepository.GetInvitesSubjectsByInviteID(invite.InviteID).ToList();
                List<int> subjectIds = subjectInvites.Select(si => si.SubjectID).ToList();
                subjects = _courseBuilderRepository.Subjects(CurrentPage.Parent, subjectIds).ToList();

                if (subjectInvites.Where(si => si.Completed).ToList().Count == subjectInvites.Count) {
                    model.TrainingComplete = true;
                }

                // Log that the invitation has been accessed
                var accessLog = _accessLogRepository.Insert(AccessLog.Access_Type.Landing, invite.InviteID);
            }
            else
            {
                subjects = _courseBuilderRepository.Subjects(CurrentPage.Parent).ToList();
            }

            model.Subjects = subjects;
            model.Invite = invite;

            model.BodyClass = CurrentPage.GetTemplateAlias().ToLower();
            model.BodyId = CurrentPage.UrlName.ToLower();

            

            return View(model);

        }

    }
}
