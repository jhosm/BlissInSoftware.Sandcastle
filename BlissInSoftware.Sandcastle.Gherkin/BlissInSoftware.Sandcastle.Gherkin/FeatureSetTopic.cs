using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using TechTalk.SpecFlow.Parser;

namespace BlissInSoftware.Sandcastle.Gherkin
{
    public class FeatureSetTopic : Topic
    {
        public FeatureTopic CustomTopic { get; set; }
        public string Introduction { get; set; }

        IDictionary<Type, string> featureSetDocumentation = new Dictionary<Type, string>();
        public string FeatureSetTopics
        {
            get
            {
                return featureSetDocumentation[typeof(FeatureSetTopic)];
            }
        }
        public string FeatureTopics
        {
            get
            {
                return featureSetDocumentation[typeof(FeatureTopic)];
            }
        }


        public override void Load()
        {
            Introduction = "";
            string customTopicPath = Path.Combine(SourcePath, "index.feature");
            if (File.Exists(customTopicPath))
            {
                CustomTopic = (FeatureTopic)Topic.Create(TopicType.Feature, Id, Title, customTopicPath, Language);
                CustomTopic.Load();
            }

            featureSetDocumentation = new Dictionary<Type, string>();

            IEnumerable<FeatureSetTopic> featureSetTopics = Children.OfType<FeatureSetTopic>();
            if (featureSetTopics.Count() > 0)
            {
                string featureSetTopicsContent = "";
                foreach (var topic in featureSetTopics)
                {
                    LoadChildTopic(topic);
                    
                    featureSetTopicsContent += String.Format("<listItem><para>{0}</para></listItem>", topic.Title);
                }
                featureSetDocumentation.Add(typeof(FeatureSetTopic), featureSetTopicsContent);
            }

            IEnumerable<FeatureTopic> featureTopics = Children.OfType<FeatureTopic>();
            if (featureTopics.Count() > 0)
            {
                string featureTopicContents = "<definitionTable>";
                foreach (var topic in featureTopics)
                {
                    LoadChildTopic(topic);
                    featureTopicContents += "<definedTerm>" + topic.Name + "</definedTerm>";
                    featureTopicContents += "<definition>" + topic.Summary + "</definition>";
                }
                featureTopicContents += "</definitionTable>";
                featureSetDocumentation.Add(typeof(FeatureTopic), featureTopicContents);
            }

            Type[] topicTypes = new Type[] { typeof(FeatureSetTopic), typeof(FeatureTopic) };
            foreach (var topicType in topicTypes)
            {
                if (!featureSetDocumentation.ContainsKey(topicType))
                {
                    featureSetDocumentation.Add(topicType, "<listItem><para>Não existem items.</para></listItem>");
                }
            }


        }

        private static void LoadChildTopic(Topic topic)
        {
            try
            {
                topic.Load();
            }
            catch (SpecFlowParserException ex)
            {
                Exception newEx = new Exception(topic.SourcePath + ": " + ex.Message, ex);
                throw newEx;
            }
        }
    }
}
