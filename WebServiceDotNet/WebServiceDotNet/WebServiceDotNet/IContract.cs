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
        [WebGet(UriTemplate = "Build?filePath={filePath}")]
        BuildResult Build(string filePath);

        [OperationContract]
        [WebInvoke(Method="POST", UriTemplate = "BuildFromSource?src={src}&fileName={fileName}")]
        BuildResult BuildFromSource(string src, string fileName);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "BuildFromSourceStream")]
        BuildResult BuildFromSourceStream(Stream src);
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class BuildResult
    {
        private string output = string.Empty;
        private string error = string.Empty;

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
    }
}
