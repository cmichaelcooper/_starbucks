using System.Collections.Generic;
namespace solutions.starbucks.Model.Training
{
    public class TrainingModulesrModel
    {
        
    }

    public class Video
    {
        public string VideoEmbed { get; set; }

        public Video(string videoEmbed)
        {
            VideoEmbed = videoEmbed;
        }
    }

    public class Pdf
    {
        public string FileChooser { get; set; }
        public string SpanishFileChooser { get; set; }

        public Pdf(string fileChooser, string spanishFileChooser)
        {
            FileChooser = fileChooser;
            SpanishFileChooser = spanishFileChooser;
        }
    }

    public class Module
    {
        public enum Types {
            Video = 1,
            Pdf = 2
        };

        public int Id { get; set; }
        public Types Type { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public List<string> CategoryNames { get; set; }
        public int SortOrder { get; set; }
        public Video Video { get; set; }
        public Pdf Pdf { get; set; }
        
        public static Types GetTypeFromAlias(string documentTypeAlias)
        {
            Types type = Types.Pdf;
            switch (documentTypeAlias)
            {
                case "VideoModule": type = Types.Video;
                    break;
                case "DownloadModule": type = Types.Pdf;
                    break;
            }
            return type;
        }

        


    }

    public class Category
    {
        public int CategoryID { get; set; }
        public string Name { get; set; }

        public Category(int categoryId, string name)
        {
            CategoryID = categoryId;
            Name = name;
        }
    }



    
}