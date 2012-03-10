using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace BlissInSoftware.Sandcastle.Cobol
{
    public class CobolTopicCreator
    {
        public abstract class TemplateBase
        {
            public StringBuilder Buffer { get; private set; }
            private TextWriter writer;
            private TextWriter Writer { get { return writer; } }

            public TemplateBase()
            {
                Buffer = new StringBuilder();
                writer = new StringWriter(Buffer);
            }

            public virtual void Write(object value)
            {
                Writer.Write(value);
            }

            public virtual void WriteLiteral(object value)
            {
                Writer.Write(value);
            }

            public abstract void Execute();
        }

        public abstract class FolderTemplate: TemplateBase {
            public TopicInfo TopicInfo { get; set; }
        }
        public abstract class ProgramInfoTemplate : TemplateBase
        {
            public ProgramInfo ProgramInfo { get; set; }
        }

        public string Create(TopicInfo info)
        {
            string templateName;
            Type templateType;
            ProgramInfo programInfo = info as ProgramInfo;
            if (programInfo != null)
            {
                templateName = "CobolProgramTemplate.razor";
                templateType = typeof(ProgramInfoTemplate);
            }
            else
            {
                templateName = "TemplateBase.razor";
                templateType = typeof(FolderTemplate);
            }
            TextReader razorTemplateReader = new StreamReader(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), templateName));
            RazorEngineHostWrapper razorEngineHostWrapper = new RazorEngineHostWrapper();
            var generatedAssembly = razorEngineHostWrapper.ParseAndCompileTemplate(templateType, "BlissInSoftware.Sandcastle.Cobol", "TopicTemplate", razorTemplateReader);

            Type type = generatedAssembly.GetType("BlissInSoftware.Sandcastle.Cobol.TopicTemplate");
            var instance = (TemplateBase)Activator.CreateInstance(type);
            if (programInfo != null)
            {

               // programInfo.ProgramDescription = programInfo.ProgramDescription.Replace(Environment.NewLine, "<markup><br /></markup>");
                ((ProgramInfoTemplate)instance).ProgramInfo = programInfo;
            }
            else
            {
                ((FolderTemplate)instance).TopicInfo = info;
            }
            instance.Execute();
            return instance.Buffer.ToString();
        }
    }
}
