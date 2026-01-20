using Helpers;
using UI;
using UnityEngine;

namespace Game {
    public class Shredder : MonoBehaviour {

        #region SerializedFields
        [Header("Components")]
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private HUD _hud;
        [Header("Properties")]
        [Header("Movement(m/s)")]
        [SerializeField] private float _maxSpeed;
        [SerializeField] private float _stoppingAccelerationForce;
        [Header("Fuel")]
        [SerializeField] private float _fuelConsumptionRate;
        [Header("Heat")]
        [SerializeField] private float _heatRate = 20f;
        [SerializeField] private float _coolRate = 15f;
        #endregion

        #region PrivateFields
        private const int MaxHeat = 100;
        
        private Models.Shredder _shredderData;
        
        private float _currentFuel;
        private float _currentHeatLevel;
        private bool _isOverheated;
        private bool _isActive;
        private bool CanMove => _currentFuel > 0 && !_isOverheated;
        #endregion

        #region Initialization
        public void Initialize(Models.Shredder shredderData) {
            _shredderData = shredderData;

            _hud.Initialize();
            Main.Instance.IsStarted.OnUpdate += OnIsStartedChange;
        }

        public void Deinitialize() {
            _hud.Deinitialize();
            Main.Instance.IsStarted.OnUpdate -= OnIsStartedChange;
        }
        #endregion

        #region State
        private void SetShredderState(bool active) {
            if (active)
                ResetState();
            
            _isActive = active;
        }

        private void ResetState() {
            _currentFuel = _shredderData.FuelData.Power;
            _currentHeatLevel = 0f;
            _isOverheated = false;
        }
        #endregion

        #region Behaviour
        private void Update() {
            if (!_isActive)
                return;
            
            HandleFuelAndHeat();
            _hud.UpdateFuelUI(_currentFuel, _shredderData.FuelData.Power);
            _hud.UpdateHeatUI(_currentHeatLevel, MaxHeat, _isOverheated);
            _hud.UpdateSpeedUI(Mathf.RoundToInt(_rigidbody.linearVelocity.magnitude * 3.6f));

            if (_currentFuel <= 0 && _rigidbody.linearVelocity.magnitude <= 0.1f)
                EndRun();
        }

        private void EndRun() {
            if (!_isActive)
                return;

            Main.Instance.IsStarted.Set(false);
        }

        private void FixedUpdate() {
            if (!_isActive)
                return;
            
            ApplyMovement();
        }
        #endregion

        #region Fuel/Heat
        private void HandleFuelAndHeat() {
            if (Input.GetMouseButton(0) && CanMove) {
                _currentFuel -= _fuelConsumptionRate * Time.deltaTime;
                _currentHeatLevel += _heatRate * Time.deltaTime;
                
                if (_currentHeatLevel >= 100f) 
                    OverheatLock();
            }
            else {
                _currentHeatLevel = Mathf.MoveTowards(_currentHeatLevel, 0f, _coolRate * Time.deltaTime);
                if (_isOverheated && _currentHeatLevel <= 0) 
                    _isOverheated = false;
            }
        }
        
        private void OverheatLock() {
            _isOverheated = true;
            _currentHeatLevel = 100f;
            _rigidbody.linearVelocity *= 0.75f;
        }
        #endregion

        #region Movement
        private void ApplyMovement() {
            if (Input.GetMouseButton(0) && CanMove) {
                if (_rigidbody.linearVelocity.z < _maxSpeed) 
                    _rigidbody.AddForce(Vector3.forward * _shredderData.AccelerationData.Power, ForceMode.Acceleration);
            }

            if (_currentFuel <= 0) {
                _rigidbody.AddForce(-transform.forward * _stoppingAccelerationForce, ForceMode.Acceleration); // ????
            }
        }
        #endregion

        #region Collisions
        private void OnTriggerEnter(Collider other) {
            if (!other.TryGetComponent(out Destructibile destructibile)) 
                return;
            
            ApplyImpact(destructibile.SpeedLoss / 100f, destructibile.HeatPenalty);
            destructibile.SpawnFragments();

            var totalUpgradesLvl = _shredderData.FuelData.Level.Value() + _shredderData.PowerData.Level.Value() + _shredderData.AccelerationData.Level.Value();
            Main.Instance.RunReceivedMoney.Set(Main.Instance.RunReceivedMoney.Value() + Constants.GetMoneyByObject(destructibile.MoneyPayout, totalUpgradesLvl));
        }

        private void ApplyImpact(float loss, float heat) {
            Vector3 velocity = _rigidbody.linearVelocity;
            velocity.z = Mathf.Max(0, velocity.z * Mathf.Clamp01(1f - loss));
            _rigidbody.linearVelocity = velocity;
            
            _currentHeatLevel = Mathf.Clamp(_currentHeatLevel + heat, 0, MaxHeat);
            
            // Тряска камеры (чем тяжелее был объект, тем сильнее трясет)
            // CameraShaker.Instance.Shake(loss * 0.1f);
        }
        #endregion

        #region Events
        private void OnIsStartedChange(bool _, bool started) => SetShredderState(started);
        #endregion
    }
}