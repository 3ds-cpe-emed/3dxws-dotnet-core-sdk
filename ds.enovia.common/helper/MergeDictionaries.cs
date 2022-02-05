using System;
using System.Collections.Generic;
using System.Text;

namespace ds.enovia.common.helper
{
    public static class HelperExtensions
    {
        /// <summary>
        /// Inspired by https://stackoverflow.com/questions/2132562/merging-dictionary-containing-a-list-in-c-sharp
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictLeft"></param>
        /// <param name="dictRight"></param>
        public static void Merge<T>(this IDictionary<string, IList<T>> dictLeft, IDictionary<string, IList<T>> dictRight)
        {
            foreach (var kvp2 in dictRight)
            {
                // If the dictionary already contains the key then merge them
                if (dictLeft.ContainsKey(kvp2.Key))
                {
                    foreach (T v in kvp2.Value)
                    {
                        dictLeft[kvp2.Key].Add(v);
                    }
                    continue;
                }

                dictLeft.Add(kvp2);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictLeft"></param>
        /// <param name="dictRight"></param>
        public static void Merge<T>(this IDictionary<string, IDictionary<string, IList<T>>> dictLeft, IDictionary<string, IDictionary<string, IList<T>>> dictRight)
        {
            foreach (var kvpOuter in dictRight)
            {
                // If the dictionary already contains the key then merge them
                if (dictLeft.ContainsKey(kvpOuter.Key))
                {
                    IDictionary<string, IList<T>> innerRightDictionary = kvpOuter.Value;
                    IDictionary<string, IList<T>> innerLeftDictionary  = dictLeft[kvpOuter.Key];

                    foreach (var kvpInner in innerRightDictionary)
                    {
                        if (!innerLeftDictionary.ContainsKey(kvpInner.Key))
                        {
                            innerLeftDictionary.Add(kvpInner);
                        }
                        else
                        {
                            IList<T> innerLeftList  = innerLeftDictionary[kvpInner.Key];
                            IList<T> innerRightList = innerRightDictionary[kvpInner.Key];

                            //Assuming that they are not null
                            foreach (T t in innerRightList)
                            {
                                //Do not insert duplicates
                                if (!innerLeftList.Contains(t))
                                {
                                    innerLeftList.Add(t);
                                }
                            }
                        }
                    }
                    continue;
                }

                dictLeft.Add(kvpOuter);
            }
        }
    }
}

