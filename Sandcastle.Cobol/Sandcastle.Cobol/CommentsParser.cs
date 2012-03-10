using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BlissInSoftware.Sandcastle.Cobol
{
    public class CommentsParser
    {
        public string TryParse(string[] source, ref int currentLineNumber)
        {
            if (!Regex.IsMatch(source[currentLineNumber], CobolLinePatterns.CustomLinePattern(@"\*\s<ID>", false)))
            {
                return "";
            }
            string result = "";
            currentLineNumber++;
            while (!Regex.IsMatch(source[currentLineNumber], CobolLinePatterns.CustomLinePattern(@"\*\s<\/ID>", false)))
            {
                result += RemoveTrailingSpacesAndAsterisks(source[currentLineNumber]) + Environment.NewLine;
                currentLineNumber++;
                if (currentLineNumber >= source.Length) throw new ParsingException("Could not find closing tag for <ID>");
            }
            currentLineNumber++;
            return result;
        }

        private static string RemoveTrailingSpacesAndAsterisks(string line)
        {
            return line.Substring(8).TrimEnd(new char[] { ' ' }).TrimEnd(new char[] { '*' });
        }


    }
}
