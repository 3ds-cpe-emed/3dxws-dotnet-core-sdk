using ds.enovia.common.exception;
using System;
using System.Net.Http;

namespace ds.enovia.dsdo
{
    public class DerivedOutputException : ResponseException
    {
        //private static string ProcessErrorMessage(HttpResponseMessage _responseError)
        //{
        //    if (_responseError == null) return "";

        //    string errorMessage = string.Format("Status = {0}", _responseError.StatusCode);

        //    if (_responseError.Content != null)
        //    {
        //        try
        //        {
        //            object errorDescription;

        //            if (SimpleJson.TryDeserializeObject(_responseError.Content, out errorDescription))
        //            {
        //                if (errorDescription is RestSharp.JsonObject)
        //                {
        //                    object status = "";
        //                    if (((RestSharp.JsonObject)errorDescription).TryGetValue("status", out status) )
        //                    {
        //                        errorMessage = string.Format("Status = {0}", status.ToString());
        //                    }

        //                    object message = "";
        //                    if (((RestSharp.JsonObject)errorDescription).TryGetValue("message", out message))
        //                    {
        //                        errorMessage += string.Format("; Error Message = {0}", message.ToString()) ;
        //                    }
        //                }
        //            }
        //        }
        //        catch
        //        { }
        //    }
        //    return errorMessage;
        //}

        //public DerivedOutputException(HttpResponseMessage response) : base (ProcessErrorMessage(response))
        public DerivedOutputException(HttpResponseMessage response) : base(response)
        {
        }

    }
}
