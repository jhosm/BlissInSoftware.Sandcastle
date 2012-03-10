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
    public class CommentsParserTestCases
    {
        CommentsParser parser;

        [Test]
        public void ShouldParseProgramDescription()
        {
            int currentLineNumber = 7; //Start of <ID> in "CBZ00000.TXT"
            string[] source = File.ReadAllLines("CBZ00000.TXT", Encoding.Default);
            parser = new CommentsParser();
            string actualComment = parser.TryParse(source, ref currentLineNumber);
            Assert.AreEqual(TestCasesConstants.PROGRAM_DESCRIPTION, actualComment);
            Assert.AreEqual(52, currentLineNumber); //Line after closing </ID>
        }
    }
}
