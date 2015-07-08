using solutions.starbucks.Interfaces;
using solutions.starbucks.Model.Pocos;
using solutions.starbucks.Model.Pocos.Training;
using solutions.starbucks.Model.Training;
using solutions.starbucks.Repository.PetaPoco;
using solutions.starbucks.Repository.Umbraco;
using solutions.starbucks.web.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TheAlchemediaProject.Services;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.WebApi;


namespace solutions.starbucks.web.Controllers
{
    [Utilities.Training.CustomAuthorization.CustomAuthorize]
    public class InvitesApiController : UmbracoApiController
    {
        private readonly IInvitesRepository _invitesRepository;
        private readonly IInvitesSubjectsRepository _invitesSubjectsRepository;
        private readonly IQuizResponsesRepository _quizResponsesRepository;
        private readonly ICourseBuilderRepository _courseBuilderRepository;
        private readonly IAccessLogRepository _accessLogRepository;
        private readonly IMemberAttributesRepository _memberAttributesRepository;

        public InvitesApiController()
        {
            _invitesRepository = new InvitesRepository();
            _invitesSubjectsRepository = new InvitesSubjectsRepository();
            _quizResponsesRepository = new QuizResponsesRepository();
            _courseBuilderRepository = new CourseBuilderRepository();
            _accessLogRepository = new AccessLogRepository();
            _memberAttributesRepository = new MemberAttributesRepository();
        }

        [Utilities.Training.AntiForgeryToken]
        [System.Web.Http.HttpGet]
        public List<Model.Pocos.Invites> GetByOperator(int id)
        {
            UmbracoHelper umbracoHelper = new UmbracoHelper(UmbracoContext);
            var rootUmbracoElement = umbracoHelper.ContentAtRoot().First();
            var subjectsContents = _courseBuilderRepository.SubjectsContentsFromRoot(rootUmbracoElement);
            IEnumerable<Subject> subjects = _courseBuilderRepository.Subjects(_courseBuilderRepository.SubjectsParent(rootUmbracoElement));
            
            // Retrieve all training invitations for any account to which this operator is associated
            List<Invites> invites = _invitesRepository.GetAccountInvitesByOperatorUmbracoUserID(id, _memberAttributesRepository).OrderByDescending(i => i.DateInvited).ToList();
            // Retrieve invite subjects for given invites
            List<InvitesSubjects> invitesSubjects = null;
            if (invites != null && invites.Count > 0)
                invitesSubjects = _invitesSubjectsRepository.GetInvitesSubjectsByInviteIDs(invites.Select(i => i.InviteID)).ToList();
            // Set subjects instances
            InvitesSubjects.SetAllSubjects(invitesSubjects, subjects);
            invites = solutions.starbucks.Model.Pocos.Invites.SetInvitesSubjects(invites, invitesSubjects).OrderByDescending(i => i.DateInvited).ToList();

            // Get all the responses to all quizzes for all invites
            IEnumerable<QuizResponses> responses = null;
            if (invitesSubjects != null)
                responses = _quizResponsesRepository.GetByInviteSubjectIDs(invitesSubjects.Select(i => i.InviteSubjectID).ToList());

            // Get all quizzes for all invites
            List<Quiz> subjectQuizzes = new List<Quiz>();
            foreach (var subjectContent in subjectsContents)
            {
                Quiz quiz = _courseBuilderRepository.SubjectQuiz(subjectContent);
                if (quiz != null) subjectQuizzes.Add(quiz);
            }

            // Use responses to quizzes and invitation subject views to determine percent complete
            invites = Invites.SetPercentComplete(invites, invitesSubjects, responses, subjectQuizzes).ToList();

            // get access logs for invites
            IEnumerable<AccessLog> accessLogs = null;
            if (invites != null && invites.Count > 0)
            {
                accessLogs = _accessLogRepository.GetByInviteIDs(invites.Select(i => i.InviteID).ToList());
            }
            invites = Invites.SetAccessLogs(invites, accessLogs).ToList();
            invites = Invites.SetAccessStates(invites).ToList();
            return invites;
            
        }

