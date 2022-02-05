using ds.enovia.common.collection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ds.enovia.common.search
{
    public interface I3DXSearchable<T>
    {
        Task<NlsLabeledItemSet2<T>> SearchAsync(SearchQuery _searchString, long _skip, long _top, string _mask = null);

    }
    public static class SearchUtils<T>
    {
        const long MAX_VALS_PER_QUERY = 1000;

        //<summary>Notes from public documentation
        // Engineering Web Services 1.3.0 - Gets a list of Engineering Items. By default, returns a total of up to 50 items, can be optionally increased upto 1000 items using $top query parameter.
        // Recommendation: Use $searchStr query parameter with a minimum of two characters for better performances.</summary>
        public static async Task<List<NlsLabeledItemSet2<T>>> SearchAllAsync(I3DXSearchable<T> _refSearchable, SearchQuery _searchString, string _mask = null, long _top = MAX_VALS_PER_QUERY)
        {
            long skip = 0;

            List<NlsLabeledItemSet2<T>> __searchBookReturn = new List<NlsLabeledItemSet2<T>>();

            NlsLabeledItemSet2<T> page;

            do
            {
                page = await _refSearchable.SearchAsync(_searchString, skip, _top, _mask);

                skip += _top;

                //TODO: Add an interval

                __searchBookReturn.Add(page);
            }
            while ((page != null) && (page.totalItems == _top));

            //remove the last page if there are more than one pages and the last one is zero
            if ((__searchBookReturn.Count > 1) && (__searchBookReturn[__searchBookReturn.Count - 1].totalItems == 0))
            {
                __searchBookReturn.RemoveAt(__searchBookReturn.Count - 1);
            }

            return __searchBookReturn;
        }
    }
}
