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
        [SerializeField] private TMP_Text _outOfFuelLabel;
        [SerializeField] private TMP_Text _levelReachedLabel;
        [SerializeField] private TMP_Text _newRecordLabel;
        [SerializeField] private TMP_Text _coinsLabel;
        [SerializeField] private TMP_Text _diamondsLabel;
        #endregion

        #region PrivateFields
        private const float PanelPopDuration = .35f;
        private const float FadeDuration = 1f;
        
        private int _adMultiplier = 1;
        #endregion

        #region Initialization
        public void Initialize() {
            Main.Instance.StateManager.State.OnUpdate += OnGameStateChange;
            Main.Instance.LastRecordDistance.OnUpdate += OnLastRecordDistanceChange;
            
            _xButton.onClick.AddListener(OnClickTakeWithMultiplier);
            _takeBtn.onClick.AddListener(OnClickTake);
            _diamondsLabel.gameObject.SetActive(false);
            _newRecordLabel.alpha = 0f;
        }
        
        public void Deinitialize() {
            _fadePanel.DOKill();
            
            Main.Instance.StateManager.State.OnUpdate -= OnGameStateChange;
            Main.Instance.LastRecordDistance.OnUpdate -= OnLastRecordDistanceChange;
            
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
            
            if (!active)
                return;
            
            SetLabels();
        }
        
        private void OnClickTakeWithMultiplier() {
            // Show Rewarded Ad
            _adMultiplier = 3;
            OnClickTake();
            
            _adMultiplier = 0;
        }
        
        private void OnClickTake() {
            Main.Instance.CurrencyData.Add(Models.CurrencyType.Diamond, Main.Instance.RunReceivedDiamonds * _adMultiplier);
            Main.Instance.RunReceivedDiamonds = 0;
            
            Main.Instance.CurrencyData.Add(Models.CurrencyType.Coin, Main.Instance.RunReceivedCoins * _adMultiplier);
            Main.Instance.RunReceivedCoins = 0;
            
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
            if (Main.Instance.StateManager.State.Value() is StateManager.GameState.Win) {
                _outOfFuelLabel.gameObject.SetActive(false);
                _levelReachedLabel.gameObject.SetActive(true);
            }
            else if (Main.Instance.StateManager.State.Value() is StateManager.GameState.Lose) {
                _outOfFuelLabel.gameObject.SetActive(true);
                _levelReachedLabel.gameObject.SetActive(false);
            }
            
            _coinsLabel.LerpLabelCount(0, Main.Instance.RunReceivedCoins, duration: 1f, onComplete: () => _coinsLabel.SetText($"+{_coinsLabel.text}"));
            AudioController.Instance.PlayUI(AudioController.UISFX.MultipleCoins);
            
            if (Main.Instance.RunReceivedDiamonds <= 0) 
                return;
            
            _diamondsLabel.gameObject.SetActive(true);
            _diamondsLabel.LerpLabelCount(0, Main.Instance.RunReceivedDiamonds, duration: 1f, onComplete: () => _diamondsLabel.SetText($"+{_diamondsLabel.text}"));
        }
        #endregion

        #region Events
        private void OnGameStateChange(StateManager.GameState arg1, StateManager.GameState state) {
            if (state is StateManager.GameState.Lose or StateManager.GameState.Win)
                SetState(true);
            else if (state is StateManager.GameState.GatheredReward) 
                _newRecordLabel.alpha = 0f;
        }
        
        private void OnLastRecordDistanceChange(int arg1, int arg2) => _newRecordLabel.alpha = 1f;
        #endregion
    }
}