using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using System.Text.RegularExpressions;
using gherkin;
using System.Resources;
using System.Reflection;
using System.Globalization;

namespace BlissInSoftware.Sandcastle.Gherkin
{
    public class FeatureTopicContentBuilder
    {
        private Feature feature;
        private const string EXTRACT_DESCRIPTION = @"^(.*?)(?=(?:\r\nRegras:|\r\nGUI:|\r\nNotas:|$))";
        private const string EXTRACT_RULES = @"(?:^|\n)Regras:[\r\n]*\s*(.*?)(?=(?:\r\nGUI:|\r\nNotas:|$))";
        private const string EXTRACT_GUI = @"(?:^|\n)GUI:[\r\n]*\s*(.*?)(?=(?:\r\nNotas:|$))";
        private const string EXTRACT_NOTES = @"(?:^|\n)Notas:[\r\n]*\s*(.*)";

        ResourceManager rm = new ResourceManager("BlissInSoftware.Sandcastle.Gherkin.Resources", Assembly.GetExecutingAssembly());
        private CultureInfo cultureInfo;

        public FeatureTopicContentBuilder(Feature feature)
        {
            this.cultureInfo = CultureInfo.CurrentCulture;
        }

        public FeatureTopicContentBuilder(Feature feature, CultureInfo cultureInfo)
        {
            this.feature = feature;
            this.cultureInfo = cultureInfo;
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

                result = ExtractDescriptionSection(String.Join(Environment.NewLine, descriptionLines));
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
            return ExtractDescriptionSection(feature.Description, "Regras:");
        }

        public string BuildGUI()
        {
            return ExtractDescriptionSection(feature.Description, "GUI:");
        }

        public string BuildNotes()
        {
            return ExtractDescriptionSection(feature.Description, "Notas:");
        }


        private string ExtractDescriptionSection(string description)
        {
            return ExtractDescriptionSection(description, null);
        }

        private string ExtractDescriptionSection(string description, string subsectionName)
        {
            string[] descriptionLines = description.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            string result = "";
            string[] subsectionNames = rm.GetString("FeatureTopicContent_SubSectionNames", cultureInfo).Split(';');
            int subsectionFirstLine = -1;
            for (int i = 0; i < descriptionLines.Length; i++)
            {
                if (subsectionName == null)
                {
                    subsectionFirstLine = i;
                    break;
                }
                if (subsectionNames.Any(x => descriptionLines[i].StartsWith(subsectionName)))
                {
                    subsectionFirstLine = i + 1;
                    break;
                }
            }
            if (subsectionFirstLine == -1) return ReplaceCrLfWithBrTag(result);


            for (int i = subsectionFirstLine; i < descriptionLines.Length; i++)
            {
                var descriptionLine = descriptionLines[i];
                if (subsectionNames.Any(x => descriptionLine.StartsWith(x))) break;
                result += descriptionLine; 
                if(i != descriptionLines.Length - 1) result += Environment.NewLine;
            }
            while (result.EndsWith(Environment.NewLine))
            {
                result = result.Remove(result.Length - Environment.NewLine.Length);
            }
            return ReplaceCrLfWithBrTag(result);
        }

        public string BuildBackground()
        {
            string result = "";
            Background background = feature.Background;
            if (background != null)
            {
                result += feature.Background.Keyword + ": ";
                result = BuildSteps(result, background.Title, background.Steps);
            }
            return result + Environment.NewLine;
        }

        public string BuildScenarios()
        {
            string result = "";
            foreach (Scenario scenario in feature.Scenarios)
            {
                if (scenario.Tags != null && scenario.Tags.Count > 0)
                {
                        result = InsertTags(result, scenario.Tags) + Environment.NewLine;
                }

                result += scenario.Keyword + ": ";
                result = BuildSteps(result, scenario.Title, scenario.Steps);
                ScenarioOutline scenarioOutline = scenario as ScenarioOutline;
                if (scenarioOutline != null)
                {
                    result = BuildScenarioOutline(result, scenarioOutline);
                }
               
                result += Environment.NewLine;
            }
            if (!String.IsNullOrEmpty(result)) result = result.Remove(result.Length - (Environment.NewLine.Length) * 2); 
            return result;
        }

        private string BuildScenarioOutline(string result, ScenarioOutline scenarioOutline)
        {
            result += Environment.NewLine;
            foreach (var example in scenarioOutline.Examples.ExampleSets)
            {
                result += "    " + example.Keyword + ": " + example.Title + Environment.NewLine;
                result = BuildTable(result, example.Table);
                result += Environment.NewLine;
            }
            return result;
        }

        private string BuildSteps(string result, string title, ScenarioSteps steps)
        {
            result += title + Environment.NewLine;
            foreach (ScenarioStep step in steps)
            {
                result += "    " + step.Keyword + step.Text + Environment.NewLine;

                if (step.TableArg != null)
                {
                    result = BuildTable(result, step.TableArg);
                }
                if (step.MultiLineTextArgument != null)
                {
                    result += 
                        "        \"\"\"" + Environment.NewLine + 
                        "        " + step.MultiLineTextArgument.Replace(Environment.NewLine, Environment.NewLine + "        ") + Environment.NewLine +
                        "        \"\"\"" + Environment.NewLine;
                }
            }
            return result;
        }

        private string BuildTable(string result, GherkinTable table)
        {
            Dictionary<int, int> columnsMaxLength = FindEachColumnMaxLength(table);

            result = BuildTableRow(result, table.Header.Cells, columnsMaxLength);
            foreach (var row in table.Body)
            {
                result = BuildTableRow(result, row.Cells, columnsMaxLength);
            }
            return result;
        }

        private Dictionary<int, int> FindEachColumnMaxLength(GherkinTable table)
        {
            Dictionary<int, int> result = new Dictionary<int,int>();
            for (int i = 0; i < table.Header.Cells.Length; i++)
            {
                result[i] = table.Header.Cells[i].Value.Length;
                foreach (var row in table.Body)
                {
                    var cellLength = row.Cells[i].Value.Length;
                    if (cellLength > result[i])
                    {
                        result[i] = cellLength; 
                    }
                }
            }
            return result;
        }

        private string BuildTableRow(string result, GherkinTableCell[] row, Dictionary<int, int> columnsMaxLength)
        {
            result += "        ";
            for (int i = 0; i < row.Length; i++)
            {
                var cell = row[i];
                result += "|" + cell.Value.PadRight(columnsMaxLength[i] + 1);
            }
            result += "|" + Environment.NewLine;
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

