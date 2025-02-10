using System;
using UnityEngine;
using Newtonsoft.Json;

namespace Services.Storage
{
    public class PlayerPrefsStorageService
    {
        public void Save(string key, object data)
        {
            string file = JsonConvert.SerializeObject(data);
            PlayerPrefs.SetString(key, file);
        }

        public bool TryLoad<T>(string key, out T result)
        {
            if (PlayerPrefs.HasKey(key))
            {
                string file = PlayerPrefs.GetString(key);
                result = JsonConvert.DeserializeObject<T>(file);
                return true;
            }
            result = default;
            return false;
        }
    }
}
