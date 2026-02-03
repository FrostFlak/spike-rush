using System;
using UnityEngine;

namespace Helpers.SDK {
    public abstract class SDKBase {

        #region Properties
        public bool IsInitialized { get; protected set; }
        #endregion

        #region Cosntructor
        protected SDKBase(Action onInitialized) { }
        #endregion

        #region Game
        public abstract void StartGame();
        public abstract void StopGame();

        public virtual void HappyTime() { }
        #endregion

        #region Prefs
        public virtual void SetInt(string key, int value) => PlayerPrefs.SetInt(key, value);
        public virtual int GetInt(string key, int defaultValue = 0) => PlayerPrefs.GetInt(key, defaultValue);
        public virtual void SetFloat(string key, float value) => PlayerPrefs.SetFloat(key, value);
        public virtual float GetFloat(string key, float defaultValue = 0) => PlayerPrefs.GetFloat(key, defaultValue);
        public virtual void SetString(string key, string value) => PlayerPrefs.SetString(key, value);
        public virtual string GetString(string key, string defaultValue = null) => PlayerPrefs.GetString(key, defaultValue);
        public virtual bool HasKey(string key) => PlayerPrefs.HasKey(key);
        public virtual void DeleteKey(string key) => PlayerPrefs.DeleteKey(key);
        public virtual void DeleteAll() => PlayerPrefs.DeleteAll();
        public virtual void Save() => PlayerPrefs.Save();
        #endregion
    }
}