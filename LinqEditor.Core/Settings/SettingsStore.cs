﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LinqEditor.Core.Settings
{
    public class SettingsStore : ApplicationSettings, ISettingsStore
    {
        [JsonProperty]
        private IDictionary<string, object> _settings = new Dictionary<string, object>();
        
        [JsonIgnore]
        public int MainX 
        {
            get { return Get<int>("mainX"); }
            set { Set("mainX", value); }
        }

        [JsonIgnore]
        public int MainY 
        {
            get { return Get<int>("mainY"); }
            set { Set("mainY", value); }
        }

        [JsonIgnore]
        public bool MainMaximized 
        {
            get { return Get<bool>("mainMaximized"); }
            set { Set("mainMaximized", value); }
        }

        [JsonIgnore]
        public int ConnectionManagerX 
        {
            get { return Get<int>("connectionManagerX"); }
            set { Set("connectionManagerX", value); }
        }

        [JsonIgnore]
        public int ConnectionManagerY 
        {
            get { return Get<int>("connectionManagerY"); }
            set { Set("connectionManagerY", value); }
        }

        [JsonIgnore]
        public Guid LastConnectionUsed 
        {
            get { return Get<Guid>("lastConnectionUsed"); }
            set { Set("lastConnectionUsed", value); }
        }

        void Set(string key, object val)
        {
            var modified = _settings.ContainsKey(key) && !_settings[key].Equals(val) ||
                !_settings.ContainsKey(key);
            _settings[key] = val;
            if (modified)
            {
                Save();
            }
        }

        T Get<T>(string key)
        {
            if (_settings.ContainsKey(key))
            {
                return (T) _settings[key];
            }
            return default(T);
        }

        public static SettingsStore Instance
        {
            get
            {
                SettingsStore store = Read<SettingsStore>();
                if (store == null)
                {
                    store = new SettingsStore()
                    {
                        LastConnectionUsed = ConnectionStore.CodeId
                    };
                }
                return store;
            }
        }
    }
}
