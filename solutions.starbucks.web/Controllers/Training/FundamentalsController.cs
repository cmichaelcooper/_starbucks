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
    public class FundamentalsController : MainMasterController
    {
        private readonly IInvitesRepository _invitesRepository;
        private readonly IInvitesSubjectsRepository _invitesSubjectsRepository;
        private readonly ICourseViewerRepository _courseViewerRepository;
        public readonly IFundamentalsRepository _fundamentalsRepository;
        private readonly IAccessLogRepository _accessLogRepository;
        private readonly ITrainingModulesRepository _trainingModulesRepository;

        public FundamentalsController(IInvitesRepository invitesRepository,
                                        IInvitesSubjectsRepository invitesSubjectsRepository, 
                                        ICourseViewerRepository courseViewerRepository, 
                                        IFundamentalsRepository fundamentalsRepository,
                                        IAccessLogRepository accessLogRepository,
                                        ITrainingModulesRepository trainingModulesRepository)
            : base(_ipAddressBlockRepository)
        {
            _invitesRepository = invitesRepository;
            _invitesSubjectsRepository = invitesSubjectsRepository;
            _courseViewerRepository = courseViewerRepository;
            _fundamentalsRepository = fundamentalsRepository;
            _accessLogRepository = accessLogRepository;
            _trainingModulesRepository = trainingModulesRepository;
            _ipAddressBlockRepository = new IPAddressBlockRepository();
        }

        /// <summary>
        /// Fundamentals controller used to display training for each of the fundamental training subjects
        /// </summary>
        /// <returns>Fundamental page with training content if user is logged in. Fundamental page for trainee if user logged in from an training email link. Home if the user isn't logged in or didn't enter from an authenticated training link. </returns>
        /// <remarks>
        /// This page serves as the place for serving up training for each of the fundamental subjects for users that are logged in or entered through an authenticated training email link.
        /// </remarks>
        [Utilities.Training.CustomAuthorization.CustomAuthorize]
        public ActionResult Index(string token = null)
        {
            FundamentalsModel model = _fundamentalsRepository.Fundamental(CurrentPage);

            model.BodyClass = CurrentPage.GetTemplateAlias().ToLower();
            model.BodyId = CurrentPage.UrlName.ToLower();

            Invites invite = Utilities.Training.InviteFromQueryStringToken(token, _invitesRepository);
            model.Invite = invite;

            if (invite != null) {
                
                InvitesSubjects inviteSubject = _invitesSubjectsRepository.GetByInviteIDSubjectID(invite.InviteID, CurrentPage.Id);
                
                // Log that this subject has been accessed
                var accessLog = _accessLogRepository.Insert(AccessLog.Access_Type.Subject, invite.InviteID, inviteSubject.InviteSubjectID);
                
                // Redirect user if training access expired or has invite been deleted
                if (invite.IsDeleted || Invites.GetAccessState(invite) == Invites.AccessStates.Expired)
                {
                    return Redirect("~/resources/training/my-training?token=" + token);
                }
            }
            
            // Look up categories dataTypeId
            var dataTypeId = umbraco.cms.businesslogic.datatype.DataTypeDefinition.GetAll().First(d=> d.Text == "Training Module Categories").Id;
            IEnumerable<Category> categories = _trainingModulesRepository.Categories(dataTypeId);


            // Fill content blocks
            model.ContentBlocks = _fundamentalsRepository.ContentBlocks(CurrentPage);

            return View(model);
        }


    }
}
