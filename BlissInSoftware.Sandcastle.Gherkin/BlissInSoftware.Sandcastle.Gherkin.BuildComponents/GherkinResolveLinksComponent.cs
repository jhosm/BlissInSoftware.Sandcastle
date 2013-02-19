using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Ddue.Tools;
using System.Xml;
using System.Xml.XPath;
using System.IO;

namespace BlissInSoftware.Sandcastle.Gherkin.BuildComponents
{
    public class GherkinResolveLinksComponent: BuildComponent
    {
        private Dictionary<string, string> topicIndex = new Dictionary<string,string>();

        public GherkinResolveLinksComponent(BuildAssembler assembler, XPathNavigator configuration)
            : base(assembler, configuration)
        {
            var fileName = configuration.SelectSingleNode("topicIndex/@location").Value;

            string[] lines = File.ReadAllLines(fileName);
            foreach (var line in lines)
            {
                string[] index = line.Split(',');
                if(topicIndex.ContainsKey(index[0])) throw new Exception("Found more than one feature file with story ID " + index[0]);
                topicIndex.Add(index[0], index[1]);
            }
        }

        #region Apply the component
        /// <summary>
        /// This is implemented to perform the component tasks.
        /// </summary>
        /// <param name="document">The XML document with
        /// which to work.</param>
        /// <param name="key">The key (member name) of the
        /// item being documented.</param>
        public override void Apply(XmlDocument document, string key)
        {
            var namespaceManager = new XmlNamespaceManager(document.NameTable);
            namespaceManager.AddNamespace("ghk", XmlNamespaces.GherkinDoc);

            var nodes = document.SelectNodes("//ghk:featureReference", namespaceManager);
            if (nodes == null)
                return;

            foreach (XmlNode node in nodes)
            {
                var parentNode = node.ParentNode;
                var userStoryId = node.InnerText;

                if (!topicIndex.ContainsKey(userStoryId))
                {
                    BuildAssembler.MessageHandler(GetType(), MessageLevel.Warn, "key", "Could not find user story with ID " + userStoryId);
                }
                else
                {
                    var topicId = topicIndex[userStoryId];
                    var linkElement = document.CreateElement("ddue", "link", XmlNamespaces.Maml);
                    linkElement.SetAttribute("href", XmlNamespaces.XLink, topicId);
                    linkElement.SetAttribute("topicType_id", "3272D745-2FFC-48C4-9E9D-CF2B2B784D5F");
                    linkElement.InnerText = "História de Utilizador " + userStoryId;
                    parentNode.ReplaceChild(linkElement, node);
                }
            }
        }
        #endregion
    }
}
