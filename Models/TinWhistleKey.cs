using System.Collections.Generic;
using Newtonsoft.Json.Linq;
#nullable disable

namespace WhistleSharp.Models {
    public class TinWhistleKey {
        public        string                     Name  { get; set; }
        public Dictionary<string, string> Notes { get; set; } = new();

        public TinWhistleKey(string name, Dictionary<string, string> notes) {
            Name = name;
            Notes = notes;
        }

        public static implicit operator TinWhistleKey(Dictionary<string, object> key) {
            if (key.TryGetValue("name", out var value) && key.TryGetValue("notes", out var notes) && notes is JObject jObject) {
                var dictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(jObject.ToString());
                return new TinWhistleKey((string)value, dictionary);
            }

            return new TinWhistleKey("", new Dictionary<string, string>());
        }
    }
}