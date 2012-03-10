using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using BlissInSoftware.Sandcastle.Cobol;
using System.Xml;

namespace BlissInSoftware.Sandcastle.Cobol.UnitTests
{
    [TestFixture]
    public class CobolTopicCreatorTestCases
    {
        CobolTopicCreator creator;

        [Test]
        public void TopicShouldHaveProgramIdAsTitle()
        {
            ProgramInfo info = new ProgramInfo();
            info.ProgramId = "CBZ000";
            info.ProgramDescription = TestCasesConstants.PROGRAM_DESCRIPTION;
            creator = new CobolTopicCreator();
            string topic = creator.Create(info);
            Console.WriteLine(topic);
        }
    }
}
