using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Ddue.Tools;
using System.Xml;
using System.Xml.XPath;
using System.IO;

namespace BlissInSoftware.Sandcastle.Cobol.BuildComponents
{
    public class ResolveLinksComponent : BuildComponent
    {
        private Dictionary<string, string> topicIndex = new Dictionary<string, string>();

        public ResolveLinksComponent(BuildAssembler assembler, XPathNavigator configuration)
            : base(assembler, configuration)
        {
            var fileName = configuration.SelectSingleNode("topicIndex/@location").Value;

            string[] lines = File.ReadAllLines(fileName);
            foreach (var line in lines)
            {
                string[] index = line.Split(',');
                if (topicIndex.ContainsKey(index[0])) throw new Exception("Found more than one Cobol program with PROGRAM-ID " + index[0] + ".");
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
            namespaceManager.AddNamespace("cbl", XmlNamespaces.COBOL_DOC);

            var nodes = document.SelectNodes("//cbl:progId", namespaceManager);
            if (nodes == null)
                return;

            foreach (XmlNode node in nodes)
            {
                var parentNode = node.ParentNode;
                var programId = node.InnerText;

                if (!topicIndex.ContainsKey(programId))
                {
                    BuildAssembler.MessageHandler(GetType(), MessageLevel.Warn, "Could not find Cobol program with PROGRAM-ID " + programId);
                }
                else
                {
                    var topicId = topicIndex[programId];
                    var linkElement = document.CreateElement("ddue", "link", XmlNamespaces.MAML);
                    linkElement.SetAttribute("href", XmlNamespaces.XLINK, topicId);
                    linkElement.SetAttribute("topicType_id", "3272D745-2FFC-48C4-9E9D-CF2B2B784D5F");
                    linkElement.InnerText = "programa de Cobol " + programId;
                    parentNode.ReplaceChild(linkElement, node);
                }
            }
        }
        #endregion
    }
}
