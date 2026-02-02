using System;
using System.Collections.Generic;
using System.Linq;
using Alchemy.Serialization;
using DG.Tweening;
using Helpers;
using UI;
using UnityEngine;

namespace Game {
    [AlchemySerialize]
    public partial class Shredder : MonoBehaviour {

        #region SerializedFields
        [Header("Components")]
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Joystick _joystick;
        [SerializeField] private HUD _hud;
        [AlchemySerializeField, NonSerialized] private Dictionary<int, GameObject> _spikeVisuals;
        [SerializeField] private AudioSource _source;
        [Header("Properties")]
        [Header("Movement(m/s)")]
        [SerializeField] private float _breakingForce;
        [SerializeField] private float _stoppingForce;
        [Header("Fuel")]
        [SerializeField] private float _fuelConsumptionRate;
        [Header("Prefabs")]
        [SerializeField] private WorldSpacePayout _worldSpacePayoutPb;
        #endregion

        #region PrivateFields
        private const float SideMovementMultiplier = 3.5f;
        private const float BoostDuration = 2f;
        
        private Models.Shredder _shredderData;
        private ObjectPool<WorldSpacePayout> _worldSpacePayoutPool;
        
        private Vector3 _input;
        private float _currentFuel;
        private bool _isActive;
        private float _bonusSpeed;
        #endregion

        #region Initialization
        public void Initialize(Models.Shredder shredderData) {
            _shredderData = shredderData;
            _worldSpacePayoutPool = new ObjectPool<WorldSpacePayout>(_worldSpacePayoutPb, 5);
            _hud.Initialize();
            
            Main.Instance.StateManager.State.OnUpdate += OnGameStateChange;
            _shredderData.PowerData.Level.OnUpdate += OnPowerLevelChange;
            SetSpikes(_shredderData.PowerData.Level.Value());
        }

        public void Deinitialize() {
            _hud.Deinitialize();
            
            if (_shredderData == null)
                return;
                
            Main.Instance.StateManager.State.OnUpdate -= OnGameStateChange;
            _shredderData.PowerData.Level.OnUpdate -= OnPowerLevelChange;
        }
        #endregion

        #region State
        private void SetShredderState(bool active) {
            _currentFuel = _shredderData.FuelData.Power;
            _isActive = active;
            _rigidbody.isKinematic = !active;
        }
        
        public void ReturnToStart(Vector3 position) {
            _rigidbody.rotation = Quaternion.identity;
            _rigidbody.position = position;
        }
        
        private void ResetAttributes() {
            _shredderData.AccelerationData.ResetUpgrade();
            _shredderData.PowerData.ResetUpgrade();
            _shredderData.FuelData.ResetUpgrade();
        }
        #endregion

        #region Behaviour
        private void Update() {
            if (_isActive) {
                _input = GatherInput();
                
                _hud.UpdateFuelUI(_currentFuel, _shredderData.FuelData.Power);
                _hud.UpdateSpeedUI(Mathf.RoundToInt(_rigidbody.linearVelocity.magnitude * 3.6f));
            }

            if (Main.Instance.LevelManager.ActiveLevel == null)
                return;
            
            Main.Instance.CurrentTraversedDistance = Mathf.RoundToInt(Vector3.Distance(transform.position, Main.Instance.LevelManager.ActiveLevel.StartTransform.position));
            _hud.UpdateDistanceLabel();
            _hud.UpdateLvlProgressBar();
        }

        private void FixedUpdate() {
            if (!_isActive) {
                _source.volume = 0f;
                
                return;
            }
            
            int maxSpeed = Constants.GetMaxSpeedByAccelerationLvl(_shredderData.AccelerationData.Level.Value());
            var effectiveMaxSpeed = maxSpeed + _bonusSpeed;
            ApplyMovement(effectiveMaxSpeed);
            HandleFuel();

            var speed = _rigidbody.linearVelocity.magnitude;
            _source.volume = Mathf.Clamp(speed / effectiveMaxSpeed, 0f, .4f);
            _source.pitch = Mathf.Lerp(0.9f, 1.12f, speed / effectiveMaxSpeed);
        }
        #endregion

        #region Fuel
        private void HandleFuel() {
            if (_currentFuel <= 0 && _rigidbody.linearVelocity.magnitude <= 0.25f) 
                Lose();
        }
        
        private void Lose() {
            if (!_isActive) 
                return;
            
            _rigidbody.linearVelocity = Vector3.zero;
            _isActive = false;
            Main.Instance.StateManager.FailLvl();
        }
        #endregion

        #region Movement
        private Vector3 GatherInput() {
            Vector3 joystickInput = new Vector3(_joystick.Horizontal, 0f, _joystick.Vertical);
            Vector3 keyboardInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            
            return joystickInput.sqrMagnitude > 0.01f ? joystickInput.normalized : keyboardInput.normalized;
        }
        
