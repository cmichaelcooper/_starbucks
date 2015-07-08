using solutions.starbucks.Interfaces;
using solutions.starbucks.Model.Pocos;
using solutions.starbucks.Repository.PetaPoco;
using solutions.starbucks.web.Classes;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Umbraco.Web.WebApi;

namespace solutions.starbucks.web.Controllers
{
    [Utilities.Training.CustomAuthorization.CustomAuthorize]
    public class InvitesSubjectsApiController : UmbracoApiController
    {
        private readonly IInvitesSubjectsRepository _invitesSubjectsRepository;

        public InvitesSubjectsApiController()
        {
            _invitesSubjectsRepository = new InvitesSubjectsRepository();
        }


        [Utilities.Training.AntiForgeryToken]
        [System.Web.Http.HttpGet]
        public List<Model.Pocos.InvitesSubjects> Index(int id)
        {
            return _invitesSubjectsRepository.GetInvitesSubjectsByInviteID(id).ToList();
        }

        [Utilities.Training.AntiForgeryToken]
        [HttpPost]
        public ActionResult Save([System.Web.Http.FromBody]PostInviteSubjectRequest request)
        {
            bool error = false;
            foreach (var subjectId in request.SubjectSelection) {
                try {
                    InvitesSubjects inviteSubject = new InvitesSubjects();
                    inviteSubject.InviteID = request.Invite.InviteID;
                    inviteSubject.SubjectID = subjectId;
                    _invitesSubjectsRepository.Save(inviteSubject);
                }
                catch {
                    error = true;
                }                
            }

            if (!error) {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }
            else {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError);
            }            
        }

        /// <summary>
        /// WebAPI only allows one "FromBody" parameter, so use this helper class to retrieve data posted to the Save method
        /// </summary>
        public class PostInviteSubjectRequest
        {
            public Invites Invite { get; set; }
            public int[] SubjectSelection { get; set; }
        }
        
    }
    
}
