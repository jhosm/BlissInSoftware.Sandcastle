﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BlissInSoftware.Sandcastle.Gherkin
{
    public class FeatureSetTopic : Topic
    {
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
            Builder.ReportProgress("Creating topic content for '{0}'...", Title);

            featureSetDocumentation = new Dictionary<Type, string>();

            if (File.Exists(Path.Combine(SourcePath, "Introduction.aml")))
            {
                Introduction = File.ReadAllText(Path.Combine(SourcePath, "Introduction.aml"));
            }
            else
            {
                Introduction = Title;
            }


            IEnumerable<FeatureSetTopic> featureSetTopics = Children.OfType<FeatureSetTopic>();
            if (featureSetTopics.Count() > 0)
            {
                string featureSetTopicsContent = "";
                foreach (var topic in featureSetTopics)
                {
                    topic.Load();
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
                    topic.Load();
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
    }
}
