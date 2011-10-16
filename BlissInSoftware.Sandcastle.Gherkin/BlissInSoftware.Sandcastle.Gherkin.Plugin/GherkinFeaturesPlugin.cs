using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.XPath;
using System.Xml;

using SandcastleBuilder.Utils;
using SandcastleBuilder.Utils.BuildEngine;
using SandcastleBuilder.Utils.PlugIn;
using System.IO;
using Microsoft.Build.Evaluation;
using System.Collections.Generic;

namespace BlissInSoftware.Sandcastle.Gherkin.Plugin
{
    public class GherkinFeaturesPlugin : SandcastleBuilder.Utils.PlugIn.IPlugIn
    {
        #region Private data members
        //=====================================================================

        private ExecutionPointCollection executionPoints;

        private BuildProcess builder;

        private const string xpathGherkinFeaturesPath = "/configuration/gherkinFeatures/path";

        private string gherkinFeaturesPath;
        #endregion

        #region IPlugIn implementation
        //=====================================================================

        /// <summary>
        /// This read-only property returns a friendly name for the plug-in
        /// </summary>
        public string Name
        {
            get { return "Gherkin Features"; }
        }

        /// <summary>
        /// This read-only property returns the version of the plug-in
        /// </summary>
        public Version Version
        {
            get
            {
                // TODO: Edit AssemblyInfo.cs to set your plug-in's version

                // Use the assembly version
                Assembly asm = Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(
                    asm.Location);

                return new Version(fvi.ProductVersion);
            }
        }

        /// <summary>
        /// This read-only property returns the copyright information for the
        /// plug-in.
        /// </summary>
        public string Copyright
        {
            get
            {
                // TODO: Edit AssemblyInfo.cs to set your plug-in's copyright

                // Use the assembly copyright
                Assembly asm = Assembly.GetExecutingAssembly();
                AssemblyCopyrightAttribute copyright =
                    (AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(
                        asm, typeof(AssemblyCopyrightAttribute));

                return copyright.Copyright;
            }
        }

        /// <summary>
        /// This read-only property returns a brief description of the plug-in
        /// </summary>
        public string Description
        {
            get
            {
                return "A plugin to transform a set of gherkin features into documentation.";
            }
        }

        /// <summary>
        /// This read-only property returns true if the plug-in should run in
        /// a partial build or false if it should not.
        /// </summary>
        /// <value>If this returns false, the plug-in will not be loaded when
        /// a partial build is performed.</value>
        public bool RunsInPartialBuild
        {
            // TODO: Set this to true if necessary
            get { return true; }
        }

        /// <summary>
        /// This read-only property returns a collection of execution points
        /// that define when the plug-in should be invoked during the build
        /// process.
        /// </summary>
        public ExecutionPointCollection ExecutionPoints
        {
            get
            {
                return executionPoints ?? (executionPoints = new ExecutionPointCollection
                                                                   {
                                                                       new ExecutionPoint(BuildStep.FindingTools,
                                                                                          ExecutionBehaviors.After)
                                                                   });
            }
        }

        /// <summary>
        /// This method is used by the Sandcastle Help File Builder to let the
        /// plug-in perform its own configuration.
        /// </summary>
        /// <param name="project">A reference to the active project</param>
        /// <param name="currentConfig">The current configuration XML fragment</param>
        /// <returns>A string containing the new configuration XML fragment</returns>
        /// <remarks>The configuration data will be stored in the help file
        /// builder project.</remarks>
        public string ConfigurePlugIn(SandcastleProject project,
          string currentConfig)
        {
            XmlDocument configXml = new XmlDocument();
            configXml.LoadXml(currentConfig);

            XmlElement elm = configXml.SelectSingleNode(xpathGherkinFeaturesPath) as XmlElement;
            if (elm != null)
            {
                gherkinFeaturesPath = elm.InnerText;
            }

            using (GherkinFeaturesConfigDlg dlg = new GherkinFeaturesConfigDlg(gherkinFeaturesPath))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    gherkinFeaturesPath = dlg.GherkinFeaturesPath;
                    if (elm != null)
                    {
                        elm.InnerText = gherkinFeaturesPath;
                    }
                    else
                    {
                        XmlElement gherkinNode = configXml.CreateElement("gherkinFeatures");
                        configXml.DocumentElement.AppendChild(gherkinNode);

                        XmlElement pathNode = configXml.CreateElement("path");
                        gherkinNode.AppendChild(pathNode);
                        pathNode.InnerText = gherkinFeaturesPath;
                    }

                    
                }
            }
            return configXml.OuterXml;
        }

