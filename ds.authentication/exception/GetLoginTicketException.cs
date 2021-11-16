using ds.enovia.common.exception;
using System.Net.Http;

namespace ds.authentication.exception
{
    public class GetLoginTicketException : ResponseException
    {
        public GetLoginTicketException(HttpResponseMessage _responseMessage) : base(_responseMessage)
        {

        }
    }
}
