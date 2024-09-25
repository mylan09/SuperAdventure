using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Converter
{
    public abstract class JsonCreationConverter<T> : JsonConverter
    {
        protected abstract T Create(Type objectType, JObject jObject);

        public override bool CanConvert(Type objectType)
        {
            return typeof(T) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType,
            object existingValue, JsonSerializer serializer)
        {
            try
            {
                var jObject = JObject.Load(reader);
                var target = Create(objectType, jObject);
                serializer.Populate(jObject.CreateReader(), target);
                return target;
            }
            catch (JsonReaderException)
            {
                return null;
            }
        }

        public override void WriteJson(JsonWriter writer, object value,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class ItemConverter : JsonCreationConverter<Item>
    {
        protected override Item Create(Type objectType, JObject jObject)
        {
            try
            {
                var tt = jObject["$type"].Value<string>();
                if (tt.Contains("Healing"))
                {
                    return new HealingPotion(100, "qwer", "qerwerwe", 5, 10);
                }
                //switch ((objectType)jObject["Type"].Value<int>())
                //{
                //    case Constants.AnimalType.Cat:
                //        return new Cat();
                //    case Constants.AnimalType.Dog:
                //        return new Dog();
                //    case Constants.AnimalType.Pig:
                //        return new Pig();
                //}
            }
            catch (Exception) { }
            return null;
        }
    }
}
