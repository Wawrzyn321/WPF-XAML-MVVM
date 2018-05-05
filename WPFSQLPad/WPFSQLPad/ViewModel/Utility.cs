using System.Collections.Generic;

namespace WPFSQLPad.ViewModel
{
    public static class Utility
    {
        //return the last element. Similar to C++ .back()
        public static T Back<T>(this IList<T> collection)
        {
            if (collection == null || collection.Count == 0) return default(T);

            return collection[collection.Count - 1];
        }
    }
}
