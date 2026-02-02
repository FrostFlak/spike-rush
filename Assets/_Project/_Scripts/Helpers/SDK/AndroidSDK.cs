using System;

namespace Helpers.SDK {
    public class AndroidSDK : SDKBase {
        public AndroidSDK(Action onInitialized) : base(onInitialized) {
            IsInitialized = true;
            onInitialized?.Invoke();
        }

        public override void StartGame() {
            Log.Debug("Start Game for Android SDK");
        }
        public override void StopGame() {
            Log.Debug("Stop Game for Android SDK");
        }
    }
}