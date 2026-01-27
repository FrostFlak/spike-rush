using System;
using DG.Tweening;
using Game;
using Models;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

namespace UI {
    public class WorldSpacePayout : MonoBehaviour {

        [Header("UI")]
        [SerializeField] private TMP_Text _payoutLabel;
        [SerializeField] private Image _payoutIcon;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private AimConstraint _aimConstraint;
        
        private Sequence _sequence;

        private void Awake() {
            _aimConstraint.SetSource(0, new ConstraintSource{ sourceTransform = Camera.main.transform, weight = 1});
        }

        private void OnDisable() {
            _sequence?.Kill();
        }

        public void SetPayout(int amount, CurrencyType currencyType, Action onCompleteAnimation) {
            _payoutLabel.SetText($"+{amount}");
            _payoutIcon.sprite = Main.Instance.CurrencyIcons[currencyType];
            
            Vector3 jumpTarget = transform.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(.35f, .75f), 0);
            _sequence = DOTween.Sequence()
                .Append(transform.DOJump(jumpTarget, 1.35f, 1, 0.8f).SetEase(Ease.OutQuad))
                .Join(transform.DOScale(1.05f, 0.2f).SetEase(Ease.OutBack))
                .Join(_canvasGroup.DOFade(1f, 0.2f))
                .Insert(0.5f, _canvasGroup.DOFade(0f, 0.3f))
                .Insert(0.5f, transform.DOScale(0.5f, 0.3f))
                .OnComplete(() => onCompleteAnimation?.Invoke());
        }
    }
}