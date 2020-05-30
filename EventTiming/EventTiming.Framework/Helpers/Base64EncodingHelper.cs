using System;
using System.Text;

namespace EventTiming.Framework.Helpers
{
    public class Base64EncodingHelper
    {
        public static string DecodeString(string encodedString)
        {
            if (string.IsNullOrEmpty(encodedString))
                return null;

            byte[] decodedBytes = null;

            //TODO: переделать
            //если строка не base64, то просто возвращаем исходный вариант
            try
            {
                decodedBytes = Convert.FromBase64String(encodedString);
            }
            catch (Exception)
            {
                return encodedString;
            }

            return System.Text.Encoding.UTF8.GetString(decodedBytes);
        }

        public static string EncodeString(string strToEncode)
        {
            if (string.IsNullOrEmpty(strToEncode))
                return null;

            var strBytes = Encoding.UTF8.GetBytes(strToEncode);
            return Convert.ToBase64String(strBytes);
        }
    }
}
