#if ITCH
using System;

namespace Helpers.SDK {
    public class ItchSDK : SDKBase {
        
        public ItchSDK(Action onInitialized) : base(onInitialized) {
            IsInitialized = true;
            onInitialized?.Invoke();
        }
        
        public override void StartGameplay() => Log.Debug("Start Game for Itch SDK");
        public override void StopGameplay() => Log.Debug("Stop Game for Itch SDK");
    }
}
#endif