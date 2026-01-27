using Helpers;
using UI;
using UnityEngine;

namespace Game {
    public class Shredder : MonoBehaviour {

        #region SerializedFields
        [field: Header("Components")]
        [field: SerializeField] public Rigidbody Rigidbody { get; private set; }
        [SerializeField] private Joystick _joystick;
        [SerializeField] private HUD _hud;
        [Header("Properties")]
        [Header("Movement(m/s)")]
        [SerializeField] private float _breakingForce;
        [SerializeField] private float _stoppingForce;
        [Header("Fuel")]
        [SerializeField] private float _fuelConsumptionRate;
        [Header("Heat")]
        [SerializeField] private float _heatRate = 20f;
        [SerializeField] private float _coolRate = 15f;
        [Header("Prefabs")]
        [SerializeField] private WorldSpacePayout _worldSpacePayoutPb;
        #endregion

        #region PrivateFields
        private const int MaxHeat = 100;
        private const float SideMovementMultiplier = 3.5f;
        
        private Models.Shredder _shredderData;
        private ObjectPool<WorldSpacePayout> _worldSpacePayoutPool;
        
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
            _worldSpacePayoutPool = new ObjectPool<WorldSpacePayout>(_worldSpacePayoutPb, 5);
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
            Rigidbody.isKinematic = !active;
        }
        
        public void ReturnToStart(Vector3 position) => Rigidbody.position = position;
        #endregion

        #region Behaviour
        private void Update() {
            if (_isActive) {
                _input = GatherInput();
                
                _hud.UpdateFuelUI(_currentFuel, _shredderData.FuelData.Power);
                _hud.UpdateHeatUI(_currentHeatLevel, MaxHeat, _isOverheated);
                _hud.UpdateSpeedUI(Mathf.RoundToInt(Rigidbody.linearVelocity.magnitude * 3.6f));
            }

            Main.Instance.CurrentTraversedDistance = Mathf.RoundToInt(Vector3.Distance(transform.position, Main.Instance.LevelManager.ActiveLevel.StartTransform.position));
            _hud.UpdateDistanceLabel();
            _hud.UpdateLvlProgressBar();
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

            if (_currentFuel <= 0 && Rigidbody.linearVelocity.magnitude <= 0.25f) 
                Lose();

            _currentHeatLevel = Mathf.Clamp(_currentHeatLevel, 0, MaxHeat);
        }
        
        private void TriggerOverheat() {
            _isOverheated = true;
            _currentHeatLevel = MaxHeat;
            Rigidbody.linearVelocity *= 0.8f;
        }
        
        private void Lose() {
            if (!_isActive) 
                return;
            
            Rigidbody.linearVelocity = Vector3.zero;
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
    
            float currentZVelocity = Rigidbody.linearVelocity.z;

            if (CanMove && movement.sqrMagnitude > 0.01f) {
                float finalMoveForce = _shredderData.AccelerationData.Power;
                bool isReversing = (moveVertical > 0 && currentZVelocity < -0.1f) || 
                                   (moveVertical < 0 && currentZVelocity > 0.1f);

                if (isReversing)
                    finalMoveForce *= _breakingForce;

                Rigidbody.AddForce(Vector3.forward * moveVertical * finalMoveForce, ForceMode.Acceleration);
                Rigidbody.AddForce(Vector3.right * moveHorizontal * _shredderData.AccelerationData.Power * SideMovementMultiplier, ForceMode.Acceleration);

                _currentFuel -= _fuelConsumptionRate * Time.fixedDeltaTime;
                _currentHeatLevel += _heatRate * Time.fixedDeltaTime;

                if (_currentHeatLevel >= MaxHeat) 
                    TriggerOverheat();
            } 
            else {
                if (Rigidbody.linearVelocity.magnitude > 0.1f)
                    Rigidbody.AddForce(-Rigidbody.linearVelocity.normalized * _stoppingForce, ForceMode.Acceleration);
            }

            int maxSpeed = Constants.GetMaxSpeedByAccelerationLvl(_shredderData.AccelerationData.Level.Value());
            Rigidbody.linearVelocity = Vector3.ClampMagnitude(Rigidbody.linearVelocity, maxSpeed);
        }
        #endregion

        #region Collisions
        private void OnTriggerEnter(Collider other) {
            if (!other.TryGetComponent(out Destructible destructibile)) 
                return;
            
            ApplyImpact(destructibile.SpeedLoss / 100f, destructibile.HeatPenalty);
            destructibile.SpawnFragments();
            var payoutUI = _worldSpacePayoutPool.Get(destructibile.transform.position, Quaternion.identity);
            payoutUI.SetPayout(destructibile.CoinsPayout, Models.CurrencyType.Coin, () => _worldSpacePayoutPool.Return(payoutUI));

            var totalUpgradesLvl = _shredderData.FuelData.Level.Value() + _shredderData.PowerData.Level.Value() + _shredderData.AccelerationData.Level.Value();
            Main.Instance.RunReceivedCoins += Constants.GetCoinsByObject(destructibile.CoinsPayout, totalUpgradesLvl);
        }

        private void ApplyImpact(float loss, float heat) {
            Rigidbody.linearVelocity *= (1f - loss);
            
            _currentHeatLevel += heat;
            if (_currentHeatLevel >= MaxHeat) 
                TriggerOverheat();
        }
        #endregion
        
        #region Events
        private void OnGameStateChange(StateManager.GameState arg1, StateManager.GameState state) {
            switch (state) {
                case StateManager.GameState.Lose or StateManager.GameState.Win:
                    var totalUpgradesLvl = _shredderData.FuelData.Level.Value() + _shredderData.PowerData.Level.Value() + _shredderData.AccelerationData.Level.Value();
                    var levelDistance = Mathf.RoundToInt(Vector3.Distance(Main.Instance.LevelManager.ActiveLevel.StartTransform.position, Main.Instance.LevelManager.ActiveLevel.EndTransform.position)); 
                    Main.Instance.RunReceivedCoins += Constants.GetCoinsByDistance(Main.Instance.CurrentTraversedDistance, levelDistance, totalUpgradesLvl);
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