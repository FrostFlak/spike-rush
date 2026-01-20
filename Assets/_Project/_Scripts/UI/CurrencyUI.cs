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

        #region PrivateFields
        private Models.Currency _currencyData;
        #endregion
        
        #region Initialization
        public void Initialize(Models.Currency currency) {
            _currencyData = currency;

            _currencyData.Coins.OnUpdate += OnCoinsChange;
            _currencyData.Diamonds.OnUpdate += OnDiamondsChange;

            SetCoinsLabels();
            SetDiamondsLabels();
        }

        public void Deinitialize() {
            _currencyData.Coins.OnUpdate -= OnCoinsChange;
            _currencyData.Diamonds.OnUpdate -= OnDiamondsChange;
        }
        #endregion

        #region UI
        private void SetCoinsLabels() => _coinsLabel.SetText(_currencyData.Coins.Value().ToString());
        
        private void SetDiamondsLabels() => _diamondsLabel.SetText(_currencyData.Diamonds.Value().ToString());
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