using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BlissInSoftware.Sandcastle.Cobol
{
    public class Parser
    {
        public Parser(string source)
        {
            Source = source;
        }

        public string Source { get; private set; }

        public ProgramInfo Parse()
        {
            string[] source = Source.Split(new char[] {'\r','\n'}, StringSplitOptions.RemoveEmptyEntries);
            ProgramInfo info = new ProgramInfo();
            for (int i = 0; i < source.Length; i++)
            {
                string line = source[i];
                TryParseProgramId(info, line);
                TryParseProgramDescription(info, source, ref i);
                TryParseLinkageSection(info, source, ref i);
            }

            return info;
        }

        private static void TryParseProgramId(ProgramInfo info, string line)
        {
            if (!String.IsNullOrEmpty(info.ProgramId)) return;
            
            Match programId = Regex.Match(line, CobolLinePatterns.ProgramId);
            if (programId.Groups.Count > 1)
            {
                info.ProgramId = programId.Groups[1].Value;
            }
        }

        private void TryParseProgramDescription(ProgramInfo info, string[] source, ref int currentLineNumber)
        {
            CommentsParser parser = new CommentsParser();
            string programDescription = parser.TryParse(source, ref currentLineNumber);
            if (!String.IsNullOrEmpty(programDescription))
            {
                info.ProgramDescription = programDescription;
            }
        }

        private void TryParseLinkageSection(ProgramInfo info, string[] source, ref int currentLineNumber)
        {
            if (!String.IsNullOrEmpty(info.LinkageSection)) return;

            if (!Regex.IsMatch(source[currentLineNumber], CobolLinePatterns.LinkageSection))
            {
                return;
            }
            currentLineNumber++;
            Stack<string> potentialLinkageSectionLines = new Stack<string>();
            while (!Regex.IsMatch(source[currentLineNumber], CobolLinePatterns.ProcedureDivision))
            {
                potentialLinkageSectionLines.Push(source[currentLineNumber]);
                currentLineNumber++;
                if (currentLineNumber >= source.Length) throw new ParsingException("Could not find end of LINKAGE SECTION.");
            }
            while (Regex.IsMatch(potentialLinkageSectionLines.Peek(), CobolLinePatterns.CustomLinePattern(@"\*", false)))
            {
                potentialLinkageSectionLines.Pop();
                currentLineNumber--;
            }
            info.LinkageSection = String.Join(Environment.NewLine, potentialLinkageSectionLines.ToArray().Reverse());
            currentLineNumber++;
        }

    }
}
