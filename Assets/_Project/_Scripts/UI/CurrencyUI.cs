using DG.Tweening;
using Game;
using TMPro;
using UnityEngine;

namespace UI {
    public class CurrencyUI : MonoBehaviour {
        
        #region SerializedFields
        [Header("Rect")]
        [SerializeField] private RectTransform _parentRect;
        [Header("Coins")]
        [SerializeField] private TMP_Text _coinsLabel;
        [Header("Diamonds")]
        [SerializeField] private TMP_Text _diamondsLabel;
        #endregion

        #region PrivateFields
        private const float XPosition = 250;
        private const float SlideDuration = .75f;
        #endregion

        #region Initialization
        public void Initialize() {
            Main.Instance.StateManager.State.OnUpdate += OnGameStateChange;
            Main.Instance.CurrencyData.Coins.OnUpdate += OnCoinsChange;
            Main.Instance.CurrencyData.Diamonds.OnUpdate += OnDiamondsChange;

            SetCoinsLabels();
            SetDiamondsLabels();
        }

        public void Deinitialize() {
            Main.Instance.StateManager.State.OnUpdate -= OnGameStateChange;
            Main.Instance.CurrencyData.Coins.OnUpdate -= OnCoinsChange;
            Main.Instance.CurrencyData.Diamonds.OnUpdate -= OnDiamondsChange;
        }
        #endregion

        #region UI
        private void SetCoinsLabels() => _coinsLabel.SetText(Main.Instance.CurrencyData.Coins.Value().ToString());
        
        private void SetDiamondsLabels() => _diamondsLabel.SetText(Main.Instance.CurrencyData.Diamonds.Value().ToString());
        
        private void SlideParentPanel(bool active) => _parentRect.DOAnchorPosX(active ? XPosition : -XPosition, SlideDuration).SetEase(Ease.OutBounce);
        #endregion

        #region Events
        private void OnCoinsChange(int arg1, int arg2) {
            SetCoinsLabels();
        }
        
        private void OnDiamondsChange(int arg1, int arg2) {
            SetDiamondsLabels();
        }
        
        private void OnGameStateChange(StateManager.GameState _, StateManager.GameState state) {
            if (state is StateManager.GameState.Playing)
                SlideParentPanel(false);
            else if (state is StateManager.GameState.GatheredReward) 
                SlideParentPanel(true);
        }
        #endregion
    }
}