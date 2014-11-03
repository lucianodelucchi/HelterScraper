namespace HelterScraper.Extensions
{
    static class ByteArray
    {
        public static string EncodeToString(this byte[] value)
        {
            return System.Text.Encoding.UTF8.GetString(value);
        }
    }
}
