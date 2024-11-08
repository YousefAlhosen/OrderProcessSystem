using Newtonsoft.Json;

namespace OrderProcessSystem.Services
{
    public class JsonSerializerService : IJsonSerializerService
    {
        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

    }
}
