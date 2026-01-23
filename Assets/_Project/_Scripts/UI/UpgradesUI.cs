using System;
using System.Collections.Generic;
using System.Linq;
using Alchemy.Serialization;
using DG.Tweening;
using Game;
using Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    [AlchemySerialize]
    public partial class UpgradesUI : MonoBehaviour {

        #region SerializedFields
        [Header("RectTransforms")]
        [SerializeField] private RectTransform _parentRect;
        [Header("Acceleration")]
        [SerializeField] private TMP_Text _accelerationLvlLabel;
        [SerializeField] private TMP_Text _currentAccelerationLabel;
        [SerializeField] private TMP_Text _accelerationPriceLabel;
        [SerializeField] private Image _accelerationPriceIcon;
        [SerializeField] private List<Button> _upgradeAccelerationBtn;
        [AlchemySerializeField, NonSerialized] private Dictionary<Image, Image> _accelerationPbParts;
        [Header("Power")]
        [SerializeField] private TMP_Text _powerLvlLabel;
        [SerializeField] private TMP_Text _currentPowerLabel;
        [SerializeField] private TMP_Text _powerPriceLabel;
        [SerializeField] private Image _powerPriceIcon;
        [SerializeField] private List<Button> _upgradePowerBtn;
        [AlchemySerializeField, NonSerialized] private Dictionary<Image, Image> _powerPbParts;
        [Header("Fuel")]
        [SerializeField] private TMP_Text _fuelLvlLabel;
        [SerializeField] private TMP_Text _currentFuelLabel;
        [SerializeField] private TMP_Text _fuelPriceLabel;
        [SerializeField] private Image _fuelPriceIcon;
        [SerializeField] private List<Button> _upgradeFuelBtn;
        [AlchemySerializeField, NonSerialized] private Dictionary<Image, Image> _fuelPbParts;
        #endregion

        #region PrivateFields
        private const float ParentRectSlideDuration = .75f;
        private const int EnabledYPosition = 330;
        private const int DisabledYPosition = -330;
        
        private Models.Shredder _shredderData;
        #endregion
        
        #region Initialization
        public void Initialize(Models.Shredder shredder) {
            _shredderData = shredder;

            Main.Instance.StateManager.State.OnUpdate += OnGameStateChange;
            Main.Instance.CurrencyData.Coins.OnUpdate += OnCoinsChange;
            
            _shredderData.AccelerationData.Level.OnUpdate += OnAccelerationLvlChange;
            _shredderData.AccelerationData.InvestedStep.OnUpdate += OnAccelerationInvestedStepChange;
            _shredderData.PowerData.Level.OnUpdate += OnPowerLvlChange;
            _shredderData.PowerData.InvestedStep.OnUpdate += OnPowerInvestedStepChange;
            _shredderData.FuelData.Level.OnUpdate += OnFuelLvlChange;
            _shredderData.FuelData.InvestedStep.OnUpdate += OnFuelInvestedStepChange;

            SetAccelerationLabels();
            SetAccelerationPrice();
            SetAccelerationButtons();
            SetAccelerationProgressBar();
            
            SetPowerLabels();
            SetPowerPrice();
            SetPowerButtons();
            SetPowerProgressBar();
            
            SetFuelLabels();
            SetFuelPrice();
            SetFuelButtons();
            SetFuelProgressBar();
        }

        public void Deinitialize() {
            Main.Instance.StateManager.State.OnUpdate -= OnGameStateChange;
            Main.Instance.CurrencyData.Coins.OnUpdate -= OnCoinsChange;
            
            _shredderData.AccelerationData.Level.OnUpdate -= OnAccelerationLvlChange;
            _shredderData.AccelerationData.InvestedStep.OnUpdate -= OnAccelerationInvestedStepChange;
            _shredderData.PowerData.Level.OnUpdate -= OnPowerLvlChange;
            _shredderData.PowerData.InvestedStep.OnUpdate -= OnPowerInvestedStepChange;
            _shredderData.FuelData.Level.OnUpdate -= OnFuelLvlChange;
            _shredderData.FuelData.InvestedStep.OnUpdate -= OnFuelInvestedStepChange;

            _parentRect.DOKill();
        }
        #endregion

        #region Panel
        private void SlideParentPanel(bool active) => _parentRect.DOAnchorPosY(active ? EnabledYPosition : DisabledYPosition, ParentRectSlideDuration).SetEase(Ease.OutBounce);
        #endregion

        #region Acceleration
        private void SetAccelerationLabels() {
            _accelerationLvlLabel.SetText(_shredderData.AccelerationData.Level.Value().ToString());
            _currentAccelerationLabel.SetText(_shredderData.AccelerationData.Power.ToString("F1"));
        }
        
        private void SetAccelerationPrice() {
            var coinsPriceForUpgrade = Constants.GetPriceForUpgrade(_shredderData.AccelerationData.Level.Value());
            var diamondsPriceForUpgrade = Constants.GetPriceForUpgradeInDiamonds(_shredderData.AccelerationData.Level.Value());
            
            if (Main.Instance.CurrencyData.Coins.Value() < coinsPriceForUpgrade) {
                _accelerationPriceIcon.sprite = Main.Instance.CurrencyIcons[Models.CurrencyType.Diamond];
                _accelerationPriceLabel.SetText(diamondsPriceForUpgrade.ToString());
            }
            else {
                _accelerationPriceIcon.sprite = Main.Instance.CurrencyIcons[Models.CurrencyType.Coin];
                _accelerationPriceLabel.SetText(coinsPriceForUpgrade.ToString());
            }
            
            var hasEnoughMoney = Main.Instance.CurrencyData.Coins.Value() >= coinsPriceForUpgrade || Main.Instance.CurrencyData.Diamonds.Value() >= diamondsPriceForUpgrade;
            _upgradeAccelerationBtn.ForEach(b => b.interactable = hasEnoughMoney);
        }
        
        private void SetAccelerationButtons() {
            _upgradeAccelerationBtn.ForEach(b => b.onClick.AddListener(UpgradeAcceleration));
        }
        
        private void SetAccelerationProgressBar() {
            if (_shredderData.AccelerationData.InvestedStep.Value() <= 0) {
                _accelerationPbParts.Values.ForEach(p => p.gameObject.SetActive(false));
            }
            else {
                for (int i = 0; i < _shredderData.AccelerationData.InvestedStep.Value(); i++)
                    _accelerationPbParts.Values.ElementAt(i).gameObject.SetActive(true);
            }
        }

        private void UpgradeAcceleration() {
            var coinsPriceForUpgrade = Constants.GetPriceForUpgrade(_shredderData.AccelerationData.Level.Value());
            var diamondsPriceForUpgrade = Constants.GetPriceForUpgradeInDiamonds(_shredderData.AccelerationData.Level.Value());
            if (Main.Instance.CurrencyData.Coins.Value() < coinsPriceForUpgrade) {
                if (Main.Instance.CurrencyData.Subtract(Models.CurrencyType.Diamond, diamondsPriceForUpgrade))
                    _shredderData.UpgradeAcceleration();
            }
            else {
                if (Main.Instance.CurrencyData.Subtract(Models.CurrencyType.Coin, coinsPriceForUpgrade))
                    _shredderData.UpgradeAcceleration();
            }
        }
        #endregion
        
        #region Power
        private void SetPowerLabels() {
            _powerLvlLabel.SetText(_shredderData.PowerData.Level.Value().ToString());
            _currentPowerLabel.SetText(((int)_shredderData.PowerData.Power).ToString());
            _powerPriceLabel.SetText(Constants.GetPriceForUpgrade(_shredderData.PowerData.Level.Value()).ToString());
        }
        
        private void SetPowerPrice() {
            var coinsPriceForUpgrade = Constants.GetPriceForUpgrade(_shredderData.PowerData.Level.Value());
            var diamondsPriceForUpgrade = Constants.GetPriceForUpgradeInDiamonds(_shredderData.PowerData.Level.Value());
            
            if (Main.Instance.CurrencyData.Coins.Value() < coinsPriceForUpgrade) {
                _powerPriceIcon.sprite = Main.Instance.CurrencyIcons[Models.CurrencyType.Diamond];
                _powerPriceLabel.SetText(diamondsPriceForUpgrade.ToString());
            }
            else {
                _powerPriceIcon.sprite = Main.Instance.CurrencyIcons[Models.CurrencyType.Coin];
                _powerPriceLabel.SetText(coinsPriceForUpgrade.ToString());
            }
            
            var hasEnoughMoney = Main.Instance.CurrencyData.Coins.Value() >= coinsPriceForUpgrade || Main.Instance.CurrencyData.Diamonds.Value() >= diamondsPriceForUpgrade;
            _upgradePowerBtn.ForEach(b => b.interactable = hasEnoughMoney);
        }
        
        private void SetPowerButtons() {
            _upgradePowerBtn.ForEach(b => b.onClick.AddListener(UpgradePower));
        }

        private void SetPowerProgressBar() {
            if (_shredderData.PowerData.InvestedStep.Value() <= 0) {
                _powerPbParts.Values.ForEach(p => p.gameObject.SetActive(false));
            }
            else {
                for (int i = 0; i < _shredderData.PowerData.InvestedStep.Value(); i++)
                    _powerPbParts.Values.ElementAt(i).gameObject.SetActive(true);
            }
        }

        private void UpgradePower() {
            var coinsPriceForUpgrade = Constants.GetPriceForUpgrade(_shredderData.PowerData.Level.Value());
            var diamondsPriceForUpgrade = Constants.GetPriceForUpgradeInDiamonds(_shredderData.PowerData.Level.Value());
            if (Main.Instance.CurrencyData.Coins.Value() < coinsPriceForUpgrade) {
                if (Main.Instance.CurrencyData.Subtract(Models.CurrencyType.Diamond, diamondsPriceForUpgrade))
                    _shredderData.UpgradePower();
            }
            else {
                if (Main.Instance.CurrencyData.Subtract(Models.CurrencyType.Coin, coinsPriceForUpgrade))
                    _shredderData.UpgradePower();
            }
        }
        #endregion
        
        #region Fuel
        private void SetFuelLabels() {
            _fuelLvlLabel.SetText(_shredderData.FuelData.Level.Value().ToString());
            _currentFuelLabel.SetText(((int)_shredderData.FuelData.Power).ToString());
            _fuelPriceLabel.SetText(Constants.GetPriceForUpgrade(_shredderData.FuelData.Level.Value()).ToString());
        }

        private void SetFuelPrice() {
            var coinsPriceForUpgrade = Constants.GetPriceForUpgrade(_shredderData.FuelData.Level.Value());
            var diamondsPriceForUpgrade = Constants.GetPriceForUpgradeInDiamonds(_shredderData.FuelData.Level.Value());
            
            if (Main.Instance.CurrencyData.Coins.Value() < coinsPriceForUpgrade) {
                _fuelPriceIcon.sprite = Main.Instance.CurrencyIcons[Models.CurrencyType.Diamond];
                _fuelPriceLabel.SetText(diamondsPriceForUpgrade.ToString());
            }
            else {
                _fuelPriceIcon.sprite = Main.Instance.CurrencyIcons[Models.CurrencyType.Coin];
                _fuelPriceLabel.SetText(coinsPriceForUpgrade.ToString());
            }
            
            var hasEnoughMoney = Main.Instance.CurrencyData.Coins.Value() >= coinsPriceForUpgrade || Main.Instance.CurrencyData.Diamonds.Value() >= diamondsPriceForUpgrade;
            _upgradeFuelBtn.ForEach(b => b.interactable = hasEnoughMoney);
        }
        
        
        private void SetFuelButtons() {
            _upgradeFuelBtn.ForEach(b => b.onClick.AddListener(UpgradeFuel));
        }

        private void SetFuelProgressBar() {
            if (_shredderData.FuelData.InvestedStep.Value() <= 0) {
                _fuelPbParts.Values.ForEach(p => p.gameObject.SetActive(false));
            }
            else {
                for (int i = 0; i < _shredderData.FuelData.InvestedStep.Value(); i++)
                    _fuelPbParts.Values.ElementAt(i).gameObject.SetActive(true);
            }
        }
        
        private void UpgradeFuel() {
            var coinsPriceForUpgrade = Constants.GetPriceForUpgrade(_shredderData.FuelData.Level.Value());
            var diamondsPriceForUpgrade = Constants.GetPriceForUpgradeInDiamonds(_shredderData.FuelData.Level.Value());
            if (Main.Instance.CurrencyData.Coins.Value() < coinsPriceForUpgrade) {
                if (Main.Instance.CurrencyData.Subtract(Models.CurrencyType.Diamond, diamondsPriceForUpgrade))
                    _shredderData.UpgradeFuel();
            }
            else {
                if (Main.Instance.CurrencyData.Subtract(Models.CurrencyType.Coin, coinsPriceForUpgrade))
                    _shredderData.UpgradeFuel();
            }
        }
        #endregion

        #region Events
        private void OnGameStateChange(StateManager.GameState _, StateManager.GameState state) {
            if (state is StateManager.GameState.Playing)
                SlideParentPanel(false);
            else if (state is StateManager.GameState.Win or StateManager.GameState.Lose) 
                SlideParentPanel(true);
        }
        
        private void OnCoinsChange(int arg1, int arg2) {
            SetAccelerationPrice();
            SetPowerPrice();
            SetFuelPrice();
        }

        private void OnAccelerationLvlChange(int arg1, int arg2) {
            SetAccelerationLabels();
            SetAccelerationProgressBar();
            SetAccelerationPrice();
        }
        
        private void OnAccelerationInvestedStepChange(int arg1, int arg2) {
            SetAccelerationLabels();
            SetAccelerationProgressBar();
            SetAccelerationPrice();
        }

        private void OnPowerLvlChange(int arg1, int arg2) {
            SetPowerLabels();
            SetPowerProgressBar();
            SetPowerPrice();
        }
        
        private void OnPowerInvestedStepChange(int arg1, int arg2) {
            SetPowerLabels();
            SetPowerProgressBar();
            SetPowerPrice();
        }
        
        private void OnFuelLvlChange(int arg1, int arg2) {
            SetFuelLabels();
            SetFuelProgressBar();
            SetFuelPrice();
        }
        
        private void OnFuelInvestedStepChange(int arg1, int arg2) {
            SetFuelLabels();
            SetFuelProgressBar();
            SetFuelPrice();
        }
        #endregion
    }
}