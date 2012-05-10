using System;
namespace BlissInSoftware.Sandcastle.Gherkin.Plugin
{
    interface IFeatureVisitor
    {
        string Visit(FeatureSetTopic featureSetTopic, Topic nextTopic, Topic previousTopic);
        string Visit(FeatureTopic feature, Topic nextTopic, Topic previousTopic);
    }
}
