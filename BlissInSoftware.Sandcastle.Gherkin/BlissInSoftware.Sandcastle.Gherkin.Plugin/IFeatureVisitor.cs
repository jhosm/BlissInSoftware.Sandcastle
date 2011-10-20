using System;
namespace BlissInSoftware.Sandcastle.Gherkin.Plugin
{
    interface IFeatureVisitor
    {
        string Visit(FeatureSetTopic featureSet);
        string Visit(FeatureTopic feature);
    }
}
