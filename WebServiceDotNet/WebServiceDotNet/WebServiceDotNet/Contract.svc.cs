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
        public const string COMPILER_PATH = "\"C:\\Program Files\\Java\\jdk1.6.0_31\\bin\\javac.exe\" ";

        public const string SRC_PATH = "D:\\";

        public const string FILE_NAME = "Test.java";

        public const string POST_SOURCE_CODE = "sourceCode";

        public const string POST_FILENAME = "fileName";

        private static Random _Generator = new Random(DateTime.Now.Millisecond);

        public BuildResult Build(string filePath)
        {
            BuildResult result = new BuildResult();
            if (string.IsNullOrEmpty(filePath))
            {
                result.Error = "ERROR";
                result.Output = "File path does not exist";
                return result;
            }
            Process compiler = new Process();
            ProcessStartInfo parameters = new ProcessStartInfo(COMPILER_PATH);//"cmd");
            parameters.Arguments = filePath;
            parameters.RedirectStandardOutput = true;
            parameters.RedirectStandardError = true;
            //parameters.RedirectStandardInput = true;
            //parameters.CreateNoWindow = true;
            parameters.UseShellExecute = false;
            compiler.StartInfo = parameters;
            compiler.Start();
            compiler.WaitForExit();
            result.Error = compiler.StandardError.ReadToEnd();
            result.Output = compiler.StandardOutput.ReadToEnd();
            return result;
        }

        public BuildResult BuildFromSource(string src, string fileName = FILE_NAME)
        {
            int rdnFolder = _Generator.Next();
            string folder = SRC_PATH + Path.DirectorySeparatorChar + rdnFolder;
            Directory.CreateDirectory(folder);
            string path = folder + Path.DirectorySeparatorChar + fileName;
            StreamWriter fileWriter = new StreamWriter(path);
            fileWriter.Write(src);
            fileWriter.Close();
            BuildResult result = Build(path);
            Directory.Delete(folder, true);
            return result;
            
        }

        public BuildResult BuildFromSourceStream(Stream src)
        {
            NameValueCollection postParameters = HttpUtility.ParseQueryString(new StreamReader(src).ReadToEnd());
            string srcCode = postParameters.Get(POST_SOURCE_CODE);
            string fileName = postParameters.Get(POST_FILENAME);

            if (string.IsNullOrEmpty(fileName))
                return BuildFromSource(srcCode);
            else
                return BuildFromSource(srcCode, fileName);
        }
    }
}
