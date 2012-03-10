using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlissInSoftware.Sandcastle.Cobol
{
    public class ProgramInfo : TopicInfo
    {
        public string ProgramId { get; set; }
        public string ProgramDescription { get; set; }
        public string LinkageSection { get; set; }

        public ProgramInfo()
        {
            LinkageSection = "";
        }
    }
}
