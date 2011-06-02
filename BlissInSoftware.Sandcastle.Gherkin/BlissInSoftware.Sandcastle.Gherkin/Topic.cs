using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlissInSoftware.Sandcastle.Gherkin.Plugin.Properties;
using SandcastleBuilder.Utils.BuildEngine;

namespace BlissInSoftware.Sandcastle.Gherkin.Plugin
{
    internal abstract class Topic
    {
        protected BuildProcess Builder {get; set;}

        protected Topic()
        {
            Children = new List<Topic>();
        }

        internal static Topic Create(BuildProcess builder, TopicType type, string id, string title, string sourcePath)
        {
            switch(type)
            {
                case TopicType.Feature: { return new FeatureTopic() { Builder = builder, Id = id, Type = type, Title = title, SourcePath = sourcePath }; }
                case TopicType.FeatureSet: { return new FeatureSetTopic() { Builder = builder, Id = id, Type = type, Title = title, SourcePath = sourcePath }; }     
            }

            return null;
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public List<Topic> Children { get; private set; }
        public string FileName { get; set; }
        public string SourcePath { get; set; }
        public TopicType Type { get; set; }

        internal abstract string CreateContent();
    }
}