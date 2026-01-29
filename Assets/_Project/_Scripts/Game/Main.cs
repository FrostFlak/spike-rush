using System;
using System.Collections.Generic;
using Alchemy.Inspector;
using Alchemy.Serialization;
using Helpers;
using UI;
using UnityEngine;

namespace Game {
    [AlchemySerialize]
    public partial class Main : SingletonMonoBehaviour<Main> {

        #region SerializedFields
        [Header("Components")]
        [field: SerializeField] public Shredder Shredder { get; private set; }
        [Header("UI")]
        [SerializeField] private TapToPlayUI _tapToPlayUI;
        [SerializeField] private UpgradesUI _upgradesUI;
        [SerializeField] private CurrencyUI _currencyUI;
        [SerializeField] private SettingsUI _settingsUI;
        [SerializeField] private EndUI _endUI;
        [Header("Storage")]
        [AlchemySerializeField, NonSerialized] public Dictionary<Models.CurrencyType, Sprite> CurrencyIcons;
        [AlchemySerializeField, NonSerialized] public Dictionary<int, LevelData> PredifinedLevelsPb;
        #endregion

        #region PrivateFields
        private Models.Shredder _shredderData;
        private Models.Settings _settingsData;
        #endregion
        
        #region Properties
        public StateManager StateManager { get; private set; }
        public LevelManager LevelManager  { get; private set; }
        
        public int CurrentLevelID { get; set; }
        public Models.Currency CurrencyData { get; private set; }
        public List<Models.Level> LevelsData { get; private set; }
        
        public int CurrentTraversedDistance { get; set; }
        public int RunReceivedCoins { get; set; }
        public int RunReceivedDiamonds { get; set; }
        public Observable<int> LastRecordDistance { get; private set; }
        #endregion

        #region Behaviour
        protected override void Awake() {
            base.Awake();

            StateManager = new StateManager();
                
            Load();
            Initialize();
            StateManager.State.OnUpdate += OnGameStateChange;
            CurrencyData.Coins.OnUpdate += OnCoinsChange;
            CurrencyData.Diamonds.OnUpdate += OnDiamondsChange;
        }
        
        protected override void OnDisable() {
            base.OnDisable();

            Save();
            Deinitialize();
            StateManager.State.OnUpdate -= OnGameStateChange;
            CurrencyData.Coins.OnUpdate -= OnCoinsChange;
            CurrencyData.Diamonds.OnUpdate -= OnDiamondsChange;
        }

        private void Initialize() {
            LevelManager = new LevelManager();
            LastRecordDistance = new Observable<int>(LevelsData[CurrentLevelID - 1].RecordDistance.Value());
            
            Shredder.Initialize(_shredderData);
            _tapToPlayUI.Initialize();
            _upgradesUI.Initialize(_shredderData);
            _currencyUI.Initialize();
            _settingsUI.Initialize(_settingsData);
            _endUI.Initialize();
        }
        
        private void Deinitialize() {
            LevelManager.Deinitialize();
            
            Shredder.Deinitialize();
            _tapToPlayUI.Deinitialize();
            _upgradesUI.Deinitialize();
            _currencyUI.Deinitialize();
            _settingsUI.Deinitialize();
            _endUI.Deinitialize();
        }
        #endregion

        #region Saves
        private void Load() {
            var defaultShredder = new Models.Shredder {
                FuelData = new Models.UpgradeData {
                    Level = new Observable<int>(1),
                    Power = Constants.DefaultFuel,
                    InvestedStep = new Observable<int>(0),
                },
                AccelerationData = new Models.UpgradeData {
                    Level = new Observable<int>(1),
                    Power = Constants.DefaultAcceleration,
                    InvestedStep = new Observable<int>(0),
                },
                PowerData = new Models.UpgradeData {
                    Level = new Observable<int>(1),
                    Power = Constants.DefaultPower,
                    InvestedStep = new Observable<int>(0),
                },
            };
            _shredderData = SavingSystem.GetOrCreate(Constants.ShredderSaveKey, defaultShredder);

            var defaultCurrency = new Models.Currency {
                Coins = new Observable<int>(0),
                Diamonds = new Observable<int>(0),
            };
            CurrencyData = SavingSystem.GetOrCreate(Constants.CurrencySaveKey, defaultCurrency);

            var defaultLevels = new List<Models.Level>();
            foreach (var predifinedLvl in PredifinedLevelsPb) {
                var lvlData = new Models.Level();
                lvlData.ID = predifinedLvl.Key;
                lvlData.IsReached = new OwnedObservable<Models.Level, bool>(lvlData, false);
                lvlData.RecordDistance = new Observable<int>(0);
                defaultLevels.Add(lvlData);
            }
            LevelsData = SavingSystem.GetOrCreate(Constants.LevelsSaveKey, defaultLevels);
            CurrentLevelID = SavingSystem.GetOrCreate(Constants.CurrentLvlIDSaveKey, 1);

            var defaultSettings = new Models.Settings {
                SfxActive = new Observable<bool>(true),
                MusicActive = new Observable<bool>(true),
                HighFpsActive = new Observable<bool>(true),
            };
            _settingsData = SavingSystem.GetOrCreate(Constants.SettingsSaveKey, defaultSettings);
        }

        private void Save() {
            SavingSystem.Save(_shredderData, Constants.ShredderSaveKey);
            SavingSystem.Save(CurrencyData, Constants.CurrencySaveKey);
            SavingSystem.Save(LevelsData, Constants.LevelsSaveKey);
            SavingSystem.Save(CurrentLevelID, Constants.CurrentLvlIDSaveKey);
            SavingSystem.Save(_settingsData, Constants.SettingsSaveKey);
        }
        #endregion
        
        private void OnGameStateChange(StateManager.GameState arg1, StateManager.GameState state) {
            if (state is StateManager.GameState.Win or StateManager.GameState.Lose) {
                if (LastRecordDistance.Value() >= CurrentTraversedDistance)
                    return;
                
                LastRecordDistance.Set(CurrentTraversedDistance);
            }
            else if (state is StateManager.GameState.GatheredReward) {
                Save();
            }
        }
        
        private void OnCoinsChange(int arg1, int arg2) => Save();

        private void OnDiamondsChange(int arg1, int arg2) => Save();

        #region DEBUG
        [Button]
        private void GiveCoins(int amount) => CurrencyData.Add(Models.CurrencyType.Coin, amount);
        
        [Button]
        private void SpendCoins(int amount) => CurrencyData.Subtract(Models.CurrencyType.Coin, amount);
        
        [Button]
        private void GiveDiamonds(int amount) => CurrencyData.Add(Models.CurrencyType.Diamond, amount);
        #endregion
    }
}