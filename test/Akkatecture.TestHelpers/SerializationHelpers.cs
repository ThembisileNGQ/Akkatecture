using Newtonsoft.Json;

namespace Akkatecture.TestHelpers
{
    public static class SerializationHelpers
    {
        public static T SerializeDeserialize<T>(this T message)
        {
            var json = JsonConvert.SerializeObject(message);
            var obj = JsonConvert.DeserializeObject<T>(json);
            return obj;
        }
    }
}