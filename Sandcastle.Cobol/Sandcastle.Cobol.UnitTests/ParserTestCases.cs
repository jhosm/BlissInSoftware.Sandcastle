using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using BlissInSoftware.Sandcastle.Cobol;

namespace BlissInSoftware.Sandcastle.Cobol.UnitTests
{
    [TestFixture]
    public class ParserTestCases
    {
        Parser parser;
        ProgramInfo info;

        [SetUp]
        public void ExecuteBeforeEachTest()
        {
            string source = File.ReadAllText("CBZ00000.TXT", Encoding.Default);
            parser = new Parser(source);
            info = parser.Parse();
        }


        [Test]
        public void ShouldParseProgramId()
        {
            Assert.AreEqual("CBZ00000", info.ProgramId);
        }

        [Test]
        public void ShouldParseProgramDescription()
        {
            Assert.AreEqual(TestCasesConstants.PROGRAM_DESCRIPTION, info.ProgramDescription);
        }

        [Test]
        public void ShouldParseLinkageSection()
        {
            Assert.AreEqual(
                "       01  DFHCOMMAREA.                                                         \r\n" +
                "       COPY CBCS0000.                                                           ", 
                info.LinkageSection);
        }
    }
}
