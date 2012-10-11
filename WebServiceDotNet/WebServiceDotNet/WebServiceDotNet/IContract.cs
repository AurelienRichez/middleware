using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.IO;

namespace WebServiceDotNet
{
    [ServiceContract]
    public interface IContract
    {
        [OperationContract]
        [WebGet(UriTemplate = "BuildJava?filePath={filePath}")]
        Result BuildJavaFile(string filePath);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "BuildFromSourceStream")]
        GlobalResult BuildFromSource(Stream src);
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class Result
    {
        private string output = string.Empty;
        private string error = string.Empty;
        private long timeElapsed = 0;

        [DataMember]
        public string Error
        {
            get { return error; }
            set { error = value; }
        }

        [DataMember]
        public string Output
        {
            get { return output; }
            set { output = value; }
        }

        [DataMember]
        public long TimeElapsed
        {
            get { return timeElapsed; }
            set { timeElapsed = value; }
        }
    }

    [DataContract]
    public class GlobalResult
    {
        private Result compilation = new Result();
        private Result execution = new Result();

        [DataMember]
        public Result Compilation
        {
            get { return compilation; }
            set { compilation = value; }
        }

        [DataMember]
        public Result Execution
        {
            get { return execution; }
            set { execution = value; }
        }
    }

}
