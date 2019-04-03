using Backload.Contracts.Context;
using System.Web;

namespace Backload.Helper
{
    /// <summary>
    /// This helper class creates the result of type ActionResult from the IBackloadResult object
    /// </summary>
    public partial class ResultCreator
    {

        /// <summary>
        /// Writes the result directly into the response stream, e.g. for a classic ASP.NET or HTML response
        /// </summary>
        /// <param name="response">A standard HttpResponse object</param>
        /// <param name="result">An IBackloadResult object</param>
        public static void Write(HttpResponse response, IBackloadResult result)
        {

            response.StatusCode = (int)result.HttpStatusCode;
            if ((int)result.HttpStatusCode < 300)
            {

                // Write result to the response (Json or file data, default: Json response)
                if (result.RequestType == RequestType.Default)
                {
                    // Json response (We use the systems JavaScriptSerializer, you can also use Newtonsoft.Json)
                    IFileStatusResult status = (IFileStatusResult)result;
                    var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

                    if ((status.ClientStatus != null) && (result.Exception == null))
                        response.Write(serializer.Serialize(status.ClientStatus));
                }
                else if ((result.RequestType == RequestType.File) || (result.RequestType == RequestType.Thumbnail))
                {
                    // file data (byte array) response
                    IFileDataResult data = (IFileDataResult)result;

                    if ((data.FileData != null) && (result.Exception == null))
                        response.BinaryWrite(data.FileData);
                }
            }
        }
    }
}