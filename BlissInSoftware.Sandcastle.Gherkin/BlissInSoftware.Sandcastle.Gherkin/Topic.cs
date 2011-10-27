using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlissInSoftware.Sandcastle.Gherkin
{
    public abstract class Topic
    {
        protected Topic()
        {
            Children = new List<Topic>();
        }

        public static Topic Create(TopicType type, string id, string title, string sourcePath)
        {
            switch(type)
            {
                case TopicType.Feature: { return new FeatureTopic() { Id = id, Type = type, Title = title, SourcePath = sourcePath }; }
                case TopicType.FeatureSet: { return new FeatureSetTopic() { Id = id, Type = type, Title = title, SourcePath = sourcePath }; }
                default: return null;
            }
        }

        public abstract void Load();
        public string Id { get; set; }
        public string Title { get; set; }
        public List<Topic> Children { get; private set; }
        public string FileName { get; set; }
        public string SourcePath { get; set; }
        public TopicType Type { get; set; }

    }
}