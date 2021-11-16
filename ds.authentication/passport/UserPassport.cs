//------------------------------------------------------------------------------------------------------------------------------------
// Copyright 2020 Dassault Systèmes - CPE EMED
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify,
// merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished
// to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS
// BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//------------------------------------------------------------------------------------------------------------------------------------

using ds.authentication.exception;
using ds.authentication.model;
using ds.enovia.common.helper;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ds.authentication
{
    public class UserPassport : CASCookieBasedPassport, IPassportAuthentication
    {
        private const string LOGIN_WS = "/login";
        private const string LOGIN = "login";

        public UserPassport(string passportUrl) : base(passportUrl)
        {
        }


        private async Task<TicketInfo> GetLoginTicketInfo()
        {
            //Step 1 - Request login ticket
            UriRelative loginTicket = new UriRelative(GetEndpointURL(LOGIN_WS));
            loginTicket.AddQueryParameter("action", "get_auth_params");

            GetHttpJsonMessage getLoginTicketRequest = new GetHttpJsonMessage(loginTicket);

            HttpResponseMessage getLoginTicketResponse = await Client.SendAsync(getLoginTicketRequest);

            if (getLoginTicketResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw new GetLoginTicketException(getLoginTicketResponse);
            }

            //Handle ticket login response
            TicketInfo loginTicketInfo = await HttpContentJsonExtensions.ReadFromJsonAsync<TicketInfo>(getLoginTicketResponse.Content);

            //TicketInfo loginTicketInfo = await ((JsonContent)getLoginTicketResponse.Content).ReadFromJsonAsync<TicketInfo>();

            if (loginTicketInfo.response != LOGIN)
            {
                //handle according to established exception policy
                throw new GetLoginTicketException(getLoginTicketResponse);
            };

            return loginTicketInfo;
        }

        
        /*Bearer*/
        public async Task<bool> CASLogin(string username, string password, bool rememberMe)
        {
            //Step 1 - Request login ticket
            TicketInfo loginTicketInfo = await GetLoginTicketInfo();

            // Step 2 - build the login request
            UriRelative loginUri = new UriRelative(GetEndpointURL(LOGIN_WS));

            loginUri.AddQueryParameter( "lt", loginTicketInfo.lt);
            loginUri.AddQueryParameter("username", username);
            loginUri.AddQueryParameter("password", password);
            loginUri.AddQueryParameter("rememberMe", rememberMe.ToString());

            PostHttpJsonMessage loginRequest = new PostHttpJsonMessage(loginUri);
            
            HttpResponseMessage loginRequestResponse = await Client.SendAsync(loginRequest);

            if (!loginRequestResponse.IsSuccessStatusCode)
            {
                //handle according to established exception policy
                throw new AuthenticationException(loginRequestResponse);

            }
            return IsCookieAuthenticated;
        }

        public async Task<T> CASLoginWithRedirection<T>(string username, string password, bool rememberMe, IPassportServiceRedirection _redir)
        {
            //Step 1 - Request login ticket
            TicketInfo loginTicketInfo = await GetLoginTicketInfo();

            // Step 2 - build the login request
            UriRelative loginUri = new UriRelative(GetEndpointURL(LOGIN_WS));
            loginUri.AddQueryParameter("service", _redir.GetServiceURL());
            loginUri.AddQueryParameter("lt", loginTicketInfo.lt);
            loginUri.AddQueryParameter("username", username);
            loginUri.AddQueryParameter("password", password);
            loginUri.AddQueryParameter("rememberMe", rememberMe.ToString());

            PostHttpJsonMessage loginRequest = new PostHttpJsonMessage(loginUri);

            HttpResponseMessage loginRequestResponse = await Client.SendAsync(loginRequest);

            if (!loginRequestResponse.IsSuccessStatusCode)
            {
                //handle according to established exception policy
                throw new AuthenticationException(loginRequestResponse);
            }

            if (!IsCookieAuthenticated)
            {
                //handle according to established exception policy
                throw new AuthenticationException(loginRequestResponse);
            }

            //Handle ticket login response
            return await loginRequestResponse.Content.ReadFromJsonAsync<T>();
        }

        /*IPassportAuthentication interface*/
        public bool IsCookieAuthentication()
        {
            return true;
        }

        public bool AuthenticateRequest(HttpRequestMessage request, bool refreshAutomatically = true)
        {
            throw new NotImplementedException();
        }

        public CookieContainer GetCookieContainer()
        {
            return CookieContainer;
        }

        public string GetIdentity()
        {
            throw new NotImplementedException();
        }

        public bool IsValid()
        {
            return IsCookieAuthenticated;
        }

        //
    }
}
