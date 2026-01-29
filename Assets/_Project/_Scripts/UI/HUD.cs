using DG.Tweening;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class HUD : MonoBehaviour {

        #region SerializedFields
        [Header("RectTransforms")]
        [SerializeField] private RectTransform _gameRect;
        [SerializeField] private RectTransform _levelRect;
        [Header("UI")]
        [Header("Shredder")]
        [SerializeField] private TMP_Text _speedLabel;
        [SerializeField] private TMP_Text _fuelLabel;
        [SerializeField] private Image _fuelBarFill;
        [SerializeField] private TMP_Text _lowFuelWarnLabel;
        [Header("LevelProgress")]
        [SerializeField] private Slider _lvlProgressBar;
        [SerializeField] private TMP_Text _lvlLabel;
        [SerializeField] private TMP_Text _lvlDistanceLabel;
        [SerializeField] private Image _recordMarker;
        [Header("Properties")]
        [SerializeField] private Gradient _fuelGradient;
        #endregion

        #region PrivateFields
        private const float PanelSlideDuration = .75f;
        private const int YHUDPosition = 350;
        private const int YLvlPosition = -200;
        
        private Tween _lowFuelTween;
        #endregion

        #region Initialization
        public void Initialize() {
            Main.Instance.StateManager.State.OnUpdate += OnGameStateChange;
            UpdateLvlProgressBar();
            
            SetLevelLabel();
            _lowFuelWarnLabel.alpha = 0f;
        }

        public void Deinitialize() {
            _gameRect.DOKill();
            _lowFuelTween?.Kill();
            
            Main.Instance.StateManager.State.OnUpdate -= OnGameStateChange;
        }
        #endregion

        #region UI
        private void SetHUDRectState(bool active) {
            _gameRect.DOAnchorPosY(active ? YHUDPosition : -YHUDPosition, PanelSlideDuration).SetEase(Ease.OutBounce);
        }
        
        private void SetLvlPanelState(bool active) {
            _levelRect.DOAnchorPosY(active ? YLvlPosition : -YLvlPosition, PanelSlideDuration).SetEase(Ease.OutBounce);
        }

        public void UpdateFuelUI(float currentFuel, float maxFuel) {
            var fuelDelta = currentFuel / maxFuel;
            _fuelBarFill.fillAmount = fuelDelta;
            _fuelBarFill.color = _fuelGradient.Evaluate(fuelDelta);
            
            _fuelLabel.SetText($"{currentFuel:N0}/{maxFuel:N0}");
            if (fuelDelta < 0.25f && fuelDelta > 0) {
                bool isActive = _lowFuelTween != null && _lowFuelTween.IsActive() && _lowFuelTween.IsPlaying();

                if (isActive) 
                    return;
                
                _lowFuelWarnLabel.alpha = 0; 
                _lowFuelTween = _lowFuelWarnLabel.DOFade(1f, 0.5f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);
            }
            else {
                if (_lowFuelTween != null && _lowFuelTween.IsActive()) 
                    _lowFuelTween.Kill();
                
                _lowFuelTween = null;
                _lowFuelWarnLabel.alpha = 0; 
            }
        }
        
        public void UpdateSpeedUI(int speed) => _speedLabel.SetText($"{speed} km/h");

        public void UpdateDistanceLabel() {
            _lvlDistanceLabel.SetText($"{Main.Instance.CurrentTraversedDistance}m");

            TrySetNewRecordDistance();
        }

        private void TrySetNewRecordDistance() {
            var currentLvlData = Main.Instance.LevelsData[Main.Instance.CurrentLevelID - 1];
            if (currentLvlData == null)
                return;

            if (currentLvlData.RecordDistance.Value() >= Main.Instance.CurrentTraversedDistance)
                return;

            currentLvlData.RecordDistance.Set(Main.Instance.CurrentTraversedDistance);
        }

        private void SetLevelLabel() => _lvlLabel.SetText($"Level {Main.Instance.CurrentLevelID}");

        public void UpdateLvlProgressBar() {
            var start = Main.Instance.LevelManager.ActiveLevel.StartTransform;
            var finish = Main.Instance.LevelManager.ActiveLevel.EndTransform;
            var totalDistance = Vector3.Distance(start.position, finish.position);
            
            _lvlProgressBar.value = Main.Instance.CurrentTraversedDistance / totalDistance;
            
            var currentLvlData = Main.Instance.LevelsData[Main.Instance.CurrentLevelID - 1];
            if (currentLvlData == null)
                return;

            float recordProgress = currentLvlData.RecordDistance.Value() / totalDistance;
            _recordMarker.rectTransform.anchorMin = new Vector2(recordProgress, 0.5f);
            _recordMarker.rectTransform.anchorMax = new Vector2(recordProgress, 0.5f);
        }
        #endregion

        #region Events
        private void OnGameStateChange(StateManager.GameState arg1, StateManager.GameState state) {
            if (state is StateManager.GameState.Playing) {
                SetHUDRectState(true);
                SetLvlPanelState(true);
            }
            else if (state is StateManager.GameState.GatheredReward) {
                SetHUDRectState(false);
                SetLvlPanelState(false);
                SetLevelLabel();
            }
        }
        #endregion
    }
}