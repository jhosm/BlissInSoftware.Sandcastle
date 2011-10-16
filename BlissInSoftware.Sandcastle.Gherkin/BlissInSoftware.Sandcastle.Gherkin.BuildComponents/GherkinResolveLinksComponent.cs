using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Ddue.Tools;
using System.Xml;
using System.Xml.XPath;

namespace BlissInSoftware.Sandcastle.Gherkin.BuildComponents
{
    public class GherkinResolveLinksComponent: BuildComponent
    {
        #region Private data members
        //=======================================================
        // Private data members

        // TODO: Add private data members here

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="assembler">A reference to the
        /// build assembler.</param>
        /// <param name="configuration">The configuration
        /// information</param>
        /// <exception cref="ConfigurationErrorsException">
        /// This is thrown if an error is detected in the
        /// configuration.</exception>
        public GherkinResolveLinksComponent(BuildAssembler assembler, XPathNavigator configuration) : base(assembler, configuration)
        {
            // TODO: Add code to extract configuration options
        }
        #endregion

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
            // TODO: Add code to perform the build
            //       component task(s)
        }
        #endregion
    }
}
