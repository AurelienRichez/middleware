using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Web;

namespace WebServiceDotNet
{
  
    public class Contract : IContract
    {
        public const string JAVA_COMPILER_PATH = "\"C:\\Program Files\\Java\\jdk1.6.0_31\\bin\\javac.exe\" ";
        public const string JAVA_EXECUTABLE_PATH = "\"C:\\Program Files\\Java\\jdk1.6.0_31\\bin\\java.exe\" ";
        public const string JAVA_TECHNOLOGY = "JAVA";
        public const string JAVA_BUILD_EXTENSION = ".java";
        public const string JAVA_EXECUTION_EXTENSION = "";

        public const string CSHARP_COMPILER_PATH = "\"C:\\Windows\\Microsoft.NET\\Framework\\v3.5\\csc.exe\"";
        public const string CSHARP_TECHNOLOGY = "C#";

        public const string SRC_PATH = "D:\\";

        public const string FILE_NAME = "Test";

        public const string POST_SOURCE_CODE = "sourceCode";

        public const string POST_FILENAME = "fileName";

        public const string POST_TECHNOLOGY = "technology";

        /// <summary>
        /// Used to generate the random folder
        /// </summary>
        private static Random _Generator = new Random(DateTime.Now.Millisecond);

        public Result BuildJavaFile(string filePath)
        {
            return Execute(JAVA_COMPILER_PATH, filePath);
        }

        public GlobalResult BuildFromSource(string technology, string src, string fileName = FILE_NAME)
        {
            int rdnFolder = _Generator.Next();
            string folder = SRC_PATH + Path.DirectorySeparatorChar + rdnFolder;
            Directory.CreateDirectory(folder);
            string path = folder + Path.DirectorySeparatorChar + fileName;
            StreamWriter fileWriter = new StreamWriter(path + JAVA_BUILD_EXTENSION);
            fileWriter.Write(src);
            fileWriter.Close();
            Directory.SetCurrentDirectory(folder);
            Result resultBuild = Execute(GetCompiler(technology), fileName + JAVA_BUILD_EXTENSION);
            Result resultExecution = Execute(GetExecutable(technology), fileName + JAVA_EXECUTION_EXTENSION);
            Directory.SetCurrentDirectory(Directory.GetParent(folder).FullName);
            GlobalResult global = new GlobalResult();
            global.Compilation = resultBuild;
            global.Execution = resultExecution;
            Directory.Delete(folder, true);
            return global;
            
        }

        public GlobalResult BuildFromSource(Stream src)
        {
            NameValueCollection postParameters = HttpUtility.ParseQueryString(new StreamReader(src).ReadToEnd());
            string srcCode = postParameters.Get(POST_SOURCE_CODE);
            string fileName = postParameters.Get(POST_FILENAME);
            string technology = postParameters.Get(POST_TECHNOLOGY);

            if (string.IsNullOrEmpty(fileName))
                return BuildFromSource(technology, srcCode);
            else
                return BuildFromSource(technology, srcCode, fileName);
        }

        private static Result Execute(string executablePath, string filePath)
        {
            Result result = new Result();
            if (string.IsNullOrEmpty(filePath))
            {
                result.Error = "ERROR";
                result.Output = "File path does not exist";
                return result;
            }
            Process compiler = new Process();
            ProcessStartInfo parameters = new ProcessStartInfo(executablePath);
            parameters.Arguments = filePath;
            parameters.RedirectStandardOutput = true;
            parameters.RedirectStandardError = true;
            
            //parameters.RedirectStandardInput = true;
            //parameters.CreateNoWindow = true;
            
            parameters.UseShellExecute = false;
            compiler.StartInfo = parameters;
            Stopwatch chronometer = new Stopwatch();
            chronometer.Start();
            compiler.Start();
            compiler.WaitForExit();
            chronometer.Stop();
            result.TimeElapsed = chronometer.ElapsedMilliseconds / 1000;
            result.Error = CleanResult(compiler.StandardError.ReadToEnd());
            result.Output = CleanResult(compiler.StandardOutput.ReadToEnd());
            return result;
        }

        private static string CleanResult(string toClean)
        {
            string cleaned = toClean.Replace(Environment.NewLine, "<br/>");
            return cleaned;
        }

        private static string GetCompiler(string technology)
        {
            switch (technology.ToUpper().Trim())
            {
                case JAVA_TECHNOLOGY: return JAVA_COMPILER_PATH;
                case CSHARP_TECHNOLOGY: return CSHARP_COMPILER_PATH;
                default: return string.Empty;
            }
        }

        private static string GetExecutable(string technology)
        {
            switch (technology.ToUpper().Trim())
            {
                case JAVA_TECHNOLOGY: return JAVA_EXECUTABLE_PATH;
                case CSHARP_TECHNOLOGY: return string.Empty;
                default: return string.Empty;
            }
        }

        private static string GetBuildExtension(string technology)
        {
            switch (technology.ToUpper().Trim())
            {
                case JAVA_TECHNOLOGY: return JAVA_BUILD_EXTENSION;
                case CSHARP_TECHNOLOGY: return string.Empty;
                default: return string.Empty;
            }
        }

        private static string GetExecutionExtension(string technology)
        {
            switch (technology.ToUpper().Trim())
            {
                case JAVA_TECHNOLOGY: return JAVA_EXECUTION_EXTENSION;
                case CSHARP_TECHNOLOGY: return string.Empty;
                default: return string.Empty;
            }
        }
    }
}
