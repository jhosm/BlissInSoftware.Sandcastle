using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SandcastleBuilder.Utils.BuildEngine;
using System.IO;
using System.Xml;
using System.Security.Cryptography;
using BlissInSoftware.Sandcastle.Gherkin;
using System.Globalization;

namespace BlissInSoftware.Sandcastle.Gherkin.Plugin
{
    class ContentGenerator
    {
        private BuildProcess builder;
        private string gherkinFeaturesPath;
        private CultureInfo gherkinFeaturesLanguage;
        private HashSet<Guid> guidsInUse;
        private HashAlgorithm md5;

        public string ContentFile { get; private set; }
        public string TopicsFolder { get; private set; }
        public string TopicIndexPath { get { return Path.Combine(TopicsFolder, "topicIndex.txt"); } }
        public List<string> TopicFiles { get; private set; }
        public Topic RootTopic { get; private set; }
        
        public ContentGenerator(BuildProcess builder, string gherkinFeaturesPath, CultureInfo gherkinFeaturesLanguage)
        {
            this.builder = builder;
            this.gherkinFeaturesPath = gherkinFeaturesPath;
            this.gherkinFeaturesLanguage = gherkinFeaturesLanguage;
            this.guidsInUse = new HashSet<Guid>();
        }

        internal void Generate()
        {
            TopicsFolder = Path.Combine(builder.OutputFolder, "gherkinFeaturesTopics");
            ContentFile = Path.Combine(TopicsFolder, "gherkinFeatures.content");
            BuildTopics();
            GenerateContentFile(RootTopic);
            GenerateTopicFiles();
            Dictionary<string, FeatureTopic> topicIndex = GenerateTopicIndex(new Dictionary<string, FeatureTopic>(), new Topic[1] { RootTopic });
            string topicIndexContents = topicIndex.Aggregate("", (acc, next) =>
                                                  acc + next.Key + "," + next.Value.Id + "\r\n");
            File.WriteAllText(TopicIndexPath, topicIndexContents.Substring(0, topicIndexContents.Length - 2));
        }

        private Topic BuildTopics()
        {
            using (md5 = HashAlgorithm.Create("MD5"))
            {

                RootTopic = Topic.Create(TopicType.FeatureSet, CreateTopicId("Documentação Funcional"), "Documentação Funcional", gherkinFeaturesPath, gherkinFeaturesLanguage);
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
                Topic currentTopic = Topic.Create(TopicType.FeatureSet, CreateTopicId(featureSet),Path.GetFileName(featureSet), featureSet, gherkinFeaturesLanguage);
                parentTopic.Children.Add(currentTopic);
                BuildTopicsTree(featureSet, currentTopic);
            }

            foreach (string featureFile in Directory.EnumerateFiles(parentPath, "*.feature"))
            {
                builder.ReportProgress("Feature: " + featureFile);
                Topic currentTopic = Topic.Create(TopicType.Feature, CreateTopicId(featureFile), Path.GetFileNameWithoutExtension(featureFile), featureFile, gherkinFeaturesLanguage);
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
            GenerateTopicFiles( new Topic[1] {RootTopic}, null, null);
        }

        private void GenerateTopicFiles(IList<Topic> topics, Topic parent, Topic parentsNextSibling)
        {

            for (int i = 0; i < topics.Count; i++)
            {
                Topic topic = topics[i];
                topic.Children.Sort((x, y) => { return String.Compare(x.Title, y.Title); });
                Topic nextTopic;
                Topic previousTopic;
                DeterminePreviousAndNextTopic(topics, parent, parentsNextSibling, i, topic, out nextTopic, out previousTopic);

                AddTopicFile(topic);
                FeatureContentCreator creator = new FeatureContentCreator();
                string topicContents = creator.Visit((dynamic)topic, nextTopic, previousTopic);
                WriteTopicFile(topic, topicContents);

                var topicsNextSibling = i + 1 < topics.Count ? parent.Children[i + 1] : null;
                GenerateTopicFiles(topic.Children, topic, topicsNextSibling);
                
            }
        }

        private static void DeterminePreviousAndNextTopic(IList<Topic> topics, Topic parent, Topic parentsNextSibling, int topicsSiblingPosition, Topic topic, out Topic nextTopic, out Topic previousTopic)
        {
            var topicIsFirstSibling = topicsSiblingPosition == 0;
            var topicIsLastSibling = topicsSiblingPosition == topics.Count - 1;
            if (topicIsFirstSibling)
            {
                previousTopic = parent;
                if (topic.Children.Count > 0)
                {
                    nextTopic = topic.Children[0];
                }
                else if (topicIsLastSibling)
                {
                    nextTopic = parentsNextSibling;
                }
                else
                {
                    nextTopic = topics[topicsSiblingPosition + 1];
                }
            }
            else if (topicIsLastSibling)
            {
                if (topic.Children.Count > 0)
                {
                    nextTopic = topic.Children[0];
                }
                else
                {
                    nextTopic = parentsNextSibling;
                }
                previousTopic = topics[topicsSiblingPosition - 1];
            }
            else
            {
                if (topic.Children.Count > 0)
                {
                    nextTopic = topic.Children[0];
                }
                else
                {
                    nextTopic = topics[topicsSiblingPosition + 1];
                }

                var previousSibling = topics[topicsSiblingPosition - 1];
                if (previousSibling.Children.Count > 0)
                {
                    previousTopic = previousSibling.Children[previousSibling.Children.Count - 1];
                }
                else
                {
                    previousTopic = topics[topicsSiblingPosition - 1];
                }
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

        private Dictionary<string, FeatureTopic> GenerateTopicIndex(Dictionary<string, FeatureTopic> acc, IEnumerable<Topic> topics)
        {
            foreach (var topic in topics)
            {
                FeatureTopic featureTopic = topic as FeatureTopic;
                if (featureTopic != null && !String.IsNullOrEmpty(featureTopic.UserStoryId))
                {
                    if(acc.ContainsKey(featureTopic.UserStoryId)) {
                        throw new Exception(string.Format("Encontradas duas histórias com a mesma tag: @HU_{0}. As Histórias são {1} e {2}.", featureTopic.UserStoryId, featureTopic.SourcePath, acc[featureTopic.UserStoryId].SourcePath));
                    }
                    acc.Add(featureTopic.UserStoryId, featureTopic);
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
