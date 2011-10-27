using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using System.Globalization;
using BlissInSoftware.Sandcastle.Gherkin;

namespace BlissInSoftware.Sandcastle.Gherkin.UnitTests
{
    [TestFixture]
    public class FeatureTopicContentBuilderFixture
    {
        [Test]
        public void ShouldExtractSummaryFromFullDescription()
        {
            string featureText = new UTF8Encoding().GetString(Resource1.FeatureWithoutDescription);
            Feature feature = LoadFeature(featureText);
            FeatureTopicContentBuilder cut = new FeatureTopicContentBuilder(feature);
            Assert.AreEqual(
                "Como Validador<markup><br /></markup>" +
                "Consigo aceder à listagem do FE de Validação, dado possuir acesso a operação GAS adequada,<markup><br /></markup>" +
	            "De modo a poder consultar todos os processos de um dado conjunto de Processos de Negócio."
                , cut.BuildSummary());
        }

        [Test]
        public void ShouldBuildScenarios()
        {
            string featureText = new UTF8Encoding().GetString(Resource1.FeatureWithoutDescription);
            Feature feature = LoadFeature(featureText);
            FeatureTopicContentBuilder cut = new FeatureTopicContentBuilder(feature);
            Assert.AreEqual(
                "Cenário: Validador tem acesso à listagem" + Environment.NewLine +
                "Dado um utilizador com a operação GAS \"keyPapiro_Listagem_BO\"" + Environment.NewLine +
                "Quando acede à listagem do FE de Validação" + Environment.NewLine +
                "Então tem acesso concedido." + Environment.NewLine +
                "" + Environment.NewLine +
                "@Automated" + Environment.NewLine +
                "Cenário: Validador não tem acesso à listagem" + Environment.NewLine +
                "Dado um utilizador sem a operação GAS \"keyPapiro_Listagem_BO\"" + Environment.NewLine +
                "Quando acede à listagem do FE de Validação" + Environment.NewLine +
	            "Então tem acesso negado."
                , cut.BuildScenarios());
        }

        [TestCase("Regras:")]
        [TestCase("GUI:")]
        [TestCase("Notas:")]
        [TestCase("")]
        public void ShouldBuildDescriptionWithSubsections(string section)
        {
            string featureText = "Funcionalidade: blabla" + Environment.NewLine +
                "Como Validador" + Environment.NewLine +
                "Consigo aceder à listagem do FE de Validação, dado possuir acesso a operação GAS adequada," + Environment.NewLine +
                "De modo a poder consultar todos os processos de um dado conjunto de Processos de Negócio." + Environment.NewLine + Environment.NewLine +
                "descrição livre" + Environment.NewLine + Environment.NewLine +
                section + Environment.NewLine; 
                
            Feature feature = LoadFeature(featureText);
            FeatureTopicContentBuilder cut = new FeatureTopicContentBuilder(feature);
            Assert.AreEqual(
                "descrição livre"
                , cut.BuildDescription());
        }

        [TestCase("GUI:")]
        [TestCase("Notas:")]
        [TestCase("")]
        public void ShouldBuildRules(string postfixFeatureText)
        {
            string featureText = "Funcionalidade: blabla" + Environment.NewLine +
                "descrição livre" + Environment.NewLine + Environment.NewLine +
                "Regras:" + Environment.NewLine +
                "uma regra" + Environment.NewLine +
                "duas regras" + Environment.NewLine + 
                postfixFeatureText;

            Feature feature = LoadFeature(featureText);
            FeatureTopicContentBuilder cut = new FeatureTopicContentBuilder(feature);

            Assert.AreEqual(
                "uma regra<markup><br /></markup>" +
                "duas regras" 
                , cut.BuildRules());
        }

        [TestCase("GUI:")]
        [TestCase("Notas:")]
        [TestCase("")]
        public void ShouldBuildRulesWhenThereIsNoDescription(string postfixFeatureText)
        {
            string featureText = "Funcionalidade: blabla" + Environment.NewLine +
                "Regras:" + Environment.NewLine +
                "uma regra" + Environment.NewLine +
                "duas regras" + Environment.NewLine +
                postfixFeatureText;

            Feature feature = LoadFeature(featureText);
            FeatureTopicContentBuilder cut = new FeatureTopicContentBuilder(feature);

            Assert.AreEqual(
                "uma regra<markup><br /></markup>" +
                "duas regras"
                , cut.BuildRules());
        }

        [Test]
        public void ShouldReturnEmptyStringWhenThereAreNoRules()
        {
            string featureText = "Funcionalidade: blabla" + Environment.NewLine +
                "descrição livre" + Environment.NewLine + Environment.NewLine;

            Feature feature = LoadFeature(featureText);
            FeatureTopicContentBuilder cut = new FeatureTopicContentBuilder(feature);

            Assert.AreEqual("Não aplicável no contexto desta história.", cut.BuildRules());
        }

        [TestCase("Notas:")]
        [TestCase("")]
        public void ShouldBuildGUI(string postfixFeatureText)
        {
            string featureText = "Funcionalidade: blabla" + Environment.NewLine +
                "descrição livre" + Environment.NewLine + Environment.NewLine +
                "GUI:" + Environment.NewLine +
                "uma GUI" + Environment.NewLine +
                postfixFeatureText;

            Feature feature = LoadFeature(featureText);
            FeatureTopicContentBuilder cut = new FeatureTopicContentBuilder(feature);

            Assert.AreEqual(
                "uma GUI"
                , cut.BuildGUI());
        }

        [Test]
        public void ShouldBuildNotes()
        {
            string featureText = "Funcionalidade: blabla" + Environment.NewLine +
                "descrição livre" + Environment.NewLine + Environment.NewLine +
                "Notas:" + Environment.NewLine +
                "uma nota";

            Feature feature = LoadFeature(featureText);
            FeatureTopicContentBuilder cut = new FeatureTopicContentBuilder(feature);

            Assert.AreEqual(
                "uma nota"
                , cut.BuildNotes());
        }

        private static Feature LoadFeature(string featureText)
        {
            SpecFlowLangParser specFlowLangParser = new SpecFlowLangParser(new CultureInfo("pt-PT"));
            TextReader textReader = new StringReader(featureText);
            Feature feature;
            using (textReader)
            {
                feature = specFlowLangParser.Parse(textReader, Environment.CurrentDirectory);
            }
            return feature;
        }
    }
}
