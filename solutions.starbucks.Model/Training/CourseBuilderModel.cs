using solutions.starbucks.Model.Masters;
using solutions.starbucks.Model.Pocos;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace solutions.starbucks.Model.Training
{
    public class CourseBuilderModel : MasterModel
    {
        public IHtmlString BodyText { get; set; }
        
        public IHtmlString LeftPath { get; set; }
        
        public IHtmlString MiddlePath { get; set; }
        
        public IHtmlString RightPath { get; set; }

        public int OperatorUmbracoUserID { get; set; }

        public IEnumerable<Subject> Subjects { get; set; }

    }


    public class Subject
    {
        public int SubjectID { get; set; }
        public string FundamentalIcon { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Url { get; set; }
        public int SortOrder { get; set; }
        public Quiz Quiz { get; set; }

        public static void SetAllQuizzes(IEnumerable<Subject> subjects, IEnumerable<Quiz> quizzes)
        {
            if (subjects != null && quizzes != null)
            {
                var join = from s in subjects
                           join q in quizzes on s.SubjectID equals q.SubjectID
                           from qOuter in quizzes.DefaultIfEmpty()
                           select new { subject = s, quiz = q };
                foreach (var joined in join)
                {
                    joined.subject.Quiz = joined.quiz;
                }
            }            
        }

    }

  
    public class Quiz
    {
        public int SubjectID { get; set; }
        public bool Graded { get; set; }
        public IEnumerable<Question> Questions { get; set; }
        public Quiz(int subjectID, IEnumerable<Question> questions)
        {
            SubjectID = subjectID;
            Questions = questions;
        }

        public bool HasResponses() {
            var resp = Responses();
            if (resp != null && resp.Count() > 0) return true; else return false;
        }

        public IEnumerable<QuizResponses> Responses()
        {
            if (this.Questions != null && this.Questions.Count() > 0)
            {
                return this.Questions.Where(q => q.Responses != null).SelectMany(q => q.Responses).ToList();
            }
            else return null;
        }

        public static void SetQuestionResponses(IEnumerable<Question> questions, IEnumerable<QuizResponses> responses)
        {
            // Group responses by QuestionId
            if (responses != null && responses.Count() > 0 && questions != null)
            {
                var responsesByQuestion = from r in responses
                                          orderby r.QuestionID
                                          group r by r.QuestionID into grp
                                          select new { key = grp.Key, responses = grp };

                // Set responses for each question
                foreach (var question in questions)
                {
                    var resp = responsesByQuestion.Where(r => r.key == question.QuestionID).SingleOrDefault();
                    if (resp != null)
                    {
                        question.Responses = resp.responses;
                    }
                }

            }
        }


    }

    public class Question
    {
        public int QuestionID { get; set; }
        public int SubjectID { get; set; }
        public string QuestionText { get; set; }
        public int SortOrder { get; set; }
        public IEnumerable<Answer> Answers { get; set; }
        public Answer CorrectAnswer { get; set; }
        public IEnumerable<QuizResponses> Responses { get; set; }
        public QuizResponses Response
        {
            get { return Responses != null ? Responses.FirstOrDefault() : null; }
        }
    }

    public class Answer
    {
        public int AnswerID { get; set; }
        public int QuestionID { get; set; }
        public string AnswerText { get; set; }
        public bool IsCorrect { get; set; }
        public int SortOrder { get; set; }
    }


  

    
}