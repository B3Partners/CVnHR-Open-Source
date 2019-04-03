using Backload.Contracts.Context;
using System;
using System.Web;
using System.Web.Mvc;

namespace Backload.Helper
{
    /// <summary>
    /// This helper class creates the result of type ActionResult from the IBackloadResult object
    /// </summary>
    public partial class ResultCreator
    {

        /// <summary>
        /// Returns an ActionResult depending on the requested type (Json or file data)
        /// </summary>
        /// <param name="result">An IBackloadResult instance</param>
        /// <returns>An ActionResult instance</returns>
        public static ActionResult Create(IBackloadResult result)
        {
            if ((int)result.HttpStatusCode < 300)
            {
                // RequestType.Default: Json output has been requested (default). 
                // Otherwise a file or a thumbnail (bytes) will be returned.
                if (result.RequestType == RequestType.Default)
                    return Create((IJsonResult)result);
                else if ((result.RequestType == RequestType.File) || (result.RequestType == RequestType.Thumbnail))
                    return Create((IFileDataResult)result);
            }

            // Http status code >= 300
            return new HttpStatusCodeResult(result.HttpStatusCode);
        }



        /// <summary>
        /// Creates the Json output for the files handled in this request
        /// </summary>
        /// <param name="result">A IFileStatusResult object with client plugin specfic data.</param>
        /// <returns>JsonResult instance or a http HttpStatusCodeResult to send an http status</returns>
        public static ActionResult Create(IJsonResult result)
        {
            object resultObject = null;

            // Converts to the correct type and gets the result object. Result is usually in ClientStatus 
            if (result.ResultType == ResultType.Status)
                resultObject = ((IFileStatusResult)result).ClientStatus;
            else if (result.ResultType == ResultType.Json) 
                resultObject = ((ICoreResult)result).ResultObject;

            // Create Json result from the returned client plugin specific file metadata.
            if ((resultObject != null) && (result.Exception == null))
                return new JsonResult()
                {
                    Data = resultObject,
                    ContentType = result.ContentType,
                    ContentEncoding = System.Text.Encoding.UTF8,
                    MaxJsonLength = Int32.MaxValue,
                    RecursionLimit = null,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };


            // A HttpStatusCodeResult result is returned on errors or if files not have been modified (304)
            return new HttpStatusCodeResult(result.HttpStatusCode);
        }



        /// <summary>
        /// Handles file data (bytes) if Backload is configured to handle file requests (e.g. thumbsUrlPattern) 
        /// </summary>
        /// <param name="result">A IFileDataResult object with file data (bytes).</param>
        /// <returns>FileContentResult instance or a http HttpStatusCodeResult to send an http status</returns>
        public static ActionResult Create(IFileDataResult result)
        {
            // Create a new FileContentResult from the returned file data.
            if ((result.FileData != null) && (result.Exception == null))
                return new FileContentResult(result.FileData, result.ContentType);


            // A HttpStatusCodeResult result is returned on errors or if files not have been modified (304)
            return new HttpStatusCodeResult(result.HttpStatusCode);
        }

    }
}