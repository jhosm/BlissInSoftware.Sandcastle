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
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string Scenarios { get; set; }
        public string Rules { get; set; }
        public string GUI { get; set; }
        public string Notes { get; set; }
        private bool contentIsCreated = false;

        public override void Load()
	    {
            Feature feature;
            if (!contentIsCreated)
            {

		        SpecFlowLangParser specFlowLangParser = new SpecFlowLangParser(new CultureInfo("pt-PT"));
		        TextReader textReader = File.OpenText(SourcePath);
		        using (textReader)
		        {
			        feature = specFlowLangParser.Parse(textReader, SourcePath);
		        }
                FeatureTopicContentBuilder builder = new FeatureTopicContentBuilder(feature);

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
