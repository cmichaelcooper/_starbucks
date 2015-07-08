using solutions.starbucks.Interfaces;
using solutions.starbucks.Model.Masters;
using solutions.starbucks.Model.Pocos;
using solutions.starbucks.Model.Training;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TheAlchemediaProject.Services;
using Umbraco.Core.Models;
using Umbraco.Web.Mvc;

namespace solutions.starbucks.web.Controllers
{
    public class CourseSurfaceController : SurfaceController
    {
        private readonly IInvitesRepository _invitesRepository;
        private readonly IInvitesSubjectsRepository _invitesSubjectsRepository;
        private readonly ICourseBuilderRepository _courseBuilderRepository;
        public readonly IQuizResponsesRepository _quizResponsesRepository;
        public readonly ICourseViewerRepository _courseViewerRepository;
        public readonly ITrainingModulesRepository _trainingModulesRepository;

        public CourseSurfaceController(IInvitesRepository invitesRepository, 
                                                    IInvitesSubjectsRepository invitesSubjectsRepository, 
                                                    ICourseBuilderRepository courseBuilderRepository, 
                                                    IQuizResponsesRepository quizResponsesRepository, 
                                                    ICourseViewerRepository courseViewerRepository,
                                                    ITrainingModulesRepository trainingModulesRepository)
        {
            _invitesRepository = invitesRepository;
            _invitesSubjectsRepository = invitesSubjectsRepository;
            _courseBuilderRepository = courseBuilderRepository;
            _quizResponsesRepository = quizResponsesRepository;
            _courseViewerRepository = courseViewerRepository;
            _trainingModulesRepository = trainingModulesRepository;
        }

        public ActionResult SessionExpired()
        {
            MasterModel model = new MasterModel();
            return View("CourseSessionExpired", model);
        }


        [ChildActionOnly]
        public ActionResult RenderSupplementalCategories()
        {
            return PartialView("SupplementalCategories", GetSupplementalCategories(CurrentPage));
        }

        private SupplementalCategories GetSupplementalCategories(IPublishedContent content)
        {

            SupplementalCategories model = new SupplementalCategories();

            int dataTypeId = umbraco.cms.businesslogic.datatype.DataTypeDefinition.GetAll().First(d => d.Text == "Training Module Categories").Id;
            List<Category> categories = _trainingModulesRepository.Categories(dataTypeId).ToList();

            List<Module> modules = _trainingModulesRepository.Modules(content).ToList();

            Dictionary<Category, List<Module>> modulesByCategory = new Dictionary<Category, List<Module>>();
            foreach (var category in categories) {
                IEnumerable<Module> categoryModules = null;
                if (modules != null) categoryModules = modules.Where(m => m.CategoryNames.Contains(category.Name));
                modulesByCategory.Add(category, categoryModules.ToList());
            }
            
            model.Categories = categories;
            model.Modules = modules;

            return model;
        }
        
        [ChildActionOnly]
        public ActionResult RenderCourseNavigation(string token, int inviteId, int subjectId)
        {
            CourseContent model = FillCourseContent(inviteId, subjectId, CurrentPage, token != null);
            if (model.SubjectQuiz != null && model.SubjectQuiz.HasResponses()) {
                model.SubjectQuiz.Graded = true;
            }

            // Log access to this Subject
            if (model.InviteSubject != null)
            {
                var now = DateTime.UtcNow;
                model.InviteSubject.DateAccessed = now;
                if (model.SubjectQuiz == null) model.InviteSubject.DateCompleted = now;
                _invitesSubjectsRepository.Save(model.InviteSubject);
            }
            
            // Check course completion status
            ProcessCourseCompleted(model);

            return PartialView("CourseNavigation", model);


        }

