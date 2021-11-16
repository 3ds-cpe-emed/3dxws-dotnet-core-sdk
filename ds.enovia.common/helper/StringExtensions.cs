namespace ds.enovia.common.helper
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return ((str == null) || (str.Equals(string.Empty)));
        }
    }
}
