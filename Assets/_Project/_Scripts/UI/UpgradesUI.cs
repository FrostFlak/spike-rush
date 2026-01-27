using DG.Tweening;
using Game;
using UnityEngine;

namespace UI {
    public class UpgradesUI : MonoBehaviour {

        #region SerializedFields
        [Header("RectTransforms")]
        [SerializeField] private RectTransform _parentRect;
        [Header("Acceleration")]
        [SerializeField] private UpgradeCardUI _accelerationCardUI;
        [Header("Power")]
        [SerializeField] private UpgradeCardUI _powerCardUI;
        [Header("Fuel")]
        [SerializeField] private UpgradeCardUI _fuelCardUI;
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
            _accelerationCardUI.Initialize(_shredderData.AccelerationData, Constants.AccelerationUpgradeStep);
            _powerCardUI.Initialize(_shredderData.PowerData, Constants.PowerUpgradeStep);
            _fuelCardUI.Initialize(_shredderData.FuelData, Constants.FuelUpgradeStep);
        }

        public void Deinitialize() {
            Main.Instance.StateManager.State.OnUpdate -= OnGameStateChange;
            
            _accelerationCardUI.Deinitialize();
            _powerCardUI.Deinitialize();
            _fuelCardUI.Deinitialize();

            _parentRect.DOKill();
        }
        #endregion

        #region Panel
        private void SlideParentPanel(bool active) => _parentRect.DOAnchorPosY(active ? EnabledYPosition : DisabledYPosition, ParentRectSlideDuration).SetEase(Ease.OutBounce);
        #endregion

        #region Events
        private void OnGameStateChange(StateManager.GameState _, StateManager.GameState state) {
            if (state is StateManager.GameState.Playing)
                SlideParentPanel(false);
            else if (state is StateManager.GameState.GatheredReward) 
                SlideParentPanel(true);
        }
        #endregion
    }
}