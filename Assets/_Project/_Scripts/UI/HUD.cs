using DG.Tweening;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class HUD : MonoBehaviour {

        #region SerializedFields
        [Header("RectTransforms")]
        [SerializeField] private RectTransform _hudParent;
        [SerializeField] private EndUI _endUI;
        [Header("UI")]
        [Header("Shredder")]
        [SerializeField] private TMP_Text _speedLabel;
        [SerializeField] private Image _fuelBarFill;
        [SerializeField] private Image _heatBarFill;
        [Header("Properties")]
        [SerializeField] private Gradient _heatingGradient;
        [SerializeField] private Gradient _coolingGradient;
        [SerializeField] private Gradient _fuelGradient;
        #endregion

        #region PrivateFields
        private const float GamePanelSlideDuration = .75f;
        private const float EndPanelPopDuration = .75f;
        
        #endregion

        #region Initialization
        public void Initialize() {
            Main.Instance.IsStarted.OnUpdate += OnIsStartedChange;
        }
        
        public void Deinitialize() {
            _hudParent.DOKill();
            
            Main.Instance.IsStarted.OnUpdate -= OnIsStartedChange;
        }
        #endregion

        #region UI
        private void SetGamePanelState() {
            _hudParent.DOAnchorPosY(-_hudParent.anchoredPosition.y, GamePanelSlideDuration).SetEase(Ease.OutBounce);
        }
        
        public void UpdateFuelUI(float currentFuel, float maxFuel) {
            var fuelDelta = currentFuel / maxFuel;
            _fuelBarFill.fillAmount = fuelDelta;
            _fuelBarFill.color = _fuelGradient.Evaluate(fuelDelta);
        }
        
        public void UpdateHeatUI(float currentHeatLevel, float maxHeat, bool isOverheated) {
            var heatDelta = currentHeatLevel / maxHeat;
            _heatBarFill.fillAmount = heatDelta;
            _heatBarFill.color = isOverheated ? _coolingGradient.Evaluate(heatDelta) : _heatingGradient.Evaluate(heatDelta);
        }

        public void UpdateSpeedUI(int speed) {
            _speedLabel.SetText($"{speed} km/h");
        }
        #endregion

        #region Events
        private void OnIsStartedChange(bool arg1, bool started) => Invoke(nameof(SetGamePanelState), started ? .35f : 0);
        #endregion
    }
}