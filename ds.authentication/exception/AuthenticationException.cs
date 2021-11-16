using ds.enovia.common.exception;
using System.Net.Http;

namespace ds.authentication.exception
{
    public class AuthenticationException : ResponseException
    {
        public AuthenticationException(HttpResponseMessage _responseMessage) : base(_responseMessage)
        {

        }
    }
}
