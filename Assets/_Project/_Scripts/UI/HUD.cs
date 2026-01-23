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
        [Header("Properties")]
        [SerializeField] private Gradient _heatingGradient;
        [SerializeField] private Gradient _coolingGradient;
        [SerializeField] private Gradient _fuelGradient;
        #endregion

        #region PrivateFields
        private const float GamePanelSlideDuration = .75f;
        
        #endregion

        #region Initialization
        public void Initialize() {
            Main.Instance.StateManager.State.OnUpdate += OnGameStateChange;
        }

        public void Deinitialize() {
            _hudParent.DOKill();
            
            Main.Instance.StateManager.State.OnUpdate -= OnGameStateChange;
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
            
            _fuelLabel.SetText($"{currentFuel:N0}/{maxFuel:N0}");
        }
        
        public void UpdateHeatUI(float currentHeatLevel, float maxHeat, bool isOverheated) {
            var heatDelta = currentHeatLevel / maxHeat;
            _heatBarFill.fillAmount = heatDelta;
            _heatBarFill.color = isOverheated ? _coolingGradient.Evaluate(heatDelta) : _heatingGradient.Evaluate(heatDelta);
            
            _heatLabel.SetText($"{currentHeatLevel:N0}/{maxHeat:N0}");
        }

        public void UpdateSpeedUI(int speed) => _speedLabel.SetText($"{speed} km/h");

        public void UpdateDistanceLabel(int distance) => _lvlDistanceLabel.SetText($"{distance}m");

        public void UpdateLvlProgressBar(Transform shredder) {
            var start = Main.Instance.LevelManager.ActiveLevel.StartTransform;
            var finish = Main.Instance.LevelManager.ActiveLevel.EndTransform;
            var totalDistance = Vector3.Distance(start.position, finish.position);
            var traveledDistance = Vector3.Distance(shredder.position, start.position);
            
            _lvlProgressBar.value = traveledDistance / totalDistance;
        }
        #endregion

        #region Events
        private void OnGameStateChange(StateManager.GameState arg1, StateManager.GameState state) {
            if (state is StateManager.GameState.Playing)
                Invoke(nameof(SetGamePanelState), .35f);
            else if (state is StateManager.GameState.Win or StateManager.GameState.Lose)
                SetGamePanelState();                
        }
        #endregion
    }
}