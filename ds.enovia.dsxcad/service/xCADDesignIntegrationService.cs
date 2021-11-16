//------------------------------------------------------------------------------------------------------------------------------------
// Copyright 2021 Dassault Systèmes - CPE EMED
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

using ds.authentication;
using ds.enovia.common.collection;
using ds.enovia.common.model;
using ds.enovia.common.search;
using ds.enovia.dsxcad.model;
using ds.enovia.service;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web;

namespace ds.enovia.dsxcad.service
{
    public abstract class xCADDesignIntegrationService : EnoviaBaseService 
    {
        private const string DOWNLOAD_TICKET = "/{0}/dsxcad:AuthoringFile/DownloadTicket";
        private const string SEARCH = "/search";

        private const long MAX_VALS_PER_QUERY = 1000;

        public xCADDesignIntegrationService(string _enoviaService, IPassportAuthentication passport) : base(_enoviaService, passport)
        {
        }

        public abstract string GetBaseResource();

        public async Task<IList<T>> SearchAll<T>(SearchQuery _searchString, string _mask = null, long _top = MAX_VALS_PER_QUERY)
        {
            long skip = 0;

            List<ItemSet<T>> searchBookReturn = new List<ItemSet<T>>();

            ItemSet<T> page;

            do
            {
                page = await _Search<T>(_searchString, _mask, skip, _top);

                skip += _top;

                //TODO: Add an interval

                searchBookReturn.Add(page);
            }
            while ((page != null) && (page.totalItems == _top));

            // Postprocessing

            List<T> __searchOutput = new List<T>();

            foreach (ItemSet<T> itemSet in searchBookReturn)
            {
                if (itemSet.totalItems > 0)
                {
                    __searchOutput.AddRange(itemSet.member);
                }
            }

            return __searchOutput;
        }

        //Important: Queries must not exceed 4096 characters.
        protected async Task<ItemSet<T>> _Search<T>(SearchQuery _searchString, string _mask = null, long _skip = 0, long _top = 100) 
        {
            string searchString = _searchString.GetSearchString();

            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            if (_mask != null)
                queryParams.Add("$mask", _mask);
            
            queryParams.Add("$skip", _skip.ToString());
            queryParams.Add("$top", _top.ToString());
            queryParams.Add("$searchStr", searchString);

            string searchResource = string.Format("{0}{1}", GetBaseResource(), SEARCH);

            HttpResponseMessage requestResponse = await GetAsync(searchResource, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                //throw (new DerivedOutputException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<ItemSet<T>>();
        }
        protected async Task<FileDownloadTicket> GetAuthoringFileDownloadTicket(string _baseDownloadUrl, string _id)
        {
            string downloadTicketResource = string.Format(_baseDownloadUrl + DOWNLOAD_TICKET, _id);

            HttpResponseMessage requestResponse = await PostAsync(downloadTicketResource, null, null, "{}");

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                //throw (new DerivedOutputException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<FileDownloadTicket>();
        }

        protected async Task<FileInfo> DownloadFile(HttpClient _downloadHttpClient, FileDownloadTicket _ticket, string _downloadLocation)
        {
            string filename = _ticket.filename;

            string downloadFile = string.Format("{0}{1}", _downloadLocation, filename);

            string downloadUrl = string.Format("{0}?{1}={2}", _ticket.ticketURL, _ticket.jobticket, HttpUtility.UrlEncode(_ticket.ticket));

            if (File.Exists(downloadFile))
                File.Delete(downloadFile);

           HttpResponseMessage res = await _downloadHttpClient.GetAsync(downloadUrl);
            
            using (var writer = File.OpenWrite(downloadFile))
            {
                    using (Stream streamToReadFrom = await res.Content.ReadAsStreamAsync())
                    {
                        streamToReadFrom.CopyTo(writer);
                    }
                //}
            }

            return new FileInfo(downloadFile);
        }
    }
}
