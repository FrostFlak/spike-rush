using Helpers;
using UnityEngine;

namespace Models {
    public class Shredder {

        public UpgradeData FuelData;
        public UpgradeData PowerData;
        public UpgradeData AccelerationData;
    }

    public class UpgradeData {
        public Observable<int> Level;
        public Observable<int> InvestedStep;
        public float Power;
        public float DefaultValue;
        
        public void Upgrade(float powerStep) {
            InvestedStep.Set(Mathf.Clamp(InvestedStep.Value() + 1, 0, Constants.StepsToNewLevel));
            if (InvestedStep.Value() < Constants.StepsToNewLevel) 
                return;

            Power += powerStep;
            Level.Set(Level.Value() + 1);
            InvestedStep.Set(0);
        }

        public void ResetUpgrade() {
            Power = DefaultValue;
            Level.Set(1);
            InvestedStep.Set(0);
        }
    }
}