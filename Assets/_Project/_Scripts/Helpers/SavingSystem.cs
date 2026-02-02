using System;
using Helpers.SDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Helpers {
    public static class SavingSystem {
        
        private const string PREFS_PREFIX = "save_";

        static SavingSystem() {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings {
                Formatting = Formatting.None,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = {
                    new StringEnumConverter(),
                    new ObservableConverter(),
                },
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        public static void Save<T>(T data, string key) {
            if (data == null)
            {
                Log.Warning($"Trying to save null data for key: {key}");
                return;
            }

            try
            {
                string json = JsonConvert.SerializeObject(data);
                string prefsKey = PREFS_PREFIX + key;

                SDKController.Instance.SDK.SetString(prefsKey, json);

                // Optional: very verbose logging for debug builds
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Log.Debug($"Saved [{key}] ({json.Length} chars)");
#endif
            }
            catch (Exception ex) {
                Log.Error($"Failed to save '{key}': {ex.Message}");
            }
        }

        public static T Load<T>(string key, T defaultValue = default) {
            string prefsKey = PREFS_PREFIX + key;

            if (!SDKController.Instance.SDK.HasKey(prefsKey))
                return defaultValue;

            try {
                string json = SDKController.Instance.SDK.GetString(prefsKey);
                T data = JsonConvert.DeserializeObject<T>(json);
                return data;
            }
            catch (System.Exception ex) {
                Log.Error($"Failed to load '{key}': {ex.Message}\nData may be corrupted → using default");
                return defaultValue;
            }
        }

        public static bool Exists(string key) => SDKController.Instance.SDK.HasKey(PREFS_PREFIX + key);

        public static void Delete(string key) {
            string prefsKey = PREFS_PREFIX + key;
            if (SDKController.Instance.SDK.HasKey(prefsKey))
            {
                SDKController.Instance.SDK.DeleteKey(prefsKey);
                Log.Debug($"Deleted save key: {key}");
            }
        }

        public static void DeleteAll() {
            SDKController.Instance.SDK.DeleteAll();
            SDKController.Instance.SDK.Save();
            Log.Warning("Saves data has been deleted");
        }

        /// <summary>
        /// Creates default value if no save exists
        /// </summary>
        public static T GetOrCreate<T>(string key, T defaultValue = default)
        {
            if (Exists(key))
                return Load(key, defaultValue);

            T valueToSave = defaultValue ?? Activator.CreateInstance<T>();
            Save(valueToSave, key);
            Log.Debug($"Created new default save for '{key}'");

            return valueToSave;
        }
    }

    #region Observable Converter
    // Converts Observable<T> to its inner Value for JSON
    public class ObservableConverter : JsonConverter {
        public override bool CanConvert(Type objectType) => objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Observable<>);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            // Call Value() method instead of property
            var method = value.GetType().GetMethod("Value");
            var val = method.Invoke(value, null);
            serializer.Serialize(writer, val);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            var innerType = objectType.GetGenericArguments()[0];
            var val = serializer.Deserialize(reader, innerType);

            // Use constructor that accepts the value
            var instance = Activator.CreateInstance(objectType, val);
            return instance;
        }
    }
    #endregion
}