        [HttpPost]
        public ActionResult ProcessCourseNavigation(int inviteId, int subjectId, FormCollection collection)
        {
            if (!ModelState.IsValid)
            {
                string retValue = "Invalid ModelState";
                return Content(retValue);
            }

            var currentPage = Umbraco.TypedContent(subjectId);
            CourseContent content = FillCourseContent(inviteId, subjectId, currentPage, renderQuiz:true);
            
            try
            {
                // Save response for each question
                foreach (var q in content.SubjectQuiz.Questions)
                {
                    QuizResponses response = q.Response;
                    if (response == null) {
                        response = new QuizResponses();
                        response.InviteSubjectID = content.InviteSubject.InviteSubjectID;
                        response.QuestionID = q.QuestionID;
                        response.DateCreated = DateTime.UtcNow;
                        q.Responses = new QuizResponses[]{ response };                        
                    }
                    response.Value = collection[q.QuestionID.ToString()];
                    response = _quizResponsesRepository.Save(response);
                }
                // Mark this subject completed
                content.InviteSubject.DateCompleted = DateTime.UtcNow;
                _invitesSubjectsRepository.Save(content.InviteSubject);

                content.Console = "Saved sucessfully!";
                content.SubjectQuiz.Graded = true;
                ProcessCourseCompleted(content);
            }
            catch (Exception exception)
            {
                content.Console = "Error saving quiz responses";
            }

            return PartialView("CourseNavigation", content);
        }


        private CourseContent FillCourseContent(int inviteId, int subjectId, IPublishedContent currentPage, bool renderQuiz)
        {
            return _courseViewerRepository.FillCourseContent(_invitesRepository, _invitesSubjectsRepository, _courseBuilderRepository, _quizResponsesRepository, inviteId, subjectId, currentPage, renderQuiz);
        }


        public void ProcessCourseCompleted(CourseContent courseContent)
        {
            bool markCompleted = MarkCourseCompleted(courseContent);
            if (markCompleted)
            {
                // Fire-and-forget notification to operator
                var trainingOperator = Services.MemberService.GetById(courseContent.Invite.OperatorUmbracoUserID);
                System.Threading.Tasks.Task.Run(() => SendOperatorNotification(courseContent, trainingOperator, Request));
            }            
        }

        public bool MarkCourseCompleted(CourseContent courseContent) {

            bool markCompleted = false;

            // Don't run the check if the course has already been completed
            if (!courseContent.CourseCompleted)
            {
                //CourseContent.SetSubjectsCompletionStatus(courseContent.SubjectInvites, courseContent.SubjectQuizzes);
                courseContent.CourseCompleted = CourseContent.GetCourseCompleted(courseContent.SubjectInvites);

                // If the course is complete, mark the invite completed and notify the operator (if it has not been done already)
                if (courseContent.CourseCompleted && courseContent.Invite.DateCompleted == null)
                {
                    courseContent.Invite.DateCompleted = DateTime.UtcNow;
                    _invitesRepository.Save(courseContent.Invite);
                    markCompleted = true;
                }
            }
            return markCompleted;
        }

