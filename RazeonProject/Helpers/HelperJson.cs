using System.Text.Json;

namespace MvcCoreSession.Helper.HelperBinarySection
{
    public class HelperJson
    {
        public HelperJson() { }

        public static string ObjectToJson(Object objeto)
        {
            string ObjectSerializer = JsonSerializer.Serialize(objeto);
            return ObjectSerializer;
        }

        public static T? JsonToObject<T>(string Json)
        {
            var objeto = JsonSerializer.Deserialize<T>(Json);
            return objeto;
        }
    }
}
