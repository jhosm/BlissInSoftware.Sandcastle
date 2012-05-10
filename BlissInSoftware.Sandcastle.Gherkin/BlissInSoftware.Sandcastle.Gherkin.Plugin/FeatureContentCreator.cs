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

        public string Visit(FeatureSetTopic featureSet, Topic nextTopic, Topic previousTopic)
        {
            if(!String.IsNullOrEmpty(featureSet.CustomTopic)) {
                return featureSet.CustomTopic;
            }
            string topicTemplate = System.Text.UTF8Encoding.UTF8.GetString((byte[])rm.GetObject("FeatureSetTopicTemplate"));
            var previousTopicId = previousTopic == null ? featureSet.Id : previousTopic.Id;
            var nextTopicId = nextTopic == null ? featureSet.Id : nextTopic.Id;
            return String.Format(CultureInfo.CurrentCulture, topicTemplate, featureSet.Id, featureSet.Introduction, featureSet.FeatureSetTopics, featureSet.FeatureTopics, previousTopicId, nextTopicId);
        }

        public string Visit(FeatureTopic feature, Topic nextTopic, Topic previousTopic)
        {
            FeatureTemplate page = new FeatureTemplate(feature, nextTopic, previousTopic);
            return page.TransformText();
        }
    }

    public partial class FeatureTemplate
    {
        public FeatureTopic feature;
        public Topic nextTopic;
        public Topic previousTopic;
        public FeatureTemplate(FeatureTopic feature, Topic nextTopic, Topic previousTopic)
        { 
            this.feature = feature;
            this.nextTopic = nextTopic;
            this.previousTopic = previousTopic; 
        }
    }
}
