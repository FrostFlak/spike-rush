using UnityEngine.SceneManagement;

namespace Helpers.SDK {
    public class SDKController : SingletonMonoBehaviour<SDKController> {

        #region Properties
        public SDKBase SDK { get; private set; }
        #endregion

        #region Behaviour
        protected override void Awake() {
            base.Awake();
            
            #if CRAZY_GAMES
            SDK = new CrazyGamesSDK(onInitialized: OnSDKInitialized);
            #elif ANDROID
            SDK = new AndroidSDK(onInitialized: OnSDKInitialized);
            #elif ITCH
            SDK = new ItchSDK(onInitialized: OnSDKInitialized);
            #endif
        }
        #endregion

        #region Events
        private void OnSDKInitialized() => SceneManager.LoadScene(1); // Load first scene, scene with index 0 is loading scene
        #endregion
    }
}