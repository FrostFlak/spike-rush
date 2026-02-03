using UnityEngine;

namespace Game {
    public class Water : MonoBehaviour {

        private void OnTriggerEnter(Collider other) {
            if (!other.TryGetComponent(out Shredder _))
                return;

            AudioController.Instance.PlayEnvironment(AudioController.EnvironmentSFX.WaterSplash);
            Main.Instance.StateManager.FailLvl();
        }
    }
}