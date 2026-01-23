using UnityEngine;

namespace Game {
    public class FinishZone : MonoBehaviour {
        private void OnTriggerEnter(Collider other) {
            if (!other.TryGetComponent(out Shredder shredder))
                return;

            Main.Instance.StateManager.FinishLvl();
        }
    }
}