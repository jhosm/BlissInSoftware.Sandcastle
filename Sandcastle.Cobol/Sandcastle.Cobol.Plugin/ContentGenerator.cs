using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SandcastleBuilder.Utils.BuildEngine;
using System.IO;
using System.Xml;
using System.Security.Cryptography;
using BlissInSoftware.Sandcastle.Cobol;

namespace BlissInSoftware.Sandcastle.Cobol.PlugIn
{
    class ContentGenerator
    {
        private BuildProcess builder;
        private string sourcePath;
        private HashSet<Guid> guidsInUse = new HashSet<Guid>();
        private HashAlgorithm md5;
        
        public string ContentFile { get; private set; }
        public string OutputFolder { get; private set; }
        public string TopicsIndexPath { get { return Path.Combine(OutputFolder, "cobolProgramsIndex.txt"); } }
        public List<string> TopicFilesPath { get; private set; }
        public TopicInfo RootTopic { get; private set; }

        public ContentGenerator(BuildProcess builder, string sourcePath)
        {
            this.builder = builder;
            this.sourcePath = sourcePath;
        }

        internal void Generate()
        {
            OutputFolder = Path.Combine(builder.OutputFolder, "cobolProgramsTopics");
            ContentFile = Path.Combine(OutputFolder, "cobolPrograms.content");
            BuildTopics();
            GenerateContentFile(RootTopic);
            GenerateTopicFiles();
            StringBuilder topicsIndex = GenerateTopicsIndex(new StringBuilder(), new TopicInfo[1] { RootTopic });
            builder.ReportProgress("***************TOPIC_INDEX****************");
            builder.ReportProgress(topicsIndex.ToString());
            File.WriteAllText(TopicsIndexPath, topicsIndex.ToString());
        }

        private TopicInfo BuildTopics()
        {
            using (md5 = HashAlgorithm.Create("MD5"))
            {
                RootTopic = TopicInfo.Create(Guid.NewGuid(), "API - Programas Cobol", ""); 
                BuildTopicsTree(sourcePath, RootTopic);
            }
            return RootTopic;
        }

        private void BuildTopicsTree(string parentPath, TopicInfo parentTopic)
        {
            /*
            foreach (string featureSet in Directory.EnumerateDirectories(parentPath))
            {
                builder.ReportProgress("Feature set: " + featureSet);
                Topic currentTopic = Topic.Create(TopicType.FeatureSet, CreateTopicId(featureSet), Path.GetFileName(featureSet), featureSet, gherkinFeaturesLanguage);
                parentTopic.Children.Add(currentTopic);
                BuildTopicsTree(featureSet, currentTopic);
            }
            */

            foreach (string topicFilePath in Directory.EnumerateFiles(parentPath, "*.txt"))
            {
                builder.ReportProgress("Topic file: " + topicFilePath);
                ProgramInfo currentTopic = (ProgramInfo)TopicInfo.Create(CreateTopicId(topicFilePath), Path.GetFileNameWithoutExtension(topicFilePath), topicFilePath);
                parentTopic.Children.Add(currentTopic);
            }
        }

        private Guid CreateTopicId(string topicSourcePath)
        {
            var input = Encoding.UTF8.GetBytes(topicSourcePath);
            var output = md5.ComputeHash(input);
            var guid = new Guid(output);
            while (!guidsInUse.Add(guid))
            {
                guid = Guid.NewGuid();
            }

            return guid;
        }


        private void GenerateContentFile(TopicInfo rootProgram)
        {
            builder.ReportProgress("Writing content file to: " + ContentFile + "..."); 
            
            var doc = new XmlDocument();
            var rootNode = doc.CreateElement("Topics");
            doc.AppendChild(rootNode);

            var definitionElement = doc.CreateElement("Topic");
            definitionElement.SetAttribute("id", rootProgram.TopicId.ToString());
            definitionElement.SetAttribute("visible", XmlConvert.ToString(true));
            definitionElement.SetAttribute("title", rootProgram.Title);
            rootNode.AppendChild(definitionElement);

            GenerateContentFileElements(definitionElement, rootProgram.Children);
            
            var directory = Path.GetDirectoryName(ContentFile);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            doc.Save(ContentFile);
        }

        private static void GenerateContentFileElements(XmlNode parentNode, IEnumerable<TopicInfo> definitions)
        {
            foreach (var definition in definitions)
            {
                var doc = parentNode.OwnerDocument;
                var definitionElement = doc.CreateElement("Topic");
                definitionElement.SetAttribute("id", definition.TopicId.ToString());
                definitionElement.SetAttribute("visible", XmlConvert.ToString(true));
                definitionElement.SetAttribute("title", definition.Title);
                parentNode.AppendChild(definitionElement);

                GenerateContentFileElements(definitionElement, definition.Children);
            }
        }

        private void GenerateTopicFiles()
        {
            Directory.CreateDirectory(OutputFolder);
            TopicFilesGenerator gen = new TopicFilesGenerator(builder, sourcePath, OutputFolder);
            gen.Generate(new TopicInfo[1] { RootTopic });
            TopicFilesPath = gen.GeneratedTopicFilesPath; 
        }

        private StringBuilder GenerateTopicsIndex(StringBuilder acc, IEnumerable<TopicInfo> topics)
        {
            foreach (var topic in topics)
            {
                if (topic != null && topic.TopicId != null)
                {
                    acc.AppendLine(topic.Title + "," + topic.TopicId.ToString());
                }

                acc = GenerateTopicsIndex(acc, topic.Children);
            }
            return acc;
        }



    }
}
