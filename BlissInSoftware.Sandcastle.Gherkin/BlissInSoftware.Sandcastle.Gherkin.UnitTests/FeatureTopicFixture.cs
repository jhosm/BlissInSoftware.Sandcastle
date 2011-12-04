using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Globalization;

namespace BlissInSoftware.Sandcastle.Gherkin.UnitTests
{
    [TestFixture]
    public class FeatureTopicFixture
    {
        [Test]
        public void ShouldIdentifyUserStoryIdIfPresent() {
            string unparsedFeature =
                "@HU_123" + Environment.NewLine +
                "Funcionalidade: Uma funcionalidade";
            FeatureTopic aTopic = new FeatureTopic(unparsedFeature) { Language = new CultureInfo("pt-PT") };
            aTopic.Load();

            Assert.AreEqual("123", aTopic.UserStoryId);
        }

        [Test]
        public void ShouldNotFailIfUserStoryIdIsNotPresent()
        {
            string unparsedFeature =
                "Funcionalidade: Uma funcionalidade";
            FeatureTopic aTopic = new FeatureTopic(unparsedFeature) { Language = new CultureInfo("pt-PT") };
            aTopic.Load();

            Assert.IsNull(aTopic.UserStoryId);
        }
    }
}
