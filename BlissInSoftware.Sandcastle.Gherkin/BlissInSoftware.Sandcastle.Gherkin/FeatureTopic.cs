using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BlissInSoftware.Sandcastle.Gherkin.Plugin.Properties;
using System.IO;

namespace BlissInSoftware.Sandcastle.Gherkin.Plugin
{
    internal class FeatureTopic: Topic
    {
        internal const string regexPatternIntroduction = @"^.*Funcionalidade:([^\r]+)\r\n(.+?)\r\n\s*\r\n(.+?)Cenário:";
        internal const string regexPatternXmlTagHeuristic = @"^\s*<";
                                                           
        internal const string regexPatternScenarios = "(Cenário.*)";

        public string Name { get; set; }
        public string Summary { get; set; }
        protected string Description { get; set; }
        protected string Scenarios { get; set; }
        private bool contentIsCreated = false;

        internal override string CreateContent()
        {
            if (!contentIsCreated)
            {
                string featureContent = File.ReadAllText(SourcePath);

                Name = "";
                Summary = "";
                Match matchForIntroduction = new Regex(regexPatternIntroduction, RegexOptions.Singleline).Match(featureContent);
                if (matchForIntroduction != null)
                {
                    Name = matchForIntroduction.Groups[1].Value;
                    Summary = matchForIntroduction.Groups[2].Value.Replace("\r\n", "<markup><br /></markup>");
                    if (matchForIntroduction.Groups[3].Value.Length > 0)
                    {
                        string[] splittedDescription = matchForIntroduction.Groups[3].Value.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                        Description = splittedDescription.
                            Select(para => Regex.IsMatch(para, regexPatternXmlTagHeuristic) ? para : "<para>" + para + "</para>").
                            Aggregate((result, para) => result + para);
                    }
                    else
                    {
                        Description = "O autor desta história considera que não é necessária descrição.";
                    }
                }
                else
                {
                    throw new Exception(String.Format("Did not find introduction for topic {0}. Check your feature content structure...", Title));
                }
                
                Scenarios = "";
                Match matchForScenarios = new Regex(regexPatternScenarios, RegexOptions.Singleline).Match(featureContent);
                if (matchForScenarios != null)
                {
                    Scenarios = matchForScenarios.Groups[1].Value;
                }
                else
                {
                    throw new Exception(String.Format("Did not find scenarios for topic {0}. Check your feature content structure...", Title));
                }

                contentIsCreated = true;
            }
            string topicTemplate = System.Text.UTF8Encoding.UTF8.GetString(Resources.FeatureTopicTemplate);
            return String.Format(topicTemplate, Id, Name, Summary, Description, Scenarios);
        }
    }
}
