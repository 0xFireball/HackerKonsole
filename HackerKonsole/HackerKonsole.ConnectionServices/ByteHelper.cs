using OmniBean.PowerCrypt4.Utilities;

namespace HackerKonsole.ConnectionServices
{
    public static class ByteHelper
    {
        public static string GetString(this byte[] bytes)
        {
            return ByteConverter.GetString(bytes);
        }

        public static byte[] GetBytes(this string str)
        {
            return ByteConverter.GetBytes(str);
        }
    }
}