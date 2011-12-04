using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Resources;
using System.Reflection;

namespace BlissInSoftware.Sandcastle.Gherkin.Plugin
{
    public class FeatureContentCreator : IFeatureVisitor
    {
        ResourceManager rm = new ResourceManager("BlissInSoftware.Sandcastle.Gherkin.Plugin.Resources", Assembly.GetExecutingAssembly());

        public string Visit(FeatureSetTopic featureSet)
        {
            if(!String.IsNullOrEmpty(featureSet.CustomTopic)) {
                return featureSet.CustomTopic;
            }
            string topicTemplate = System.Text.UTF8Encoding.UTF8.GetString((byte[])rm.GetObject("FeatureSetTopicTemplate"));
            return String.Format(CultureInfo.CurrentCulture, topicTemplate, featureSet.Id, featureSet.Introduction, featureSet.FeatureSetTopics, featureSet.FeatureTopics);
        }

        public string Visit(FeatureTopic feature)
        {
            string topicTemplate = System.Text.UTF8Encoding.UTF8.GetString((byte[])rm.GetObject("FeatureTopicTemplate"));
            return String.Format(CultureInfo.CurrentCulture, topicTemplate, feature.Id, feature.Name, feature.Summary, feature.Description, feature.Rules, feature.GUI, feature.Notes, feature.Scenarios);
        }
    }
}
