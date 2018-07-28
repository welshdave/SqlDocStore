namespace SqlDocStore
{
    public class SimpleJsonSerializer : ISerializer
    {
        public string Serialize(object obj)
        {
            return SimpleJson.SerializeObject(obj);
        }

        public T Deserialize<T>(string json)
        {
            return SimpleJson.DeserializeObject<T>(json);
        }
    }
}