        public void ApplyBoost(float power, Vector3 direction) {
            _bonusSpeed = power / 3.6f;
            _rigidbody.AddForce(direction * power, ForceMode.Impulse);
            DOTween.To(() => _bonusSpeed, x => _bonusSpeed = x, 0f, BoostDuration).SetEase(Ease.Linear);
        }
        
        private void ApplyMovement(float effectiveMaxSpeed) {
            float moveVertical = _input.z;
            float moveHorizontal = _input.x;
            Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical).normalized;
    
            float currentZVelocity = _rigidbody.linearVelocity.z;

            var hasFuel = _currentFuel > 0;
            if (Mathf.Abs(moveHorizontal) > 0.01f) {
                float handlingMultiplier = hasFuel ? 1f : 0.5f;
                float horizontalForce = moveHorizontal * _shredderData.AccelerationData.Power * SideMovementMultiplier * handlingMultiplier;
                _rigidbody.AddForce(Vector3.right * horizontalForce, ForceMode.Acceleration);
            }

            if (hasFuel && movement.sqrMagnitude > 0.01f) {
                float finalMoveForce = _shredderData.AccelerationData.Power;
                bool isReversing = (moveVertical > 0 && currentZVelocity < -0.1f) || 
                                   (moveVertical < 0 && currentZVelocity > 0.1f);

                if (isReversing)
                    finalMoveForce *= _breakingForce;

                _rigidbody.AddForce(Vector3.forward * moveVertical * finalMoveForce, ForceMode.Acceleration);

                _currentFuel -= _fuelConsumptionRate * Time.fixedDeltaTime;
            } 
            else {
                if (_rigidbody.linearVelocity.magnitude > 0.1f)
                    _rigidbody.AddForce(-_rigidbody.linearVelocity.normalized * _stoppingForce, ForceMode.Acceleration);
            }
            
            _rigidbody.linearVelocity = Vector3.ClampMagnitude(_rigidbody.linearVelocity, effectiveMaxSpeed);
        }
        #endregion

        #region Collisions
        private void OnTriggerEnter(Collider other) {
            if (!other.TryGetComponent(out Destructible destructibile)) 
                return;
            
            ApplyImpact(destructibile.SpeedLoss / 100f);
            
            destructibile.SpawnFragments();
            var payoutUI = _worldSpacePayoutPool.Get(destructibile.transform.position, Quaternion.identity);
            var totalUpgradesLvl = _shredderData.FuelData.Level.Value() + _shredderData.PowerData.Level.Value() + _shredderData.AccelerationData.Level.Value();
            
            var payout = Constants.GetCoinsByObject(destructibile.CoinsPayout, totalUpgradesLvl);
            payoutUI.SetPayout(payout, Models.CurrencyType.Coin, () => _worldSpacePayoutPool.Return(payoutUI));

            Main.Instance.RunReceivedCoins += payout;
        }

        private void ApplyImpact(float speedLoss) {
            float mitigation = 1f / _shredderData.PowerData.Power;
            float finalLoss = speedLoss * mitigation;
            finalLoss = Mathf.Max(finalLoss, 0.05f);

            _rigidbody.linearVelocity *= 1f - finalLoss;
        }
        #endregion

        #region Spikes
        private void SetSpikes(int lvl) {
            if (lvl == 1) {
                foreach (var spike in _spikeVisuals.Where(k => k.Key != 1)) 
                    spike.Value.SetActive(false);
                
                return;
            }
            
            for (int i = 1; i <= lvl; i++) {
                if (!_spikeVisuals.TryGetValue(i, out var value))
                    continue;
                
                if (value.activeInHierarchy)
                    continue;
                
                value.SetActive(true);
                value.transform.DOScale(Vector3.one, 1f).From(Vector3.zero).SetEase(Ease.InOutBounce);
            } 
        }
        #endregion
        
        #region Events
        private void OnGameStateChange(StateManager.GameState arg1, StateManager.GameState state) {
            switch (state) {
                case StateManager.GameState.Lose or StateManager.GameState.Win:
                    var totalUpgradesLvl = _shredderData.FuelData.Level.Value() + _shredderData.PowerData.Level.Value() + _shredderData.AccelerationData.Level.Value();
                    var levelDistance = Mathf.RoundToInt(Vector3.Distance(Main.Instance.LevelManager.ActiveLevel.StartTransform.position, Main.Instance.LevelManager.ActiveLevel.EndTransform.position)); 
                    SetShredderState(false);
                    
                    Main.Instance.RunReceivedCoins += Constants.GetCoinsByDistance(Main.Instance.CurrentTraversedDistance, levelDistance, totalUpgradesLvl);
                    if (state is StateManager.GameState.Win) {
                        Main.Instance.RunReceivedDiamonds += 10;
                        ResetAttributes();
                    }
                    break;
                
                case StateManager.GameState.Playing:
                    SetShredderState(true);
                    break;
            }
        }

        private void OnPowerLevelChange(int arg1, int lvl) => SetSpikes(lvl);
        #endregion
    }
}