namespace Render3DTo2D.Utility.Extensions
{
    public static class ArrayExtensions
    {
        public static bool IsNullOrEmpty<T>(this T[] aArray)
        {
            return aArray == null || aArray.Length == 0;
        }
    }
}