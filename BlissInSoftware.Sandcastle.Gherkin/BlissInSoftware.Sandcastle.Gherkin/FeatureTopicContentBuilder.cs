using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace BlissInSoftware.Sandcastle.Gherkin.Plugin
{
    public class FeatureTopicContentBuilder
    {
        private Feature feature;

        public FeatureTopicContentBuilder(Feature feature)
        {
            this.feature = feature;
        }

        public string BuildName()
        {
            string result = "";
            result += InsertTags(result, feature.Tags) + Environment.NewLine;
            result += feature.Title;
            result = ReplaceCrLfWithBrTag(result);
            return result;
        }

        public string BuildSummary()
        {
            string[] fullDescription = SplitDescription();
            return ReplaceCrLfWithBrTag(fullDescription[0]);
        }

        public string BuildDescription()
        {
            string[] fullDescription = SplitDescription();
            string result;
            if (fullDescription.Length > 1)
            {
                string[] description = new string[fullDescription.Length - 1];
                Array.ConstrainedCopy(fullDescription, 1, description, 0, description.Length);
                result = ReplaceCrLfWithBrTag(string.Join(Environment.NewLine, description));
            }
            else
            {
                result = "O autor desta história considera que não é necessária descrição.";
            }
            return result;
        }

        public string BuildBackground()
        {
            string result = "";
            Background background = feature.Background;
            if (background != null)
            {
                foreach (ScenarioStep step in background.Steps)
                {
                    result += step.Keyword + step.Text + Environment.NewLine;
                }
            }
            return result;
        }

        public string BuildScenarios()
        {
            string result = "";
            foreach (var scenario in feature.Scenarios)
            {
                if (scenario.Tags != null)
                {
                    if (scenario.Tags.Count > 0)
                    {
                        result = InsertTags(result, scenario.Tags) + Environment.NewLine;
                    }
                }

                foreach (ScenarioStep scenarioStep in scenario.Steps)
                {
                    result += scenarioStep.Keyword + scenarioStep.Text + Environment.NewLine;
                }
                result += Environment.NewLine;
            }
            if (!String.IsNullOrEmpty(result)) result = result.Remove(result.Length - (Environment.NewLine.Length) * 2); 
            return result;
        }

        private string[] SplitDescription()
        {
            return feature.Description.Split(new string[] { Environment.NewLine + Environment.NewLine }, StringSplitOptions.None);
        }


        private string InsertTags(string result, Tags tags)
        {
            if (tags == null)
            {
                return result;
            }
            foreach (Tag tag in tags)
            {
                result += "@" + tag.Name + ", ";
            }
            if (tags.Count > 0)
            {
                result = result.Substring(0, result.Length - 2);
            }
            return result;
        }

        private string ReplaceCrLfWithBrTag(string input)
        {
            return input.Replace(Environment.NewLine, "<markup><br /></markup>");
        }
    }
}
