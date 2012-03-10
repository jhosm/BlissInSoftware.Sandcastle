using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BlissInSoftware.Sandcastle.Cobol
{
    public class TopicInfo
    {
        public string FileName { get; set; }

        public string Title { get; set; }

        public Guid TopicId { get; set; }

        public IList<TopicInfo> Children { get; set; }

        public TopicInfo()
        {
            Children = new List<TopicInfo>();
        }

        public static TopicInfo Create(Guid topicId, string title, string topicPath)
        {
            TopicInfo result;
            if (String.IsNullOrEmpty(topicPath))
            {
                result = new TopicInfo();
            }
            else
            {
                Parser parser = new Parser(File.ReadAllText(topicPath, Encoding.Default));
                result = parser.Parse();

            }

            result.TopicId = topicId;
            result.Title = title;

            return result;
        }
    }
}
