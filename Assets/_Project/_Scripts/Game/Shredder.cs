using Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace Game {
    public class Shredder : MonoBehaviour {

        #region SerializedFields
        [Header("Components")]
        [SerializeField] private Rigidbody _rigidbody;
        [Header("Movement")]
        [SerializeField] private float _accelerationForce;
        [SerializeField] private float _maxSpeed = 20f;
        [Header("Fuel")]
        [SerializeField] private float _maxFuel = 100f;
        [SerializeField] private float _fuelConsumptionRate = 10f;
        [SerializeField] private Image _fuelBarFill;
        [Header("Heat")]
        [SerializeField] private float _heatRate = 20f;
        [SerializeField] private float _coolRate = 15f;
        [SerializeField] private Image _heatBarFill;
        #endregion

        #region PrivateFields
        private float _currentFuel;
        private float _currentHeatLevel;
        private bool _isOverheated;
        private bool _isStarted;
        private bool _canMove => _currentFuel > 0 && !_isOverheated;
        #endregion

        #region Initialization
        public void StartShredder() {
            _currentFuel = _maxFuel;


            _isStarted = true;
        }
        
        public void StopShredder() {
            _currentFuel = 0;

            _isStarted = false;
        }
        #endregion

        #region Behaviour
        private void Update() {
            if (!_isStarted)
                return;
            
            HandleFuelAndHeat();
            UpdateUI();

            // Условие окончания заезда
            if (_currentFuel <= 0 && _rigidbody.linearVelocity.magnitude < 0.1f) 
                StopShredder();
        }
        
        private void FixedUpdate() => ApplyMovement();
        #endregion

        #region Fuel/Heat
        private void HandleFuelAndHeat() {
            if (Input.GetMouseButton(0) && _canMove) {
                _currentFuel -= _fuelConsumptionRate * Time.deltaTime;
                _currentHeatLevel += _heatRate * Time.deltaTime;
                if (_currentHeatLevel >= 100f) 
                    OverheatLock();
            }
            else {
                _currentHeatLevel = Mathf.MoveTowards(_currentHeatLevel, 0f, _coolRate * Time.deltaTime);
                if (_isOverheated && _currentHeatLevel <= 0) {
                    _isOverheated = false;
                    _heatBarFill.color = Color.white;
                }
            }
        }
        
        private void OverheatLock() {
            _isOverheated = true;
            _currentHeatLevel = 100f;
            _heatBarFill.color = Color.red;
            _rigidbody.linearVelocity *= 0.75f;
        }
        #endregion

        #region Movement
        private void ApplyMovement() {
            if (Input.GetMouseButton(0) && _canMove) {
                if (_rigidbody.linearVelocity.z < _maxSpeed) {
                    _rigidbody.AddForce(Vector3.forward * _accelerationForce, ForceMode.Acceleration);
                }
            }

            Log.Debug($"Linear: {_rigidbody.linearVelocity.magnitude}");
        }
        #endregion

        #region Collisions
        private void OnTriggerEnter(Collider other) {
            if (!other.TryGetComponent(out Destructibile destructibile)) 
                return;
            
            ApplyImpact(destructibile.speedLoss, destructibile.heatPenalty);
            destructibile.SpawnFragments();
        }

        private void ApplyImpact(float loss, float heat) {
            Vector3 v = _rigidbody.linearVelocity;
            v.z = Mathf.Max(0, v.z - loss);
            _rigidbody.linearVelocity = v;

            _currentHeatLevel += heat;

            // Тряска камеры (чем тяжелее был объект, тем сильнее трясет)
            // CameraShaker.Instance.Shake(loss * 0.1f); 
        }
        #endregion

        #region UI
        private void UpdateUI() {
            if (_fuelBarFill != null) _fuelBarFill.fillAmount = _currentFuel / _maxFuel;
            if (_heatBarFill != null) _heatBarFill.fillAmount = _currentHeatLevel / 100f;
        }
        #endregion
    }
}