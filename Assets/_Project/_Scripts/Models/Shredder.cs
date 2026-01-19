using System;
using Helpers;
using UnityEngine;

namespace Models {
    [Serializable]
    public class Shredder {
        public UpgradeData FuelData;
        public UpgradeData PowerData;
        public UpgradeData AccelerationData;

        private bool ProcessUpgrade(UpgradeData data, float powerStep) {
            data.Level.Set(data.Level.Value() + 1);
            data.Power.Set(data.Power.Value() + powerStep);
            data.Price.Set(Mathf.RoundToInt(data.Price.Value() * Constants.PriceMultiplier));
                
            return true;
        }

        public bool TryUpgradeFuel() => ProcessUpgrade(FuelData, 20f);
        public bool TryUpgradePower() => ProcessUpgrade(PowerData, 0.5f);
        public bool TryUpgradeSpeed() => ProcessUpgrade(AccelerationData, 5f);
    }

    [Serializable]
    public class UpgradeData {
        public Observable<int> Level;
        public Observable<float> Power;
        public Observable<int> Price;
    }
}