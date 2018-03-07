using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoStar.Items
{
    class ItemBase
    {
        public static Dictionary<string, ItemBase> ItemDictionary;
        public static void LoadItemDictionary()
        {
            ItemDictionary = new Dictionary<string, ItemBase>();
        }

        private uint id;
        public uint ID { get => id; set => id = value; }
        public string Name { get; set; }
        public float Weight { get; set; }

        public ItemBase()
        {
            Name = string.Empty;
            ID = 0;
            Weight = 0f;
        }

        public ItemBase(string name, float weight, uint id = 0)
        {
            Name = name;
            Weight = weight;
            ID = id;
        }

        public int CompareWeight(ItemBase other)
        {
            return Weight.CompareTo(other.Weight);
        }

        public int CompareName(ItemBase other)
        {
            if (string.IsNullOrEmpty(Name))
            {
                if (string.IsNullOrEmpty(other.Name))
                {
                    return 0;
                }
                else return 1;
            }
            else
            {
                if (string.IsNullOrEmpty(other.Name))
                {
                    return -1;
                }
                else return Name.CompareTo(other.Name);
            }
        }

        public ItemBehaviour ItemBehaviour { get; set; }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                hash = hash * 23 + Name.GetHashCode();
                hash = hash * 23 + Weight.GetHashCode();
                hash = hash * 23 + id.GetHashCode();
                return hash;
            }
        }
    }

    class ItemBaseJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ItemBase);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            ItemBase temp = (ItemBase)value;

            writer.WriteStartObject();
            writer.WritePropertyName("ID");
            serializer.Serialize(writer, temp.ID);
            writer.WritePropertyName("Name");
            serializer.Serialize(writer, temp.Name);
            writer.WritePropertyName("Weight");
            serializer.Serialize(writer, temp.Weight);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string name = string.Empty;
            float weight = 0f;
            uint id = 0;

            while (reader.Read())
            {
                if (reader.TokenType != JsonToken.PropertyName)
                {
                    break;
                }

                string propertyName = (string)reader.Value;
                if (!reader.Read())
                {
                    continue;
                }

                switch (propertyName)
                {
                    case "ID":
                        id = serializer.Deserialize<uint>(reader);
                        break;
                    case "Name":
                        name = serializer.Deserialize<string>(reader);
                        break;
                    case "Weight":
                        weight = serializer.Deserialize<float>(reader);
                        break;
                }
            }

            return new ItemBase(name, weight, id);
        }
    }
}
