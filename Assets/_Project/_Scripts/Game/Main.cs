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
        [Header("Storage")]
        [AlchemySerializeField, NonSerialized] public Dictionary<Models.CurrencyType, Sprite> CurrencyIcons;
        #endregion

        #region PrivateFields
        private Models.Shredder _shredderData;
        private List<Models.Level> _levelsData;
        #endregion
        
        #region Properties
        public Observable<bool> IsStarted { get; private set; }
        public Models.Currency CurrencyData { get; private set; }
        public Observable<int> RunReceivedMoney { get; private set; }
        public Observable<int> RunReceivedCrystals { get; private set; }
        #endregion

        #region Behaviour
        protected override void Awake() {
            base.Awake();
            
            IsStarted = new Observable<bool>(false);
            
            Load();
            Initialize();
        }

        protected override void OnDisable() {
            base.OnDisable();

            Deinitialize();
        }

        private void Initialize() {
            _tapToPlayUI.Initialize();
            _upgradesUI.Initialize(_shredderData);
            _currencyUI.Initialize(CurrencyData);
        }
        
        private void Deinitialize() {
            _tapToPlayUI.Deinitialize();
            _upgradesUI.Deinitialize();
            _currencyUI.Deinitialize();
            
            Shredder.Deinitialize();
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
            Shredder.Initialize(_shredderData);

            var defaultCurrency = new Models.Currency {
                Coins = new Observable<int>(0),
                Diamonds = new Observable<int>(0),
            };
            CurrencyData = SavingSystem.GetOrCreate(Constants.CurrencySaveKey, defaultCurrency);

            var defaultLevels = new List<Models.Level> {
                new() {
                    ID = 1,
                    IsReached = new Observable<bool>(false),
                    CurrentDistance = new Observable<int>(0),
                },
            };
            _levelsData = SavingSystem.GetOrCreate(Constants.LevelsSaveKey, defaultLevels);
        }
        #endregion

        #region Events
        
        #endregion

        #region DEBUG
        [Button]
        private void ToggleIsStarted() => IsStarted.Set(!IsStarted.Value());
        
        [Button]
        private void GiveCoins(int amount) => CurrencyData.Add(Models.CurrencyType.Coin, amount);
        
        [Button]
        private void GiveDiamonds(int amount) => CurrencyData.Add(Models.CurrencyType.Diamond, amount);
        #endregion
    }
}