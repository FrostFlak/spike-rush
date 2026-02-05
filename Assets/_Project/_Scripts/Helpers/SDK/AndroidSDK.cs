using System;

namespace Helpers.SDK {
    public class AndroidSDK : SDKBase {
        public AndroidSDK(Action onInitialized) : base(onInitialized) {
            IsInitialized = true;
            onInitialized?.Invoke();
        }

        public override void StartGameplay() => Log.Debug("Start Game for Android SDK");
        public override void StopGameplay() => Log.Debug("Stop Game for Android SDK");
    }
}