        [Utilities.Training.AntiForgeryToken]
        [System.Web.Http.HttpPost]
        public Invites GetDetails(Invites invite)
        {
            try {
                IEnumerable<QuizResponses> responses = null;
                if (invite.InviteSubjects != null)
                {
                    responses = _quizResponsesRepository.GetByInviteSubjectIDs(invite.InviteSubjects.Select(i => i.InviteSubjectID).ToList());
                    IEnumerable<Question> questions = invite.InviteSubjects.SelectMany(s => s.Subject.Quiz.Questions);

                    //Quiz.SetQuestionResponses(questions, responses);

                    UmbracoHelper umbracoHelper = new UmbracoHelper(UmbracoContext);
                    var rootUmbracoElement = umbracoHelper.ContentAtRoot().First();
                    var subjectsContents = _courseBuilderRepository.SubjectsContentsFromRoot(rootUmbracoElement);

                    // Get all quizzes for all invites
                    List<Quiz> subjectQuizzes = new List<Quiz>();
                    foreach (var subjectContent in subjectsContents)
                    {
                        Quiz quiz = _courseBuilderRepository.SubjectQuiz(subjectContent, responses);
                        if (quiz != null) subjectQuizzes.Add(quiz);
                    }

                    // Set all the quiz instances
                    InvitesSubjects.SetAllSubjectQuizzes(invite.InviteSubjects, subjectQuizzes);
                    
                }
                
            }
            catch (Exception e)
            {
                // TODO: If elaboration is necessary, change the signature of this method to HttpResponseMessage
            }
            return invite;
        }

        
        [Utilities.Training.AntiForgeryToken]
        [HttpPost]
        public Model.Pocos.Invites Save(Model.Pocos.Invites invite)
        {
            try
            {
                Model.Pocos.Invites retVal = _invitesRepository.Save(invite);
                return retVal;
            }
            catch(Exception ex)
            {
                return null;               
            }
            
        }

        [Utilities.Training.AntiForgeryToken]
        [System.Web.Http.HttpPost]
        public Model.Pocos.Invites Delete(Model.Pocos.Invites invite)
        {
            try
            {
                Model.Pocos.Invites retVal = _invitesRepository.Delete(invite);
                _invitesSubjectsRepository.Delete(invite);
                return retVal;
            }
            catch (Exception ex)
            {
                return null;                
            }

        }

