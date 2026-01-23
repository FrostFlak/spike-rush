using UI;
using Unity.Logging;
using UnityEngine;

namespace Game {
    public class Shredder : MonoBehaviour {

        #region SerializedFields
        [Header("Components")]
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Joystick _joystick;
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
        
        private Vector3 _input;
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
            
            Main.Instance.StateManager.State.OnUpdate += OnGameStateChange;
        }

        public void Deinitialize() {
            _hud.Deinitialize();
            
            Main.Instance.StateManager.State.OnUpdate -= OnGameStateChange;
        }
        #endregion

        #region State
        private void SetShredderState(bool active) {
            transform.rotation = Quaternion.identity;
            _currentFuel = _shredderData.FuelData.Power;
            _currentHeatLevel = 0f;
            _isOverheated = false;
            _isActive = active;
            _rigidbody.isKinematic = !active;
        }
        
        public void ReturnToStart(Vector3 position) => _rigidbody.position = position;
        #endregion

        #region Behaviour
        private void Update() {
            if (_isActive) {
                _input = GatherInput();
                
                _hud.UpdateFuelUI(_currentFuel, _shredderData.FuelData.Power);
                _hud.UpdateHeatUI(_currentHeatLevel, MaxHeat, _isOverheated);
                _hud.UpdateSpeedUI(Mathf.RoundToInt(_rigidbody.linearVelocity.magnitude * 3.6f));
            }

            _hud.UpdateDistanceLabel(Mathf.RoundToInt(Vector3.Distance(transform.position, Main.Instance.LevelManager.ActiveLevel.StartTransform.position)));
            _hud.UpdateLvlProgressBar(transform);
        }

        private void FixedUpdate() {
            if (!_isActive) 
                return;
            
            ApplyMovement();
            HandleFuelAndHeat();
        }
        #endregion

        #region Fuel/Heat
        private void HandleFuelAndHeat() {
            if (_currentHeatLevel > 0)
                _currentHeatLevel -= _coolRate * Time.fixedDeltaTime;
            
            if (_isOverheated && _currentHeatLevel <= 0) {
                _isOverheated = false;
                _currentHeatLevel = 0;
            }

            if (_currentFuel <= 0 && _rigidbody.linearVelocity.magnitude <= 0.25f) 
                Lose();

            _currentHeatLevel = Mathf.Clamp(_currentHeatLevel, 0, MaxHeat);
        }
        
        private void TriggerOverheat() {
            _isOverheated = true;
            _currentHeatLevel = MaxHeat;
            _rigidbody.linearVelocity *= 0.8f;
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
        
        private void ApplyMovement() {
            float moveVertical = _input.z;
            float moveHorizontal = _input.x;

            Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical).normalized;

            if (CanMove && movement.sqrMagnitude > 0.01f) {
                if (_rigidbody.linearVelocity.magnitude < _maxSpeed) 
                    _rigidbody.AddForce(movement * _shredderData.AccelerationData.Power, ForceMode.Acceleration);

                _currentFuel -= _fuelConsumptionRate * Time.fixedDeltaTime;
                _currentHeatLevel += _heatRate * Time.fixedDeltaTime;

                if (_currentHeatLevel >= MaxHeat) 
                    TriggerOverheat();
            } 
            else {
                if (_rigidbody.linearVelocity.magnitude > 0.1f) 
                    _rigidbody.AddForce(-_rigidbody.linearVelocity.normalized * _stoppingAccelerationForce, ForceMode.Acceleration);
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
            _rigidbody.linearVelocity *= (1f - loss);
            
            _currentHeatLevel += heat;
            if (_currentHeatLevel >= MaxHeat) 
                TriggerOverheat();
        }
        #endregion
        
        #region Events
        private void OnGameStateChange(StateManager.GameState arg1, StateManager.GameState state) {
            switch (state) {
                case StateManager.GameState.Lose or StateManager.GameState.Win:
                    SetShredderState(false);
                    break;
                
                case StateManager.GameState.Playing:
                    SetShredderState(true);
                    break;
            }
        }
        #endregion
    }
}