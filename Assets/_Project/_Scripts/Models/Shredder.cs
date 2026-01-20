using Helpers;
using UnityEngine;

namespace Models {
    public class Shredder {

        public UpgradeData FuelData;
        public UpgradeData PowerData;
        public UpgradeData AccelerationData;

        private void ProcessUpgrade(UpgradeData data, float powerStep) {
            data.Power += powerStep;
            data.Level.Set(data.Level.Value() + 1);
            data.InvestedStep.Set(0);
        }

        public void UpgradeFuel() {
            FuelData.InvestedStep.Set(Mathf.Clamp(FuelData.InvestedStep.Value() + 1, 0, Constants.StepsToNewLevel));
            if (FuelData.InvestedStep.Value() < Constants.StepsToNewLevel) 
                return;

            ProcessUpgrade(FuelData, Constants.FuelUpgradeStep);
        }

        public void UpgradePower() {
            PowerData.InvestedStep.Set(Mathf.Clamp(PowerData.InvestedStep.Value() + 1, 0, Constants.StepsToNewLevel));
            if (PowerData.InvestedStep.Value() < Constants.StepsToNewLevel) 
                return;

            ProcessUpgrade(PowerData, Constants.PowerUpgradeStep);
        }

        public void UpgradeAcceleration() {
            AccelerationData.InvestedStep.Set(Mathf.Clamp(AccelerationData.InvestedStep.Value() + 1, 0, Constants.StepsToNewLevel));
            if (AccelerationData.InvestedStep.Value() < Constants.StepsToNewLevel) 
                return;

            ProcessUpgrade(AccelerationData, Constants.AccelerationUpgradeStep);
        }
    }

    public class UpgradeData {
        public Observable<int> Level;
        public Observable<int> InvestedStep;
        public float Power;
    }
}