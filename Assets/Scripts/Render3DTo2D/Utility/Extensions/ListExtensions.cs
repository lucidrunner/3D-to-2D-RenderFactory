using System.Collections.Generic;

namespace Render3DTo2D.Utility.Extensions
{
    public static class ListExtensions
    {
        public static bool IsNullOrEmpty<T>(this List<T> aList)
        {
            return aList == null || aList.Count == 0;
        }
    }
}