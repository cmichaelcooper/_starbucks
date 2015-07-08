using solutions.starbucks.Model.Pocos;
using solutions.starbucks.Model.Training;
using System.Collections.Generic;
using Umbraco.Core.Models;

namespace solutions.starbucks.Interfaces
{
    public interface ICourseBuilderRepository
    {
        IPublishedContent SubjectsParent(IPublishedContent rootContent);
        IEnumerable<IPublishedContent> SubjectsContentsFromRoot(IPublishedContent rootContent);
        IEnumerable<Subject> Subjects(IPublishedContent content, List<int> subjectIds = null);
        IEnumerable<Subject> SubjectsFromRoot(IPublishedContent rootContent, List<int> subjectIds = null);
        Quiz SubjectQuiz(IPublishedContent content, IEnumerable<QuizResponses> responses = null);
        
    }
}