using UnityEngine;

namespace Game {
    public class SpeedZone : MonoBehaviour {

        [SerializeField] private bool _boost;
        [SerializeField] private float _forcePower;
        
        private void OnTriggerEnter(Collider other) {
            if (!other.TryGetComponent(out Shredder shredder))
                return;

            Vector3 direction = _boost ? Vector3.forward : Vector3.back;
            shredder.ApplyBoost(_forcePower, direction);
            AudioController.Instance.PlayEnvironment(AudioController.EnvironmentSFX.BoostPlatform);
        }
    }
}