        [Utilities.Training.AntiForgeryToken]
        [HttpPost]
        public System.Net.Http.HttpResponseMessage Send(Invites invite)
        {
            bool error = false;

            if (invite == null) error = true;

            var emailFrom = "no-reply@starbucksfs.com";
            var emailSubject = "Please Complete Your We Proudly Serve Starbucks Training";
            MailService email = new MailService();

            IMember trainingOperator = Services.MemberService.GetById(invite.OperatorUmbracoUserID);
            var accessUrl = Classes.Utilities.Training.AccessLink(invite);

            var message = string.Format(
                    "<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>"
                    + "<html xmlns='http://www.w3.org/1999/xhtml'>"
                    +   "<head>"
                    +       "<meta http-equiv='Content-Type' content='text/html; charset=UTF-8' />"
                    +       "<title>Trainee Email</title>"
                    +       "<style>{{ display: block; }} a, a:link, a:visited, a:hover, a:active, a:visited:hover {{ /* :visited:hover is an old IE bug, not sure if it's still relevant */ text-decoration:none;}} body {{-webkit-text-size-adjust: none;}} </style>"
                    +   "</head>"
                    +   "<body>"
                    +       "<table align='center' width='600' border='0' cellspacing='0' cellpadding='0'>"
                    +           "<tr>"
                    +               "<td>"
                    +                   "<img src='http://marlinco.com/eblast/starbucks/su_start_15/images/01_600x90.jpg' width='600' height='90' alt='Branded Solutions' style='display:block;' />"
                    +               "</td>"
                    +           "</tr>"
                    +           "<tr>"
                    +               "<td>"
                    +                   "<a href='{0}'>"
                    +                       "<img src='http://marlinco.com/eblast/starbucks/su_start_15/images/02_600x130.jpg' width='600' height='130' alt='Solutions University ' style='margin-left:0px; margin-top:0px; margin-bottom:0px; margin-right:0px; font-family: Avenir, arial; font-size:14px; line-height: 18px; color:#ffffff' />"
                    +                   "</a>"
                    +               "</td>"
                    +           "</tr>"
                    +           "<tr>"
                    +               "<td align='center'>"
                    +                   "<p style='margin-left:0px; margin-top:30px; margin-bottom:0px; margin-right:0px; font-family: Avenir, arial;font-size:24px; line-height: 30px; color:#3e3935;'>"
                    +                   "{1} has invited you to begin your<br />We Proudly Serve Starbucks training.</p>"
                    +               "</td>"
                    +           "</tr>"
                    +           "<tr>"
                    +               "<td>"
                    +                   "<p style='margin-left:0px; margin-top:30px; margin-bottom:0px; margin-right:0px; font-family: Avenir, arial;font-size:16px; line-height: 30px; color:#3e3935;'>At Solutions University, you'll learn the basics of how to craft Starbucks beverages and deliver the best possible We Proudly Serve Starbucks experience to customers. At the end of each training section, you'll take a short quiz to test your knowledge.</p>"
                    +               "</td>"
                    +           "</tr>"
                    +           "<tr>"
                    +               "<td align='center'>"
                    +                   "<a href='{2}'>"
                    +                       "<img src='http://marlinco.com/eblast/sbs/system_emails/button-get-started.jpg' alt='GET STARTED' width='180' height='36' align='center' style='margin-top:20px;' />"
                    +                   "</a>"
                    +               "</td>"
                    +           "</tr>"
                    +           "<tr>"
                    +               "<td align='center'>"
                    +                   "<p style='margin-left:0px; margin-top:50px; margin-bottom:0px; margin-right:0px; font-family: Avenir, arial;font-size:16px; line-height: 20px; color:#3e3935;'>"
                    +                       "If you received this email by mistake, you can ignore it. If concerned, you may wish to notify us of this activity here: "
                    +                       "<span style='color:#61a60e;'>fsinsidesales@starbucks.com</span>."
                    +                   "</p>"
                    +                   "<p style='margin-left:0px; margin-top:25px; margin-bottom:0px; margin-right:0px; font-family: Avenir, arial;font-size:16px; line-height: 20px; color:#3e3935;'>"
                    +                       "Thanks again,<br/>Starbucks Branded Solutions"
                    +                   "</p>"
                    +               "</td>"
                    +           "</tr>"
                    +       "</table>"
                    +   "</body>"
                    + "</html>",
                    accessUrl, trainingOperator.Name, accessUrl);
            
            var response = new System.Net.Http.HttpResponseMessage();
            
            try {
                email.SendEmail(invite.TraineeEmail, emailFrom, emailSubject, message, true);
            }
            catch (Exception e) {
                error = true;
                response.StatusCode = HttpStatusCode.InternalServerError;
            }

            try {
                if (!error){
                    invite.DateInvited = DateTime.UtcNow;
                    invite = _invitesRepository.Save(invite);
                    invite.AccessState = Invites.GetAccessState(invite);
                }
            }
            catch (Exception e) {
                response.StatusCode = HttpStatusCode.NotModified;
            }
            
            response.Content = new System.Net.Http.ObjectContent<Invites>(invite, Configuration.Formatters.JsonFormatter);
            return response;
            
            //return invite;

        }
       

        
    }
        
}
