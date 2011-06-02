using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SandcastleBuilder.Utils.BuildEngine;
using System.IO;
using System.Xml;
using BlissInSoftware.Sandcastle.Gherkin.Plugin.Properties;

namespace BlissInSoftware.Sandcastle.Gherkin.Plugin
{
    class ContentGenerator
    {
        private BuildProcess builder;
        private string gherkinFeaturesPath;

        public string ContentFile { get; private set; }
        public string TopicsFolder { get; private set; }
        public List<string> TopicFiles { get; private set; }
        public Topic RootTopic { get; private set; }
        
        public ContentGenerator(BuildProcess builder, string gherkinFeaturesPath)
        {
            this.builder = builder;
            this.gherkinFeaturesPath = gherkinFeaturesPath;
        }

        internal void Generate()
        {
            TopicsFolder = Path.Combine(builder.OutputFolder, "gherkinFeaturesTopics");
            ContentFile = Path.Combine(TopicsFolder, "gherkinFeatures.content");
            BuildTopics();
            GenerateContentFile(RootTopic);
            GenerateTopicFiles();
         
        }

        private Topic BuildTopics()
        {
            RootTopic = Topic.Create(builder, TopicType.FeatureSet, Guid.NewGuid().ToString(), "Features", gherkinFeaturesPath);
            BuildTopicsTree(gherkinFeaturesPath, RootTopic);
            return RootTopic;
        }

        private void BuildTopicsTree(string parentPath, Topic parentTopic)
        {
            foreach (string featureSet in Directory.EnumerateDirectories(parentPath))
            {
                builder.ReportProgress("Feature set: " + featureSet);
                Topic currentTopic = Topic.Create(builder, TopicType.FeatureSet, Guid.NewGuid().ToString(),Path.GetFileName(featureSet), featureSet);
                parentTopic.Children.Add(currentTopic);
                BuildTopicsTree(featureSet, currentTopic);
            }

            foreach (string featureFile in Directory.EnumerateFiles(parentPath, "*.feature"))
            {
                builder.ReportProgress("Feature: " + featureFile);
                Topic currentTopic = Topic.Create(builder, TopicType.Feature, Guid.NewGuid().ToString(), Path.GetFileNameWithoutExtension(featureFile), featureFile);
                parentTopic.Children.Add(currentTopic);
            }
        }

        private void GenerateContentFile(Topic RootTopic)
        {
            builder.ReportProgress("Writing content file to: " + ContentFile + "..."); 
            
            var doc = new XmlDocument();
            var rootNode = doc.CreateElement("Topics");
            doc.AppendChild(rootNode);

            var topicElement = doc.CreateElement("Topic");
            topicElement.SetAttribute("id", RootTopic.Id);
            topicElement.SetAttribute("visible", XmlConvert.ToString(true));
            topicElement.SetAttribute("title", RootTopic.Title);
            rootNode.AppendChild(topicElement);

            GenerateContentFileElements(topicElement, RootTopic.Children);

            var directory = Path.GetDirectoryName(ContentFile);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            doc.Save(ContentFile);
        }

        private static void GenerateContentFileElements(XmlNode parentNode, IEnumerable<Topic> topics)
        {
            foreach (var topic in topics)
            {
                var doc = parentNode.OwnerDocument;
                var topicElement = doc.CreateElement("Topic");
                topicElement.SetAttribute("id", topic.Id);
                topicElement.SetAttribute("visible", XmlConvert.ToString(true));
                topicElement.SetAttribute("title", topic.Title);
                parentNode.AppendChild(topicElement);

                GenerateContentFileElements(topicElement, topic.Children);
            }
        }

        private void GenerateTopicFiles()
        {
            Directory.CreateDirectory(TopicsFolder);
            TopicFiles = new List<string>();
            AddTopicFile(RootTopic);
            string topicContents = RootTopic.CreateContent();
            WriteTopicFile(RootTopic, topicContents);

            GenerateTopicFiles(RootTopic.Children);
        }

        private void GenerateTopicFiles(IEnumerable<Topic> topics)
        {
            foreach (var topic in topics)
            {
                AddTopicFile(topic);

                string topicContents = topic.CreateContent(); 
                WriteTopicFile(topic, topicContents);

                GenerateTopicFiles(topic.Children);
            }
        }

        private void WriteTopicFile(Topic topic, string topicContents)
        {
            builder.ReportProgress("Writing topic contents to {0}...", topic.FileName);
            File.WriteAllText(topic.FileName, topicContents);
        }

        private void AddTopicFile(Topic topic)
        {
            topic.FileName = GetAbsoluteFileName(TopicsFolder, topic);
            TopicFiles.Add(topic.FileName);

            builder.ReportProgress("Adding topic file titled '{0}' to '{1}'...", topic.Title, topic.FileName);
        }

        private string GetAbsoluteFileName(string topicsFolder, Topic topic)
        {
            return Path.Combine(topicsFolder, Path.ChangeExtension(topic.Id, ".aml"));
        }

    }
}
