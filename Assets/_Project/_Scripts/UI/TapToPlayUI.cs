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
        [SerializeField] private Image _bg;
        #endregion

        #region PrivateFields
        private const float FadeDuration = .35f;
        
        #endregion

        #region Initialization
        public void Initialize() {
            Main.Instance.StateManager.State.OnUpdate += OnGameStateChange;
            
            _tapToPlayBtn.onClick.AddListener(OnTapToPlayClick);
            _tapToPlayLabel.SetText(Application.platform != RuntimePlatform.WebGLPlayer ? "Click To Play" : "Tap To Play");
        }

        public void Deinitialize() {
            Main.Instance.StateManager.State.OnUpdate -= OnGameStateChange;
            
            _tapToPlayBtn.onClick.RemoveAllListeners();
        }
        #endregion

        #region Events
        private void OnTapToPlayClick() {
            Main.Instance.StateManager.State.Set(StateManager.GameState.Playing);
        }

        private void OnGameStateChange(StateManager.GameState arg1, StateManager.GameState state) {
            if (state is StateManager.GameState.Playing) {
                _bg.raycastTarget = false;
                _tapToPlayBtn.interactable = false;
                _tapToPlayLabel.DOFade(0f, FadeDuration);
            }
            else if (state is StateManager.GameState.GatheredReward) {
                _bg.raycastTarget = true;
                _tapToPlayBtn.interactable = true;
                _tapToPlayLabel.DOFade(1f, FadeDuration);
            }
        }
        #endregion
    }
}