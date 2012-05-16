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
            if(featureSet.CustomTopic != null) {
                FeatureTemplate featureTemplate = new FeatureTemplate(featureSet.CustomTopic, nextTopic, previousTopic);
                return featureTemplate.TransformText(); 
            }
            FeatureSetTemplate featureSetTemplate = new FeatureSetTemplate(featureSet, nextTopic, previousTopic);
            return featureSetTemplate.TransformText();
        }

        public string Visit(FeatureTopic feature, Topic nextTopic, Topic previousTopic)
        {
            FeatureTemplate page = new FeatureTemplate(feature, nextTopic, previousTopic);
            return page.TransformText();
        }
    }

    public partial class FeatureSetTemplate
    {
        public FeatureSetTopic featureSet;
        public Topic nextTopic;
        public Topic previousTopic;
        public FeatureSetTemplate(FeatureSetTopic featureSet, Topic nextTopic, Topic previousTopic)
        {
            this.featureSet = featureSet;
            this.nextTopic = nextTopic;
            this.previousTopic = previousTopic;
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