        public static bool SendOperatorNotification(CourseContent courseContent, IMember trainingOperator, HttpRequestBase request)
        {
            bool error = false;

            if (courseContent.Invite == null) error = true;

            int questionTotal = courseContent.SubjectInvites.Where(si => si.Subject.Quiz != null)
                    .SelectMany(si => si.Subject.Quiz.Questions).Count();
            int answeredCorrectlyTotal = courseContent.SubjectInvites.Where(si => si.Subject.Quiz != null)
                    .SelectMany(si => si.Subject.Quiz.Questions).Where(q => q.Response.Value == q.CorrectAnswer.AnswerID.ToString()).Count();

            string emailImgBaseUrl = "http://marlinco.com/eblast/sbs/system_emails/";
            string gradeImgUrl = "<img width='15' height='15' src='{0}' alt='' />";
            string gradeImgUrlRight = string.Format(gradeImgUrl, emailImgBaseUrl + "question-right.png");
            string gradeImgUrlWrong = string.Format(gradeImgUrl, emailImgBaseUrl + "question-wrong.png"); 


            StringBuilder grades = new StringBuilder();
            foreach (var inviteSubject in courseContent.SubjectInvites.OrderBy(s => s.Subject.SortOrder))
            {
                grades.Append(String.Format("<table style='margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left:18px; text-align:left; font-family:Corbel, sans-serif; font-size:18px; font-weight:normal; color:#3E3935;'><tr>"));
                grades.Append(String.Format("<td style='width: 118px;'><p style='margin-top:0; margin-bottom:0; padding-top:0; padding-bottom:0; font-size:20px; font-weight: bold;'>Quiz Results:</p></td></tr>", inviteSubject.Subject.Title));
                grades.Append(String.Format("<tr><td style='width: 370px; color: #245557;'><p style='margin-top:0;'><b>{0}</b></p></td>", inviteSubject.Subject.Title));
                if (inviteSubject.Subject.Quiz != null)
                {
                    foreach (var q in inviteSubject.Subject.Quiz.Questions) {
                        grades.Append(String.Format("<td style='width: 60px;'><p style='margin-top:0;'><b>Q{0}</b> <span>{1}</span></p></td>", q.SortOrder + 1, q.Response.Value == q.CorrectAnswer.AnswerID.ToString() ? gradeImgUrlRight : gradeImgUrlWrong));    
                    }
                    
                }
                else
                {
                    grades.Append(String.Format("<td style='width: 118px;'><p>(No Quiz)</p></td>", inviteSubject.Subject.FundamentalIcon));
                }
                grades.Append(String.Format("</tr></table>"));
            }
           
            var emailFrom = "no-reply@starbucksfs.com";
            var emailSubject = "Starbucks Training Results";
            MailService email = new MailService();
            string baseURL = request.Url.Scheme + "://" + request.Url.Authority + request.ApplicationPath.TrimEnd('/');
            var courseBuilderURL = baseURL + "/resources/training/CourseBuilder/";
                
            var messsage = string.Format(
                "<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>"
                + "<html xmlns='http://www.w3.org/1999/xhtml'>"
                +   "<head>"
                +       "<meta http-equiv='Content-Type' content='text/html; charset=UTF-8' />"
                +       "<title>Training Complete</title>"
                +   "</head>"
                +   "<body style='-webkit-text-size-adjust: none;'>"
                +       "<table align='center' width='600' border='0' cellspacing='0' cellpadding='0'>"
                +           "<tr>"
                +               "<td style='text-align:left;' colspan='2' bgcolor='#3E3935' height='90'>"
                +                   "<p style='margin-left:18px; margin-top:18px; margin-bottom:18px; margin-right:18px'>"
                +                       "<img src='http://marlinco.com/eblast/sbs/system_emails/branded_solutions_logo.png' width='223' height='54'/>"
                +                   "</p>"
                +               "</td>"
                +           "</tr>"
                +           "<tr>"
                +               "<td>"
                +                   "<a href='http://solutions.starbucks.com/resources/training/coursebuilder'>"
                +                       "<img src='http://marlinco.com/eblast/starbucks/su_start_15/images/02_600x130.jpg' width='600' height='130' alt='Solutions University ' style='margin-left:0px; margin-top:0px; margin-bottom:0px; margin-right:0px; font-family: arial; font-size:14px; line-height: 18px; color:#ffffff'/>"
                +                   "</a>"
                +               "</td>"
                +           "</tr>"
                +           "<tr>"
                +               "<td bgcolor='#FFFFFF' colspan='2' width='600' style='text-align:left;'>"
                +                   "<p style='text-align:left; font-family:arial; font-size:25px; font-weight:normal; color:#3E3935; margin-left:18px; margin-right:18px; margin-top:30px;'>"
                +                   "{0} has completed their Starbucks training" + (questionTotal > 0 ? ", and answered {1} out of {2} questions correctly" : string.Empty) + ".</p>"
                +                   grades.ToString()
                +                   "<p style='margin-top:18px; margin-right:0px; margin-bottom:0px; margin-left:18px;'>"
                +                       "<a href='{3}'>"
                +                           "<img src='{4}' width='256' height='35' alt='' style='border:0;' />"
                +                       "</a>"
                +                   "</p>"
                +                   "<p style='text-align:left; font-family:arial; font-size:18px; font-weight:normal; color:#3E3935; margin-left:18px; margin-right:18px; margin-bottom:30px;'>Thanks again,<br> Starbucks Branded Solutions</p>"
                +               "</td>"
                +           "</tr>"
                +       "</table>"
                +   "</body>"
                + "</html>",
                courseContent.Invite.TraineeName, answeredCorrectlyTotal.ToString(), questionTotal.ToString(), courseBuilderURL, emailImgBaseUrl + "button-return-solutions-u.jpg");
            try
            {
                email.SendEmail(trainingOperator.Email, emailFrom, emailSubject, messsage, true); 
            }
            catch (Exception e)
            {
                error = true;
            }
            return error;
        }


    }

    
    
}
