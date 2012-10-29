using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Collections.Specialized;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace RemoteCompiling
{

    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class Contract : IContract
    {
		
		/*
		 * définition des constantes java
		 */ 
		public const string JAVA_TECHNO = "java";
        public const string JAVA_COMPILER_PATH = "/usr/bin/javac";
        public const string JAVA_EXECUTABLE_PATH = "/usr/bin/java";
        public const string JAVA_BUILD_EXTENSION = ".java";
        public const string JAVA_EXECUTION_EXTENSION = "";
		
		/*
		 * définition des constantes c#
		 */ 
		public const string CSHARP_TECHNO = "c#";
		public const string CSHARP_COMPILER_PATH = "/usr/bin/mono-csc";
		public const string CSHARP_EXECUTABLE_PATH = "/usr/bin/mono";
		public const string CSHARP_BUILD_EXTENSION = ".cs";
        public const string CSHARP_EXECUTION_EXTENSION = ".exe";
		
		/*
		 * Diverses constantes
		 */ 
        public const string SRC_PATH = "/tmp";
        public const string FILE_NAME = "Test";
        public const string POST_SOURCE_CODE = "sourceCode";
        public const string POST_FILENAME = "fileName";
        public const string POST_TECHNOLOGY = "technology";
		public const string ERROR = "ERROR";

        /// <summary>
        /// Used to generate the random folder
        /// </summary>
        private static Random _Generator = new Random(DateTime.Now.Millisecond);

        public GlobalResult BuildFromSource(Stream src)
        {
            NameValueCollection postParameters = HttpUtility.ParseQueryString(new StreamReader(src).ReadToEnd());
            string srcCode = postParameters.Get(POST_SOURCE_CODE);
            string fileName = postParameters.Get(POST_FILENAME);
            string technology = postParameters.Get(POST_TECHNOLOGY);

            return InternalBuildFromSource(technology, srcCode, fileName);
        }

        public GlobalResult InternalBuildFromSource(string technology, string src, string fileName = FILE_NAME)
        {
			// Génère un dossier aléatoirement pour pouvoir supporter le parallelisme
            int rdnFolder = _Generator.Next();
            string folder = SRC_PATH + Path.DirectorySeparatorChar + rdnFolder;
            Directory.CreateDirectory(folder);
            string path = folder + Path.DirectorySeparatorChar + fileName;
			// On ecrit le fichier
            StreamWriter fileWriter = new StreamWriter(path + GetBuildExtension(technology));
            fileWriter.Write(src);
            fileWriter.Close();
			// On se place dans le bon dossier
            Directory.SetCurrentDirectory(folder);
			// On cree l'objet a retourner
			GlobalResult global = new GlobalResult();
			// On execute et si une erreur survient, on affiche un code d'erreur et on retourne un ERROR code egal
			// a "ERROR"
			try 
			{
           		global.Compilation = Execute(GetCompilerPath(technology), fileName + GetBuildExtension(technology));
			} 
			catch(Exception e)
			{
				global.Compilation.Error = ERROR;
				global.Compilation.Error = e.StackTrace;
			}
            try 
			{
				global.Execution = Execute(GetExecutablePath(technology) , fileName + GetExecutionExtension(technology));
			}
			catch(Exception e)
			{
				global.Execution.Error = ERROR;
				global.Execution.Output = e.StackTrace;
			}
			// On revient dans le dossier parent
            Directory.SetCurrentDirectory(Directory.GetParent(folder).FullName);
			// On supprime le dossier
            Directory.Delete(folder, true);
            return global;
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


        public string HelloWorld()
        {
            return "Hello World!";
        }
		
		private string GetBuildExtension(string technology)
		{
			if(technology.ToLower().Equals(JAVA_TECHNO))
				return JAVA_BUILD_EXTENSION;
			return CSHARP_BUILD_EXTENSION;
		}
		
		private string GetExecutionExtension(string technology)
		{
			if(technology.ToLower().Equals(JAVA_TECHNO))
				return JAVA_EXECUTION_EXTENSION;
			return CSHARP_EXECUTION_EXTENSION;
		}
		
		private string GetCompilerPath(string technology)
		{
			if(technology.ToLower().Equals(JAVA_TECHNO))
				return JAVA_COMPILER_PATH;
			return CSHARP_COMPILER_PATH;
		}
		
		private string GetExecutablePath(string technology)
		{
			if(technology.ToLower().Equals(JAVA_TECHNO))
				return JAVA_EXECUTABLE_PATH;
			return CSHARP_EXECUTABLE_PATH;
		}
		
    }
}
