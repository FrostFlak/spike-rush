using DG.Tweening;
using Game;
using TMPro;
using Unity.Logging;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class HUD : MonoBehaviour {

        #region SerializedFields
        [Header("RectTransforms")]
        [SerializeField] private RectTransform _hudParent;
        [Header("UI")]
        [Header("Shredder")]
        [SerializeField] private TMP_Text _speedLabel;
        [SerializeField] private TMP_Text _fuelLabel;
        [SerializeField] private TMP_Text _heatLabel;
        [SerializeField] private Image _fuelBarFill;
        [SerializeField] private Image _heatBarFill;
        [Header("LevelProgress")]
        [SerializeField] private Slider _lvlProgressBar;
        [SerializeField] private TMP_Text _lvlDistanceLabel;
        [SerializeField] private Image _recordMarker;
        [Header("Properties")]
        [SerializeField] private Gradient _heatingGradient;
        [SerializeField] private Gradient _coolingGradient;
        [SerializeField] private Gradient _fuelGradient;
        #endregion

        #region PrivateFields
        private const float GamePanelSlideDuration = .75f;
        private const int DisabledYPosition = -350;
        private const int EnabledYPosition = 350;
        
        #endregion

        #region Initialization
        public void Initialize() {
            Main.Instance.StateManager.State.OnUpdate += OnGameStateChange;
            UpdateLvlProgressBar();
        }

        public void Deinitialize() {
            _hudParent.DOKill();
            
            Main.Instance.StateManager.State.OnUpdate -= OnGameStateChange;
        }
        #endregion

        #region UI
        private void SetGamePanelState(bool active) {
            _hudParent.DOAnchorPosY(active ? EnabledYPosition : DisabledYPosition, GamePanelSlideDuration).SetEase(Ease.OutBounce);
        }
        
        public void UpdateFuelUI(float currentFuel, float maxFuel) {
            var fuelDelta = currentFuel / maxFuel;
            _fuelBarFill.fillAmount = fuelDelta;
            _fuelBarFill.color = _fuelGradient.Evaluate(fuelDelta);
            
            _fuelLabel.SetText($"{currentFuel:N0}/{maxFuel:N0}");
        }
        
        public void UpdateHeatUI(float currentHeatLevel, float maxHeat, bool isOverheated) {
            var heatDelta = currentHeatLevel / maxHeat;
            _heatBarFill.fillAmount = heatDelta;
            _heatBarFill.color = isOverheated ? _coolingGradient.Evaluate(heatDelta) : _heatingGradient.Evaluate(heatDelta);
            
            _heatLabel.SetText($"{currentHeatLevel:N0}/{maxHeat:N0}");
        }

        public void UpdateSpeedUI(int speed) => _speedLabel.SetText($"{speed} km/h");

        public void UpdateDistanceLabel() {
            _lvlDistanceLabel.SetText($"{Main.Instance.CurrentDistance}m");

            TrySetNewRecordDistance();
        }

        private void TrySetNewRecordDistance() {
            var currentLvlData = Main.Instance.LevelsData[Main.Instance.CurrentLevelID - 1];
            if (currentLvlData == null)
                return;

            if (currentLvlData.RecordDistance.Value() >= Main.Instance.CurrentDistance)
                return;

            currentLvlData.RecordDistance.Set(Main.Instance.CurrentDistance);
        }

        public void UpdateLvlProgressBar() {
            var start = Main.Instance.LevelManager.ActiveLevel.StartTransform;
            var finish = Main.Instance.LevelManager.ActiveLevel.EndTransform;
            var totalDistance = Vector3.Distance(start.position, finish.position);
            
            _lvlProgressBar.value = Main.Instance.CurrentDistance / totalDistance;
            
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
            if (state is StateManager.GameState.Playing)
                SetGamePanelState(true);
            else if (state is StateManager.GameState.Win or StateManager.GameState.Lose)
                SetGamePanelState(false);
        }
        #endregion
    }
}