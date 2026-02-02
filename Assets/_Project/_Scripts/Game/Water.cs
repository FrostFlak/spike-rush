using UnityEngine;

namespace Game {
    public class Water : MonoBehaviour {

        private Vector3 _initialPosition;

        private void Awake() => _initialPosition = transform.position;

        private void Update() {
            if (Main.Instance == null)
                return;
            
            if (Main.Instance.StateManager == null)
                return;

            if (Main.Instance.StateManager.State.Value() is StateManager.GameState.Playing) {
                var vector = new Vector3(Main.Instance.Shredder.transform.position.x, transform.position.y, Main.Instance.Shredder.transform.position.z + 100f);
                transform.position = Vector3.Lerp(transform.position, vector, Time.deltaTime * 0.25f);
            }
            else if (Main.Instance.StateManager.State.Value() is StateManager.GameState.GatheredReward) {
                transform.position = _initialPosition;
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (!other.TryGetComponent(out Shredder _))
                return;

            AudioController.Instance.PlayEnvironment(AudioController.EnvironmentSFX.WaterSplash);
            Main.Instance.StateManager.FailLvl();
        }
    }
}