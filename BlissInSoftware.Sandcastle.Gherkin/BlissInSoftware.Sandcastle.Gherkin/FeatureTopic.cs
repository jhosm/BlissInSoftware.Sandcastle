using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using System.Globalization;

namespace BlissInSoftware.Sandcastle.Gherkin
{
    public class FeatureTopic : Topic
    {
        private const string USER_STORY_ID_TAG_PREFIX = "HU_";

        public string Name { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string Scenarios { get; set; }
        public string Rules { get; set; }
        public string GUI { get; set; }
        public string Notes { get; set; }
        public string UserStoryId { get; set; }
        
        private bool contentIsCreated = false;
        private string unparsedFeature;

        public FeatureTopic() { }

        public FeatureTopic(string unparsedFeature)
        {
            this.unparsedFeature = unparsedFeature;
        }

        public override void Load()
	    {
            Feature feature;
            if (!contentIsCreated)
            {

		        SpecFlowLangParser specFlowLangParser = new SpecFlowLangParser(Language);
                TextReader textReader;
                if (String.IsNullOrEmpty(unparsedFeature))
                {
                    textReader = File.OpenText(SourcePath);
                }
                else
                {
                    textReader = new StringReader(unparsedFeature);
                }
                
                using (textReader)
                {
                    feature = specFlowLangParser.Parse(textReader, SourcePath);
                }

                if (feature.Tags != null && feature.Tags.Count > 0)
                {
                    Tag userStoryIdTag = feature.Tags.Find(aTag => { return aTag.Name.StartsWith(USER_STORY_ID_TAG_PREFIX); });
                    if (userStoryIdTag != null)
                    {
                        UserStoryId = userStoryIdTag.Name.Substring(USER_STORY_ID_TAG_PREFIX.Length);
                    }
                }

                FeatureTopicContentBuilder builder = new FeatureTopicContentBuilder(feature, Language);

                Name = builder.BuildName();
                Summary = builder.BuildSummary();
                Description = builder.BuildDescription();
                Rules = builder.BuildRules();
                GUI = builder.BuildGUI();
                Notes = builder.BuildNotes();
                Scenarios = builder.BuildBackground();
                Scenarios += builder.BuildScenarios();

                contentIsCreated = true;
            }
	    }
    }
}
