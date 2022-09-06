// ------------------------------------------------------------------------------------------------------------------------------------
// Copyright 2022 Dassault Systèmes - CPE EMED
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
// ------------------------------------------------------------------------------------------------------------------------------------


using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web;
using ds.authentication;
using ds.enovia.bookmark.exception;
using ds.enovia.bookmark.mask;
using ds.enovia.bookmark.model;
using ds.enovia.common.collection;
using ds.enovia.common.search;
using ds.enovia.service;

namespace ds.enovia.bookmark.service
{
    public class BookmarkService : EnoviaBaseService, I3DXSearchable<Bookmark>
    {
        private const string BASE_RESOURCE = "/resources/v1/modeler/dsbks/dsbks:Bookmark";

        private const string SEARCH = "/search";

        public string GetBaseResource()
        {
            return BASE_RESOURCE;
        }

        public BookmarkService(string _enoviaService, IPassportAuthentication passport) : base(_enoviaService, passport)
        {

        }


        public async Task<NlsLabeledItemSet<Bookmark>> GetBookmark(string _bookmarkId)
        {
            string getBookmark = $"{GetBaseResource()}/{_bookmarkId}";

            HttpResponseMessage requestResponse = await GetAsync(getBookmark);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new GetBookmarkException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<Bookmark>>();
        }

        #region Search

        public async Task<List<NlsLabeledItemSet<Bookmark>>> SearchAll(SearchQuery _searchString)
        {
            return await SearchUtils<Bookmark>.SearchAllAsync(this, _searchString);
        }

        public async Task<NlsLabeledItemSet<Bookmark>> SearchAsync(SearchQuery _searchString, long _skip, long _top, string _mask = null)
        {
            return await _Search(_searchString.GetSearchString(), _skip, _top);
        }

        private async Task<NlsLabeledItemSet<Bookmark>> _Search(string _searchString, long _skip = 0, long _top = 100, BookmarkSearchMask _mask = BookmarkSearchMask.Details)
        {
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", _mask.GetString());
            queryParams.Add("$skip", _skip.ToString());
            queryParams.Add("$top", _top.ToString());
            queryParams.Add("$searchStr", HttpUtility.UrlEncode(_searchString));

            string searchResource = string.Format("{0}{1}", GetBaseResource(), SEARCH);

            HttpResponseMessage requestResponse = await GetAsync(searchResource, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new SearchBookmarkException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<Bookmark>>();
        }
        #endregion
    }
}
