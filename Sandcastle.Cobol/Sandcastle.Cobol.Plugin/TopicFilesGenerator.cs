using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Resources;
using System.Reflection;
using SandcastleBuilder.Utils.BuildEngine;

namespace BlissInSoftware.Sandcastle.Cobol.PlugIn
{
    public class TopicFilesGenerator 
    {
        private string basePath;
        private BuildProcess builder;
        private string outputPath;
        public List<string> GeneratedTopicFilesPath {get; private set;}

        public TopicFilesGenerator(BuildProcess builder, string basePath, string outputPath)
        {
            this.outputPath = outputPath;
            this.builder = builder;
            this.basePath = basePath;
            GeneratedTopicFilesPath = new List<string>();
        }

        public void Generate(IEnumerable<TopicInfo> topics)
        {
            foreach (var topic in topics)
            {
                AddTopicToListOfGeneratedFiles(topic);
                CobolTopicCreator creator = new CobolTopicCreator();
                string topicContents = creator.Create(topic);
                WriteTopicFile(topic, topicContents);

                Generate(topic.Children);
            }
        }

        private void WriteTopicFile(TopicInfo topicInfo, string topicContents)
        {
            builder.ReportProgress("Writing definition contents to {0}...", topicInfo.FileName);
            File.WriteAllText(topicInfo.FileName, topicContents);
        }

        private void AddTopicToListOfGeneratedFiles(TopicInfo topicInfo)
        {
            topicInfo.FileName = GetAbsoluteFileName(outputPath, topicInfo);
            GeneratedTopicFilesPath.Add(topicInfo.FileName);

            builder.ReportProgress("Adding definition file titled '{0}' to '{1}'...", topicInfo.Title, topicInfo.FileName);
        }

        private static string GetAbsoluteFileName(string outputFolder, TopicInfo topicInfo)
        {
            return Path.Combine(outputFolder, Path.ChangeExtension(topicInfo.TopicId.ToString(), ".aml"));
        }
    }
}
