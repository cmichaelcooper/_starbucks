using solutions.starbucks.Model.Masters;
using solutions.starbucks.Model.Pocos;
using solutions.starbucks.Model.Training;
using System.Collections.Generic;
using System.Web;

namespace solutions.starbucks.Model
{
    public class CourseViewerModel : MasterModel
    {
        public IHtmlString BodyText { get; set; }
        
        public IHtmlString LeftPath { get; set; }
        
        public IHtmlString MiddlePath { get; set; }
        
        public IHtmlString RightPath { get; set; }

        public Invites Invite { get; set; }

        public IEnumerable<Subject> Subjects { get; set; }

        public bool TrainingComplete { get; set; }

    }
        
}