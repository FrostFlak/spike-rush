using System;
using DG.Tweening;
using Game;
using Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class WorldSpacePayout : MonoBehaviour {

        [SerializeField] private TMP_Text _payoutLabel;
        [SerializeField] private Image _payoutIcon;
        [SerializeField] private CanvasGroup _canvasGroup;
        
        private Sequence _sequence;

        private void OnDisable() {
            _sequence?.Kill();
        }

        public void SetPayout(int amount, CurrencyType currencyType, Action onCompleteAnimation) {
            _payoutLabel.SetText($"+{amount}");
            _payoutIcon.sprite = Main.Instance.CurrencyIcons[currencyType];
            
            Vector3 jumpTarget = transform.position + new Vector3(UnityEngine.Random.Range(-1.5f, 1.5f), UnityEngine.Random.Range(.5f, 1.5f), 0);
            
            // REFACTOR PULA
            _sequence = DOTween.Sequence()
                .Append(transform.DOJump(jumpTarget, 2f, 1, 0.8f).SetEase(Ease.OutQuad))
                .Join(transform.DOScale(1.2f, 0.2f).SetEase(Ease.OutBack))
                .Join(_canvasGroup.DOFade(1f, 0.2f))
                .Join(transform.DOPunchRotation(new Vector3(0, 0, 10f), 0.5f))
                .Insert(0.5f, _canvasGroup.DOFade(0f, 0.3f))
                .Insert(0.5f, transform.DOScale(0.5f, 0.3f))
                .OnComplete(() => onCompleteAnimation?.Invoke());
        }
    }
}