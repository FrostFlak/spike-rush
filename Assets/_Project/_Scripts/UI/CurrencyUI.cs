using Game;
using TMPro;
using UnityEngine;

namespace UI {
    public class CurrencyUI : MonoBehaviour {
        
        #region SerializedFields
        [Header("Coins")]
        [SerializeField] private TMP_Text _coinsLabel;
        [Header("Diamonds")]
        [SerializeField] private TMP_Text _diamondsLabel;
        #endregion

        #region Initialization
        public void Initialize() {
            Main.Instance.CurrencyData.Coins.OnUpdate += OnCoinsChange;
            Main.Instance.CurrencyData.Diamonds.OnUpdate += OnDiamondsChange;

            SetCoinsLabels();
            SetDiamondsLabels();
        }

        public void Deinitialize() {
            Main.Instance.CurrencyData.Coins.OnUpdate -= OnCoinsChange;
            Main.Instance.CurrencyData.Diamonds.OnUpdate -= OnDiamondsChange;
        }
        #endregion

        #region UI
        private void SetCoinsLabels() => _coinsLabel.SetText(Main.Instance.CurrencyData.Coins.Value().ToString());
        
        private void SetDiamondsLabels() => _diamondsLabel.SetText(Main.Instance.CurrencyData.Diamonds.Value().ToString());
        #endregion

        #region Events
        private void OnCoinsChange(int arg1, int arg2) {
            SetCoinsLabels();
        }
        
        private void OnDiamondsChange(int arg1, int arg2) {
            SetDiamondsLabels();
        }    
        #endregion
    }
}