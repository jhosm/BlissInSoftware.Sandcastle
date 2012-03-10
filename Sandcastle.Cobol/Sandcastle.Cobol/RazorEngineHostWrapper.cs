using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.Razor;
using Microsoft.CSharp;

namespace BlissInSoftware.Sandcastle.Cobol
{
    /// <summary>
    /// This class is a wrapper around the Razor Engine and Razor Engine Host
    /// </summary>
    public class RazorEngineHostWrapper
    {
        private RazorTemplateEngine InitializeRazorEngine(Type baseClassType, string namespaceOfGeneratedClass, string generatedClassName)
        {
            RazorEngineHost host = new RazorEngineHost(new CSharpRazorCodeLanguage());
            host.DefaultBaseClass = baseClassType.FullName;
            host.DefaultClassName = generatedClassName;
            host.DefaultNamespace = namespaceOfGeneratedClass;
            host.NamespaceImports.Add("System");
            host.NamespaceImports.Add("System.Collections.Generic");
            host.NamespaceImports.Add("System.Linq");
            return new RazorTemplateEngine(host);
        }

        /// <summary>
        /// This method Parses and compiles the source code into an Assembly and returns it
        /// </summary>
        /// <param name="baseClassType">The Type of the Base class the generated class descends from</param>
        /// <param name="namespaceOfGeneratedClass">The Namespace of the generated class</param>
        /// <param name="generatedClassName">The Class Name of the generated class</param>
        /// <param name="sourceCodeReader">A Text reader that is a warpper around the "Template" that is to be parsed and compiled</param>
        /// <returns>An instance of a generated assembly that contains the generated class</returns>
        public Assembly ParseAndCompileTemplate(Type baseClassType, string namespaceOfGeneratedClass, string generatedClassName, TextReader sourceCodeReader)
        {
            RazorTemplateEngine engine = InitializeRazorEngine(baseClassType, namespaceOfGeneratedClass, generatedClassName);
            GeneratorResults razorResults = engine.GenerateCode(sourceCodeReader);

            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            CodeGeneratorOptions options = new CodeGeneratorOptions();

            string generatedCode = null;
            using (StringWriter writer = new StringWriter())
            {
                codeProvider.GenerateCodeFromCompileUnit(razorResults.GeneratedCode, writer, options);
                generatedCode = writer.GetStringBuilder().ToString();
            }

            var outputAssemblyName = Path.GetTempPath() + Guid.NewGuid().ToString("N") + ".dll";
            CompilerParameters compilerParameters = new CompilerParameters(new string[]{}, outputAssemblyName);
            compilerParameters.ReferencedAssemblies.Add("System.dll");
            compilerParameters.ReferencedAssemblies.Add("System.Core.dll");
            compilerParameters.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().CodeBase.Substring(8));
            compilerParameters.GenerateInMemory = false;

            CompilerResults compilerResults = codeProvider.CompileAssemblyFromDom(compilerParameters, razorResults.GeneratedCode);
            if (compilerResults.Errors.Count > 0)
            {
                var compileErrors = new StringBuilder();
                foreach (System.CodeDom.Compiler.CompilerError compileError in compilerResults.Errors)
                    compileErrors.Append(String.Format("Line: {0}\t Col: {1}\t Error: {2}\r\n", compileError.Line, compileError.Column, compileError.ErrorText));

                throw new Exception(compileErrors.ToString() + generatedCode);
            }

            return compilerResults.CompiledAssembly;
        }
    }

}