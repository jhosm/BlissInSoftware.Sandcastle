using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using System.Text.RegularExpressions;

namespace BlissInSoftware.Sandcastle.Gherkin
{
    public class FeatureTopicContentBuilder
    {
        private Feature feature;
        private const string EXTRACT_DESCRIPTION = @"^(.*?)(?=^(?:Regras:|GUI:|Notas:):?)";
        private const string EXTRACT_RULES = @"^Regras:[\r\n]*\s*(.*?)(?=^(?:GUI:|Notas:):?)";
        private const string EXTRACT_GUI = @"^GUI:[\r\n]*\s*(.*?)(?=^(?:Notas:):?)";
        private const string EXTRACT_NOTES = @"^Notas:[\r\n]*\s*(.*)";

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
                string[] descriptionLines = new string[fullDescription.Length - 1];
                Array.ConstrainedCopy(fullDescription, 1, descriptionLines, 0, descriptionLines.Length);

                string description = string.Join(Environment.NewLine, descriptionLines);
                result = ExtractDescriptionSection(description, EXTRACT_DESCRIPTION, "");
                if (result == "") result = description;
                result = ReplaceCrLfWithBrTag(result);
            }
            else
            {
                result = "O autor desta história considera que não é necessária descrição.";
            }
            return result;
        }

        public string BuildRules()
        {
            return ExtractDescriptionSection(feature.Description, EXTRACT_RULES);
        }

        public string BuildGUI()
        {
            return ExtractDescriptionSection(feature.Description, EXTRACT_GUI);
        }

        public string BuildNotes()
        {
            return ExtractDescriptionSection(feature.Description, EXTRACT_NOTES);
        }


        private string ExtractDescriptionSection(string description, string sectionExtractionRules)
        {
            return ExtractDescriptionSection(description, sectionExtractionRules, "Não aplicável no contexto desta história.");
        }

        private string ExtractDescriptionSection(string description, string sectionExtractionRules, string defaultResult)
        {
            string result = defaultResult;
            Match rulesMatch = Regex.Match(description, sectionExtractionRules, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);
            if (rulesMatch.Groups[1].Captures.Count > 0) result = rulesMatch.Groups[1].Captures[0].Value;
            return ReplaceCrLfWithBrTag(result);
        }

        public string BuildBackground()
        {
            string result = "";
            Background background = feature.Background;
            if (background != null)
            {
                result += "Contexto: ";
                result = BuildSteps(result, background);
            }
            return result;
        }

        public string BuildScenarios()
        {
            string result = "";
            foreach (var scenario in feature.Scenarios)
            {
                if (scenario.Tags != null && scenario.Tags.Count > 0)
                {
                        result = InsertTags(result, scenario.Tags) + Environment.NewLine;
                }

                result += "Cenário: ";
                result = BuildSteps(result, scenario);
                result += Environment.NewLine;
            }
            if (!String.IsNullOrEmpty(result)) result = result.Remove(result.Length - (Environment.NewLine.Length) * 2); 
            return result;
        }

        private static string BuildSteps(string result, dynamic scenario)
        {
            result += scenario.Title + Environment.NewLine;
            foreach (ScenarioStep step in scenario.Steps)
            {
                result += step.Keyword + step.Text + Environment.NewLine;
            }
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

