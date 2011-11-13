using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using BlissInSoftware.Sandcastle.Gherkin.Plugin.Properties;
using System.Globalization;

namespace BlissInSoftware.Sandcastle.Gherkin.Plugin
{
    public class FeatureContentCreator : IFeatureVisitor
    {
        public string Visit(FeatureSetTopic featureSet)
        {
            if(!String.IsNullOrEmpty(featureSet.CustomTopic)) {
                return featureSet.CustomTopic;
            }
            string topicTemplate = System.Text.UTF8Encoding.UTF8.GetString(Resources.FeatureSetTopicTemplate);
            return String.Format(CultureInfo.CurrentCulture, topicTemplate, featureSet.Id, featureSet.Introduction, featureSet.FeatureSetTopics, featureSet.FeatureTopics);
        }

        public string Visit(FeatureTopic feature)
        {
            string topicTemplate = System.Text.UTF8Encoding.UTF8.GetString(Resources.FeatureTopicTemplate);
            return String.Format(CultureInfo.CurrentCulture, topicTemplate, feature.Id, feature.Name, feature.Summary, feature.Description, feature.Rules, feature.GUI, feature.Notes, feature.Scenarios);
        }
    }
}
