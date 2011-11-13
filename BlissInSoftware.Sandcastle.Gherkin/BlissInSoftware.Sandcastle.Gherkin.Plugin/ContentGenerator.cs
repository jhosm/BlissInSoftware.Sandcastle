using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SandcastleBuilder.Utils.BuildEngine;
using System.IO;
using System.Xml;
using BlissInSoftware.Sandcastle.Gherkin.Plugin.Properties;
using System.Security.Cryptography;
using BlissInSoftware.Sandcastle.Gherkin;

namespace BlissInSoftware.Sandcastle.Gherkin.Plugin
{
    class ContentGenerator
    {
        private BuildProcess builder;
        private string gherkinFeaturesPath;
        private HashSet<Guid> guidsInUse;
        private HashAlgorithm md5;

        public string ContentFile { get; private set; }
        public string TopicsFolder { get; private set; }
        public string TopicIndexPath { get { return Path.Combine(TopicsFolder, "topicIndex.txt"); } }
        public List<string> TopicFiles { get; private set; }
        public Topic RootTopic { get; private set; }
        
        public ContentGenerator(BuildProcess builder, string gherkinFeaturesPath)
        {
            this.builder = builder;
            this.gherkinFeaturesPath = gherkinFeaturesPath;
            this.guidsInUse = new HashSet<Guid>();
        }

        internal void Generate()
        {
            TopicsFolder = Path.Combine(builder.OutputFolder, "gherkinFeaturesTopics");
            ContentFile = Path.Combine(TopicsFolder, "gherkinFeatures.content");
            BuildTopics();
            GenerateContentFile(RootTopic);
            GenerateTopicFiles();
            StringBuilder topicIndex = GenerateTopicIndex(new StringBuilder(), new Topic[1] { RootTopic });
            File.WriteAllText(TopicIndexPath, topicIndex.ToString());
        }

        private Topic BuildTopics()
        {
            using (md5 = HashAlgorithm.Create("MD5"))
            {

                RootTopic = Topic.Create(TopicType.FeatureSet, CreateTopicId("Features"), "Features", gherkinFeaturesPath);
                BuildTopicsTree(gherkinFeaturesPath, RootTopic);
            }
            RootTopic.Load();
            return RootTopic;
        }

        private void BuildTopicsTree(string parentPath, Topic parentTopic)
        {
            foreach (string featureSet in Directory.EnumerateDirectories(parentPath))
            {
                builder.ReportProgress("Feature set: " + featureSet);
                Topic currentTopic = Topic.Create(TopicType.FeatureSet, CreateTopicId(featureSet),Path.GetFileName(featureSet), featureSet);
                parentTopic.Children.Add(currentTopic);
                BuildTopicsTree(featureSet, currentTopic);
            }

            foreach (string featureFile in Directory.EnumerateFiles(parentPath, "*.feature"))
            {
                builder.ReportProgress("Feature: " + featureFile);
                Topic currentTopic = Topic.Create(TopicType.Feature, CreateTopicId(featureFile), Path.GetFileNameWithoutExtension(featureFile), featureFile);
                parentTopic.Children.Add(currentTopic);
            }
        }

        private string CreateTopicId(string topicSourcePath) 
        {
            var input = Encoding.UTF8.GetBytes(topicSourcePath);
            var output = md5.ComputeHash(input);
            var guid = new Guid(output);
            while (!guidsInUse.Add(guid))
                guid = Guid.NewGuid();

            string result = guid.ToString();
            
            return result;

        }

        private void GenerateContentFile(Topic rootTopic)
        {
            builder.ReportProgress("Writing content file to: " + ContentFile + "..."); 
            
            var doc = new XmlDocument();
            var rootNode = doc.CreateElement("Topics");
            doc.AppendChild(rootNode);

            var topicElement = doc.CreateElement("Topic");
            topicElement.SetAttribute("id", rootTopic.Id);
            topicElement.SetAttribute("visible", XmlConvert.ToString(true));
            topicElement.SetAttribute("title", rootTopic.Title);
            rootNode.AppendChild(topicElement);

            GenerateContentFileElements(topicElement, rootTopic.Children);

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
            GenerateTopicFiles( new Topic[1] {RootTopic} );
        }

        private void GenerateTopicFiles(IEnumerable<Topic> topics)
        {
            foreach (var topic in topics)
            {
                AddTopicFile(topic);
                FeatureContentCreator creator = new FeatureContentCreator();
                string topicContents = creator.Visit((dynamic)topic); 
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

        private StringBuilder GenerateTopicIndex(StringBuilder acc, IEnumerable<Topic> topics)
        {
            foreach (var topic in topics)
            {
                FeatureTopic featureTopic = topic as FeatureTopic;
                if (featureTopic != null && !String.IsNullOrEmpty(featureTopic.UserStoryId))
                {
                    acc.AppendLine(featureTopic.UserStoryId + "," + featureTopic.Id);
                }

                acc = GenerateTopicIndex(acc, topic.Children);
            }
            return acc;
        }

        private static string GetAbsoluteFileName(string topicsFolder, Topic topic)
        {
            return Path.Combine(topicsFolder, Path.ChangeExtension(topic.Id, ".aml"));
        }

    }
}
