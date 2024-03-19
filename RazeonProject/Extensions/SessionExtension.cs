using MvcCoreSession.Helper.HelperBinarySection;

namespace MvcCoreSession.Extensions
{
    public static class SessionExtension
    {
        public static T? GetObject<T>(this ISession session, string key)
        {
            string? json = session.GetString(key);
            if (json == null)
            {
                return default(T);
            }
            else
            {
                T? data = HelperJson.JsonToObject<T>(json);
                return data;
            }
        }

        public static void SetObject(this ISession session, string key, object value)
        {
            string data = HelperJson.ObjectToJson(value);
            session.SetString(key, data);
        }
    }

}
