using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlissInSoftware.Sandcastle.Cobol
{
    class CobolLinePatterns
    {
        public static string ProgramId
        {
            get { return CustomLinePattern(@"PROGRAM-ID.\s*([^\.]+)\."); }
        }

        public static string LinkageSection
        {
            get { return CustomLinePattern(@"LINKAGE\sSECTION\."); }
        }

        public static string ProcedureDivision
        {
            get { return CustomLinePattern(@"PROCEDURE\sDIVISION"); }
        }

        public static string CustomLinePattern(string pattern)
        {
            return CustomLinePattern(pattern, true);
        }

        public static string CustomLinePattern(string pattern, bool ignoreContinuationArea)
        {
            string prefix;
            if (ignoreContinuationArea)
            {
                prefix = @"^.{7}";
            }
            else
            {
                prefix = @"^.{6}";
            }
            return prefix + pattern + @".*$";
        }
    }
}