        /// <summary>
        /// This method is used to initialize the plug-in at the start of the
        /// build process.
        /// </summary>
        /// <param name="buildProcess">A reference to the current build
        /// process.</param>
        /// <param name="configuration">The configuration data that the plug-in
        /// should use to initialize itself.</param>
        public void Initialize(BuildProcess buildProcess,
          XPathNavigator configuration)
        {
            builder = buildProcess;

            builder.ReportProgress("{0} Version {1}\r\n{2}\r\n",
                this.Name, this.Version, this.Copyright);

            // TODO: Add your initialization code here such as reading the
            // configuration data.
            XPathNavigator gherkinFeaturesConfiguredPath = configuration.SelectSingleNode(xpathGherkinFeaturesPath) ;
            if (gherkinFeaturesConfiguredPath == null) throw new InvalidOperationException("Could not find on the configuration the path to the features to document. Please, check the " + Name + " plugin configuration.");
            
            gherkinFeaturesPath = gherkinFeaturesConfiguredPath.InnerXml;

            if (!Directory.Exists(gherkinFeaturesPath)) throw new InvalidOperationException("Could not find the configured path to the features to document. Please, check the " + Name + " plugin configuration.");
        }

        /// <summary>
        /// This method is used to execute the plug-in during the build process
        /// </summary>
        /// <param name="context">The current execution context</param>
        public void Execute(ExecutionContext context)
        {
            builder.ReportProgress("Creating Features documentation...");

            var contentGenerator = new ContentGenerator(builder, gherkinFeaturesPath);
            contentGenerator.Generate();

            var contentLayoutItem = AddLinkedItem(BuildAction.ContentLayout, contentGenerator.ContentFile);
  //          contentLayoutItem.SetMetadataValue("SortOrder", Convert.ToString(_configuration.SortOrder, CultureInfo.InvariantCulture));

            foreach (var topicFileName in contentGenerator.TopicFiles)
                AddLinkedItem(BuildAction.None, topicFileName);

            builder.CurrentProject.MSBuildProject.ReevaluateIfNecessary();
        }
        #endregion

        private ProjectItem AddLinkedItem(BuildAction buildAction, string fileName)
        {
            var project = builder.CurrentProject.MSBuildProject;
            var itemName = buildAction.ToString();
            var buildItems = project.AddItem(itemName, fileName, new[] { new KeyValuePair<string, string>("Link", fileName) });
            Debug.Assert(buildItems.Count == 1);
            return buildItems[0];
        }

        #region IDisposable implementation
        //=====================================================================

        /// <summary>
        /// This handles garbage collection to ensure proper disposal of the
        /// plug-in if not done explicity with <see cref="Dispose()"/>.
        /// </summary>
        ~GherkinFeaturesPlugin()
        {
            // TODO: Change the name of this method to your plug-in name
            this.Dispose(false);
        }

        /// <summary>
        /// This implements the Dispose() interface to properly dispose of
        /// the plug-in object.
        /// </summary>
        /// <overloads>There are two overloads for this method.</overloads>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// This can be overridden by derived classes to add their own
        /// disposal code if necessary.
        /// </summary>
        /// <param name="disposing">Pass true to dispose of the managed
        /// and unmanaged resources or false to just dispose of the
        /// unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            // TODO: Dispose of any resources here if necessary
        }
        #endregion
    }
}

