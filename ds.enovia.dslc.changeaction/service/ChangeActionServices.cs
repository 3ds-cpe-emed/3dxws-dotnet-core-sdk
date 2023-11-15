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

using ds.authentication;
using ds.enovia.common.model;
using ds.enovia.common.search;
using ds.enovia.dslc.changeaction.exception;
using ds.enovia.dslc.changeaction.model;
using ds.enovia.service;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web;

namespace ds.enovia.dslc.changeaction.service
{
   public class ChangeActionServices : EnoviaBaseService
   {
      private const string BASE_RESOURCE = "/resources/v1/modeler/dslc/changeaction";
      private const string SEARCH = "/search";

      // Notes from public documentation
      // Engineering Web Services 1.3.0 - Gets a list of Engineering Items.
      // By default, returns a total of up to 50 items, can be optionally increased upto 1000 items using $top query parameter.
      private const long MAX_VALS_PER_QUERY = 1000;

      public string GetBaseResource()
      {
         return BASE_RESOURCE;
      }

      public ChangeActionServices(string _enoviaService, IPassportAuthentication passport) : base(_enoviaService, passport)
      {

      }

      public async Task<ChangeAction> GetChangeAction(string _id, bool detailed = true)
      {
         string getChangeActionById = string.Format("{0}/{1}", GetBaseResource(), _id);

         Dictionary<string, string> queryParamsDictionary = null;
         if (detailed)
         {
            queryParamsDictionary = new Dictionary<string, string>
            {
               { "$fields", "members,proposedChanges,realizedChanges,referentials,contexts,customerAttributes" }
            };
         }

         HttpResponseMessage requestResponse = await GetAsync(getChangeActionById, queryParamsDictionary);

         if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
         {
            //handle according to established exception policy
            throw (new GetChangeActionException(requestResponse));
         }

         return await requestResponse.Content.ReadFromJsonAsync<ChangeAction>();
      }

      #region Search
      //<summary>Notes from public documentation
      // Engineering Web Services 1.3.0 - Gets a list of Engineering Items. By default, returns a total of up to 50 items, can be optionally increased upto 1000 items using $top query parameter.
      // Recommendation: Use $searchStr query parameter with a minimum of two characters for better performances.</summary>
      public async Task<List<BusinessObjectIdentifier>> SearchAll(SearchQuery _searchString)
      {
         return await SearchAllAsync(_searchString);
      }
      public async Task<IList<BusinessObjectIdentifier>> SearchAsync(SearchQuery _searchString, long _skip, long _top)
      {
         return await _Search(_searchString.GetSearchString(), _skip, _top);
      }

      public async Task<IList<BusinessObjectIdentifier>> Search(SearchQuery _searchString, long _skip = 0, long _top = 100)
      {
         return await SearchAsync(_searchString, _skip, _top);
      }

      //Important: Queries must not exceed 4096 characters.
      private async Task<IList<BusinessObjectIdentifier>> _Search(string _searchString, long _skip = 0, long _top = 100)
      {
         Dictionary<string, string> queryParams = new Dictionary<string, string>
         {
            { "$skip", _skip.ToString() },
            { "$top", _top.ToString() },
            { "$searchStr", HttpUtility.UrlEncode(_searchString) }
         };

         string searchResource = string.Format("{0}{1}", GetBaseResource(), SEARCH);

         HttpResponseMessage requestResponse = await GetAsync(searchResource, queryParams);

         if (!requestResponse.IsSuccessStatusCode)
         {
            //handle according to established exception policy
            throw (new SearchChangeActionException(requestResponse));
         }

         SearchChangeActionResponse searchCAResponseContent = await requestResponse.Content.ReadFromJsonAsync<SearchChangeActionResponse>();

         return searchCAResponseContent.ChangeAction;
      }

      //<summary>Notes from public documentation
      // Engineering Web Services 1.3.0 - Gets a list of Engineering Items. By default, returns a total of up to 50 items, can be optionally increased upto 1000 items using $top query parameter.
      // Recommendation: Use $searchStr query parameter with a minimum of two characters for better performances.</summary>
      public async Task<List<BusinessObjectIdentifier>> SearchAllAsync(SearchQuery _searchString, long _top = MAX_VALS_PER_QUERY)
      {
         long skip = 0;

         List<BusinessObjectIdentifier> __searchBookReturn = new List<BusinessObjectIdentifier>();

         IList<BusinessObjectIdentifier> curPage;

         do
         {
            curPage = await SearchAsync(_searchString, skip, _top);

            skip += _top;

            __searchBookReturn.AddRange(curPage);
         }
         while ((curPage != null) && (curPage.Count == _top));

         return __searchBookReturn;
      }
      #endregion
   }
}
