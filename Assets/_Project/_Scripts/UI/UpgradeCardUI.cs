using System;
using System.Collections.Generic;
using System.Linq;
using Alchemy.Serialization;
using DG.Tweening;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    [AlchemySerialize]
    public partial class UpgradeCardUI : MonoBehaviour {
        
        [Header("Labels")]
        [SerializeField] private TMP_Text _lvlLabel;
        [SerializeField] private TMP_Text _priceLabel;
        [Header("Buttons")]
        [SerializeField] private List<Button> _upgradeBtn;
        [Header("Images")]
        [SerializeField] private Image _priceIcon;
        [AlchemySerializeField, NonSerialized] private Dictionary<int, Image> _progressbarParts;
        
        private Models.UpgradeData _upgradeData;
        private float _powerStep;
        private Tweener _stepUpgradeTween;

        #region Initialization
        public void Initialize(Models.UpgradeData upgradeData, float powerStep) {
            _upgradeData = upgradeData;
            _powerStep = powerStep;
            
            Main.Instance.CurrencyData.Coins.OnUpdate += OnCoinsChange;
            
            _upgradeData.Level.OnUpdate += OnLvlChange;
            _upgradeData.InvestedStep.OnUpdate += OnInvestedStepChange;
            
            SetLabels();
            SetPrice();
            SetButtons();
            SetProgressBar();
        }

        public void Deinitialize() {
            _stepUpgradeTween?.Kill();
            
            Main.Instance.CurrencyData.Coins.OnUpdate -= OnCoinsChange;
            
            _upgradeData.Level.OnUpdate -= OnLvlChange;
            _upgradeData.InvestedStep.OnUpdate -= OnInvestedStepChange;
        }
        #endregion
        
        #region UI
        private void SetLabels() {
            _lvlLabel.SetText(_upgradeData.Level.Value().ToString());
        }
        
        private void SetPrice() {
            var coinsPriceForUpgrade = Constants.GetPriceForUpgrade(_upgradeData.Level.Value());
            var diamondsPriceForUpgrade = Constants.GetPriceForUpgradeInDiamonds(_upgradeData.Level.Value());
            
            if (Main.Instance.CurrencyData.Coins.Value() < coinsPriceForUpgrade) {
                _priceIcon.sprite = Main.Instance.CurrencyIcons[Models.CurrencyType.Diamond];
                _priceLabel.SetText(diamondsPriceForUpgrade.ToString());
            }
            else {
                _priceIcon.sprite = Main.Instance.CurrencyIcons[Models.CurrencyType.Coin];
                _priceLabel.SetText(coinsPriceForUpgrade.ToString());
            }
            
            var hasEnoughMoney = Main.Instance.CurrencyData.Coins.Value() >= coinsPriceForUpgrade || Main.Instance.CurrencyData.Diamonds.Value() >= diamondsPriceForUpgrade;
            _upgradeBtn.ForEach(b => b.interactable = hasEnoughMoney);
        }
        
        private void SetButtons() {
            _upgradeBtn.ForEach(b => b.onClick.AddListener(Upgrade));
        }
        
        private void SetProgressBar() {
            var list = _progressbarParts.OrderBy(k => k.Key).ToList();

            for (int i = 0; i < list.Count; i++) 
                list[i].Value.gameObject.SetActive(i < _upgradeData.InvestedStep.Value());
        }

        private void Upgrade() {
            var coinsPriceForUpgrade = Constants.GetPriceForUpgrade(_upgradeData.Level.Value());
            var diamondsPriceForUpgrade = Constants.GetPriceForUpgradeInDiamonds(_upgradeData.Level.Value());
            if (Main.Instance.CurrencyData.Coins.Value() < coinsPriceForUpgrade) {
                if (!Main.Instance.CurrencyData.Subtract(Models.CurrencyType.Diamond, diamondsPriceForUpgrade)) 
                    return;
                
                _upgradeData.Upgrade(_powerStep);
                AnimationStepUpgrade();
            }
            else {
                if (!Main.Instance.CurrencyData.Subtract(Models.CurrencyType.Coin, coinsPriceForUpgrade)) 
                    return;
                
                _upgradeData.Upgrade(_powerStep);
                AnimationStepUpgrade();
            }
        }

        private void AnimationStepUpgrade() {
            _stepUpgradeTween?.Kill(true);
            _stepUpgradeTween = transform
                .DOPunchScale(new Vector3(0.25f, 0.35f, 0.25f), 0.3f, 3, 0.15f)
                .SetEase(Ease.OutBack)
                .OnComplete(() => {
                    transform.localScale = Vector3.one;
                });
        }
        #endregion

        #region Events
        private void OnCoinsChange(int arg1, int arg2) {
            SetPrice();
        }

        private void OnLvlChange(int arg1, int arg2) {
            SetLabels();
            SetProgressBar();
            SetPrice();
        }
        
        private void OnInvestedStepChange(int arg1, int arg2) {
            SetLabels();
            SetProgressBar();
            SetPrice();
        }
        #endregion
    }
}