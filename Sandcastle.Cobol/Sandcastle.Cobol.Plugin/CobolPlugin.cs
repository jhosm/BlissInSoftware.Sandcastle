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
using System.Globalization;

namespace BlissInSoftware.Sandcastle.Cobol.PlugIn
{
    public class CobolPlugin : SandcastleBuilder.Utils.PlugIn.IPlugIn
    {
        #region Private data members
        //=====================================================================

        private ExecutionPointCollection executionPoints;

        private BuildProcess builder;

        private const string xpathCobolProgramsPath = "/configuration/cobolPrograms/path";

        private string cobolProgramsPath;
        #endregion

        #region IPlugIn implementation
        //=====================================================================

        /// <summary>
        /// This read-only property returns a friendly name for the plug-in
        /// </summary>
        public string Name
        {
            get { return "Cobol Programs"; }
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
                return "None";
            }
        }

        /// <summary>
        /// This read-only property returns a brief description of the plug-in
        /// </summary>
        public string Description
        {
            get
            {
                return "A plugin to transform a set of Cobol Programs into documentation.";
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

            XmlElement elm = configXml.SelectSingleNode(xpathCobolProgramsPath) as XmlElement;
            if (elm != null)
            {
                cobolProgramsPath = elm.InnerText;
            }

            using (CobolConfigDialog dialog = new CobolConfigDialog(cobolProgramsPath))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    cobolProgramsPath = dialog.CobolProgramsPath;
                    if (elm != null)
                    {
                        elm.InnerText = cobolProgramsPath;
                    }
                    else
                    {
                        XmlElement cobolPrograms = configXml.CreateElement("cobolPrograms");
                        configXml.DocumentElement.AppendChild(cobolPrograms);

                        XmlElement pathNode = configXml.CreateElement("path");
                        cobolPrograms.AppendChild(pathNode);
                        pathNode.InnerText = cobolProgramsPath;
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
            XPathNavigator cobolProgramsConfiguredPath = configuration.SelectSingleNode(xpathCobolProgramsPath) ;
            if (cobolProgramsConfiguredPath == null) throw new InvalidOperationException("Could not find on the configuration the path to the definitions to document. Please, check the " + Name + " plugin configuration.");
            
            cobolProgramsPath = cobolProgramsConfiguredPath.InnerXml;

            if (!Directory.Exists(cobolProgramsPath)) throw new InvalidOperationException("Could not find the configured path to the definitions to document. Please, check the " + Name + " plugin configuration.");
        }

        /// <summary>
        /// This method is used to execute the plug-in during the build process
        /// </summary>
        /// <param name="context">The current execution context</param>
        public void Execute(ExecutionContext context)
        {
            builder.ReportProgress("Creating Cobol Programs documentation...");

            var contentGenerator = new ContentGenerator(builder, cobolProgramsPath);
            contentGenerator.Generate();

            AddLinkedItem(BuildAction.ContentLayout, contentGenerator.ContentFile);

            foreach (var cobolProgramTopicFileName in contentGenerator.TopicFilesPath)
                AddLinkedItem(BuildAction.None, cobolProgramTopicFileName);

            var componentConfig = GetComponentConfiguration(contentGenerator.TopicsIndexPath);
            builder.CurrentProject.ComponentConfigurations.Add(GetComponentId(), true, componentConfig);

            builder.CurrentProject.MSBuildProject.ReevaluateIfNecessary();
        }
        #endregion


        private static string GetComponentId()
        {
            return @"CobolResolveLinks";
        }

        private string GetComponentConfiguration(string topicIndexPath)
        {
            var id = GetComponentId();
            const string name = @"BlissInSoftware.Sandcastle.Cobol.BuildComponents.ResolveLinksComponent";
            const string componentDllName = "Sandcastle.Cobol.BuildComponents.dll";
            var plugInDirectoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var componentPath = Path.Combine(plugInDirectoryPath, componentDllName);
            var componentConfig = string.Format(CultureInfo.CurrentCulture, @"<component id=""{0}"" type=""{1}"" assembly=""{2}""><topicIndex location=""{3}"" /></component>", id, name, componentPath, topicIndexPath);
            builder.ReportProgress(componentConfig);

            return componentConfig;
        }

        private ProjectItem AddLinkedItem(BuildAction buildAction, string fileName)
        {
            var project = builder.CurrentProject.MSBuildProject;
            var itemName = buildAction.ToString();
            builder.ReportProgress("Adding itemName {0} located at {1} to project...", itemName, fileName);
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
        ~CobolPlugin()
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

