using UnityEngine;

namespace Game {
    public class Water : MonoBehaviour {

        private void Update() {
            if (Main.Instance == null)
                return;

            var vector = new Vector3(Main.Instance.Shredder.transform.position.x, transform.position.y, Main.Instance.Shredder.transform.position.z + 100f);
            transform.position = Vector3.Lerp(transform.position, vector, Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other) {
            if (!other.TryGetComponent(out Shredder shredder))
                return;

            AudioController.Instance.PlayEnvironment(AudioController.EnvironmentSFX.WaterSplash);
            Main.Instance.StateManager.FailLvl();
        }
    }
}