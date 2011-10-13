using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BlissInSoftware.Sandcastle.Gherkin.Plugin.Properties;
using System.IO;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using System.Globalization;

namespace BlissInSoftware.Sandcastle.Gherkin.Plugin
{
    internal class FeatureTopic : Topic
    {
        public string Name { get; set; }
        public string Summary { get; set; }
        protected string Description { get; set; }
        protected string Scenarios { get; set; }
        protected string Rules { get; set; }
        protected string GUI { get; set; }
        protected string Notes { get; set; }
        private bool contentIsCreated = false;

        internal override string CreateContent()
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
            string topicTemplate = System.Text.UTF8Encoding.UTF8.GetString(Resources.FeatureTopicTemplate);
            return String.Format(topicTemplate, Id, Name, Summary, Description, Rules, GUI, Notes, Scenarios);
	    }
    }
}
