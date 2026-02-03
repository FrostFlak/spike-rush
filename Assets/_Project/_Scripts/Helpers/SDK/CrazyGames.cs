using System;
using CrazyGames;

namespace Helpers.SDK {
    public class CrazyGames : SDKBase {

        #region Constructor
        public CrazyGames(Action onInitialized) : base(onInitialized) {
            if (!CrazySDK.IsAvailable) {
                Log.Error("The Crazy SDK is not available");
                return;
            }
            
            CrazySDK.Init(() => {
                if (!CrazySDK.IsInitialized)
                    return;

                IsInitialized = true;
                onInitialized?.Invoke();
                Log.Debug("Crazy games SDK successfully initialized");
            });
        }
        #endregion

        #region Game
        public override void StartGame() {
            CrazySDK.Game.GameplayStart();
            CrazySDK.Game.HideInviteButton();
            Log.Debug("Crazy games SDK gameplay started");
        }
        
        public override void StopGame() {
            CrazySDK.Game.GameplayStop();
            Log.Debug("Crazy games SDK gameplay stopped");
        }

        public override void HappyTime() {
            CrazySDK.Game.HappyTime();
            Log.Debug("Crazy games SDK happy time");
        }
        #endregion

        #region Prefs
        public override void SetInt(string key, int value) => CrazySDK.Data.SetInt(key, value);
        public override int GetInt(string key, int defaultValue = 0) => CrazySDK.Data.GetInt(key, defaultValue);
        public override void SetFloat(string key, float value) => CrazySDK.Data.SetFloat(key, value);
        public override float GetFloat(string key, float defaultValue = 0) => CrazySDK.Data.GetFloat(key, defaultValue);
        public override void SetString(string key, string value) => CrazySDK.Data.SetString(key, value);
        public override string GetString(string key, string defaultValue = null) => CrazySDK.Data.GetString(key, defaultValue);
        public override bool HasKey(string key) => CrazySDK.Data.HasKey(key);
        public override void DeleteKey(string key) => CrazySDK.Data.DeleteKey(key);
        public override void DeleteAll() => CrazySDK.Data.DeleteAll();
        public override void Save() { }
        #endregion
    }
}