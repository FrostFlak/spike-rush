using DG.Tweening;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class TapToPlayUI : MonoBehaviour {

        #region SerializedFields
        [Header("Components")]
        [SerializeField] private Button _tapToPlayBtn;
        [SerializeField] private TMP_Text _tapToPlayLabel;
        #endregion

        #region PrivateFields
        private const float FadeDuration = .35f;
        
        #endregion

        #region Initialization
        public void Initialize() {
            Main.Instance.IsStarted.OnUpdate += OnIsStartedChange;
            
            _tapToPlayBtn.onClick.AddListener(OnTapToPlayClick);
        }
        
        public void Deinitialize() {
            Main.Instance.IsStarted.OnUpdate -= OnIsStartedChange;
            
            _tapToPlayBtn.onClick.RemoveAllListeners();
        }
        #endregion

        #region Events
        private void OnIsStartedChange(bool arg1, bool started) {
            _tapToPlayBtn.interactable = !started;
            _tapToPlayLabel.DOFade(started ? 0f : 1f, FadeDuration);
        }
        
        private void OnTapToPlayClick() {
            if (Main.Instance.IsStarted.Value())
                return;
            
            Main.Instance.IsStarted.Set(true);
        }
        #endregion

    }
}