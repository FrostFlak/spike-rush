using UnityEngine;

namespace Game {
    public class SpeedZone : MonoBehaviour {

        [SerializeField] private bool _boost;
        [SerializeField, Range(3, 30)] private float _forcePower;
        
        private void OnTriggerEnter(Collider other) {
            if (!other.TryGetComponent(out Shredder shredder))
                return;

            shredder.Rigidbody.AddForce(_boost ? Vector3.forward : Vector3.back * _forcePower, ForceMode.Impulse);
        }
    }
}