using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using System.Globalization;
using BlissInSoftware.Sandcastle.Gherkin.Plugin;

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
                "Dado um utilizador com a operação GAS \"keyPapiro_Listagem_BO\"" + Environment.NewLine +
                "Quando acede à listagem do FE de Validação" + Environment.NewLine +
                "Então tem acesso concedido." + Environment.NewLine +
                "" + Environment.NewLine +
                "@Automated" + Environment.NewLine + 
                "Dado um utilizador sem a operação GAS \"keyPapiro_Listagem_BO\"" + Environment.NewLine +
                "Quando acede à listagem do FE de Validação" + Environment.NewLine +
	            "Então tem acesso negado."
                , cut.BuildScenarios());
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
