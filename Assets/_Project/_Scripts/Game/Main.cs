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
        [SerializeField] private EndUI _endUI;
        [Header("Storage")]
        [AlchemySerializeField, NonSerialized] public Dictionary<Models.CurrencyType, Sprite> CurrencyIcons;
        [AlchemySerializeField, NonSerialized] public Dictionary<int, LevelData> PredifinedLevelsPb;
        #endregion

        #region PrivateFields
        private Models.Shredder _shredderData;
        private List<Models.Level> _levelsData;

        #endregion
        
        #region Properties
        public StateManager StateManager { get; private set; }
        public LevelManager LevelManager  { get; private set; }
        
        public Models.Currency CurrencyData { get; private set; }
        public int CurrentLevelID { get; set; }
        public Observable<int> RunReceivedMoney { get; private set; }
        public Observable<int> RunReceivedCrystals { get; private set; }
        #endregion

        #region Behaviour
        protected override void Awake() {
            base.Awake();

            StateManager = new StateManager();
                
            Load();
            Initialize();
        }

        protected override void OnDisable() {
            base.OnDisable();

            Save();
            Deinitialize();
        }

        private void Initialize() {
            LevelManager = new LevelManager(_levelsData);
            
            Shredder.Initialize(_shredderData);
            _tapToPlayUI.Initialize();
            _upgradesUI.Initialize(_shredderData);
            _currencyUI.Initialize(CurrencyData);
            _endUI.Initialize();
        }
        
        private void Deinitialize() {
            LevelManager.Deinitialize();
            
            Shredder.Deinitialize();
            _tapToPlayUI.Deinitialize();
            _upgradesUI.Deinitialize();
            _currencyUI.Deinitialize();
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
                }
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
            _levelsData = SavingSystem.GetOrCreate(Constants.LevelsSaveKey, defaultLevels);
            CurrentLevelID = SavingSystem.GetOrCreate(Constants.CurrentLvlIDSaveKey, 1);
        }

        private void Save() {
            SavingSystem.Save(_shredderData, Constants.ShredderSaveKey);
            SavingSystem.Save(CurrencyData, Constants.CurrencySaveKey);
            SavingSystem.Save(_levelsData, Constants.LevelsSaveKey);
            SavingSystem.Save(CurrentLevelID, Constants.CurrentLvlIDSaveKey);
        }
        #endregion

        #region DEBUG
        [Button]
        private void GiveCoins(int amount) => CurrencyData.Add(Models.CurrencyType.Coin, amount);
        
        [Button]
        private void GiveDiamonds(int amount) => CurrencyData.Add(Models.CurrencyType.Diamond, amount);
        #endregion
    }
}