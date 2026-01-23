using DG.Tweening;
using Game;
using Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class EndUI : MonoBehaviour {

        #region SerializedFields
        [Header("RectTransforms")]
        [SerializeField] private RectTransform _panel;
        [SerializeField] private RectTransform _bgPanel;
        [SerializeField] private Image _fadePanel;
        [Header("Buttons")]
        [SerializeField] private Button _xButton;
        [SerializeField] private Button _takeBtn;
        [Header("Labels")]
        [SerializeField] private TMP_Text _coinsLabel;
        [SerializeField] private TMP_Text _diamondsLabel;
        [SerializeField] private TMP_Text _distanceLabel;
        [SerializeField] private TMP_Text _newRecordLabel;
        #endregion

        #region PrivateFields
        private const float PanelPopDuration = .35f;
        private const float FadeDuration = 1f;
        
        #endregion

        #region Initialization
        public void Initialize() {
            Main.Instance.StateManager.State.OnUpdate += OnGameStateChange;
            
            _xButton.onClick.AddListener(OnClickTakeWithMultiplier);
            _takeBtn.onClick.AddListener(OnClickTake);
        }

        public void Deinitialize() {
            _fadePanel.DOKill();
            
            Main.Instance.StateManager.State.OnUpdate -= OnGameStateChange;
            
            _xButton.onClick.RemoveAllListeners();
            _takeBtn.onClick.RemoveAllListeners();
        }
        #endregion

        #region UI
        private void SetState(bool active) {
            _bgPanel.gameObject.SetActive(active);
            _panel.gameObject.SetActive(true);
            _panel.Pop(
                active,
                PanelPopDuration,
                onComplete: () => {
                    if (!active)
                        _panel.gameObject.SetActive(false);
                }
            );
        }
        
        private void OnClickTakeWithMultiplier() {
            // Show Rewarded Ad
        }
        
        private void OnClickTake() {
            // add money
            _fadePanel.raycastTarget = true;
            _fadePanel
                .DOFade(1, FadeDuration)
                .OnComplete(() => {
                    Main.Instance.StateManager.State.Set(StateManager.GameState.GatheredReward);
                    _fadePanel.DOFade(0, FadeDuration).OnComplete(() => _fadePanel.raycastTarget = false);
                });
            
            SetState(false);
        }
        
        private void SetLabels() {
            // _coinsLabel.SetText();
        }
        #endregion

        #region Events
        private void OnGameStateChange(StateManager.GameState arg1, StateManager.GameState state) {
            if (state is StateManager.GameState.Lose or StateManager.GameState.Win)
                SetState(true);
        }
        #endregion
    }
}