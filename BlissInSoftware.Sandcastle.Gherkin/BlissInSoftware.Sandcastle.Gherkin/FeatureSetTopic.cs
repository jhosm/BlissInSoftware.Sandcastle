using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using BlissInSoftware.Sandcastle.Gherkin.Plugin.Properties;

namespace BlissInSoftware.Sandcastle.Gherkin.Plugin
{
    internal class FeatureSetTopic : Topic
    {
        public string Introduction { get; set; }

        internal override string CreateContent()
        {
            Builder.ReportProgress("Creating topic content for '{0}'...", Title);

            string topicTemplate = System.Text.UTF8Encoding.UTF8.GetString(Resources.FeatureSetTopicTemplate);

            if (File.Exists(Path.Combine(SourcePath, "Introduction.aml")))
            {
                Introduction = File.ReadAllText(Path.Combine(SourcePath, "Introduction.aml"));
            }
            else
            {
                Introduction = Title;
            }

            IDictionary<Type, string> featureSetDocumentation = new Dictionary<Type, string>();

            IEnumerable<FeatureSetTopic> featureSetTopics = Children.OfType<FeatureSetTopic>();
            if (featureSetTopics.Count() > 0)
            {
                string featureSetTopicsContent = "";
                foreach (var topic in featureSetTopics)
                {
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
                    topic.CreateContent();
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

            return String.Format(topicTemplate, Id, Introduction, featureSetDocumentation[typeof(FeatureSetTopic)], featureSetDocumentation[typeof(FeatureTopic)]);

        }
    }
}
