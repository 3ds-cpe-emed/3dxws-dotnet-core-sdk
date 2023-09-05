using ds.enovia.common.collection;
using ds.enovia.common.model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ds.enovia.dseng.tests
{
   public static class BulkFetchUtils<T> where T : Item
   {
      public static List<T> AsList(List<NlsLabeledItemSet<T>> _totalResultAsCollection)
      {
         List<T> __items = new List<T>();

         foreach (NlsLabeledItemSet<T> partialSearchResult in _totalResultAsCollection)
         {
            __items.AddRange(partialSearchResult.member);
         }

         return __items;
      }

      public static List<List<T>> Split(List<T> _wholeList, int _splitSize = 50)
      {
         int wholeListCount = _wholeList.Count;

         int maxLoop = Math.DivRem(wholeListCount, _splitSize, out int rem);

         List<List<T>> __splitLists = new List<List<T>>();

         for (int i = 0; i < maxLoop; i++)
         {
            __splitLists.Add(_wholeList.GetRange(i * _splitSize, _splitSize));
         }

         if (rem > 0)
         {
            __splitLists.Add(_wholeList.GetRange(maxLoop * _splitSize, rem));
         }

         return __splitLists;
      }

      public static List<List<string>> GetSplitIds(List<T> _wholeList, int _splitSize = 50)
      {
         List<List<string>> __splitIdList = new List<List<string>>();

         List<List<T>> splitList = Split(_wholeList, _splitSize);

         for (int i = 0; i < splitList.Count; i++)
         {
            List<T> partialList = splitList[i];

            List<string> __partialIdList = new List<string>();

            __splitIdList.Add(__partialIdList);

            for (int j = 0; j < partialList.Count; j++)
            {
               __partialIdList.Add(partialList[j].id);
            }
         }

         return __splitIdList;
      }
   }
}
