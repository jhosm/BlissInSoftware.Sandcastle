using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Parser;
using System.IO;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using System.Globalization;

namespace BlissInSoftware.Sandcastle.Gherkin.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string SourcePath = @"G:\_Tools\BlissInSoftware.Sandcastle\BlissInSoftware.Sandcastle.Gherkin\BlissInSoftware.Sandcastle.Gherkin.UnitTests\FeatureWithMultilineTextArgument.feature";
            SpecFlowLangParser specFlowLangParser = new SpecFlowLangParser(new CultureInfo("en-US"));
            TextReader textReader;
            textReader = File.OpenText(SourcePath);
            Feature feature;
            using (textReader)
            {
                try
                {
                    feature = specFlowLangParser.Parse(textReader, SourcePath);
                }
                catch (SpecFlowParserException ex)
                {
                    throw new Exception(SourcePath + "\r\n" + ex.Message, ex);
                }
            }

            FeatureTopicContentBuilder build = new FeatureTopicContentBuilder(feature, new CultureInfo("en-US")); ;
        }
    }
}
