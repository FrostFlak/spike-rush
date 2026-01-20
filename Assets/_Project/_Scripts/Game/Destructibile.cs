using UnityEngine;

namespace Game {
    public class Destructibile : MonoBehaviour {

        #region SerializedFields
        [Header("Components")]
        [SerializeField] private GameObject _nonDestructed;
        [SerializeField] private GameObject _destructed;
        [SerializeField] private Collider _collider;
        [Header("Properties")]
        [SerializeField] private float _explosionForce = 5f;

        [field: SerializeField, Range(1, 99)] public float SpeedLoss { get; private set; }
        [field: SerializeField] public float HeatPenalty { get; private set; }
        [field: SerializeField] public int MoneyPayout { get; private set; }
        #endregion

        #region PrivateFields
        #endregion

        #region Destroy
        public void SpawnFragments() {
            Rigidbody[] rbChilds = _destructed.GetComponentsInChildren<Rigidbody>();
            foreach (var rb in rbChilds) {
                rb.useGravity = true;
                rb.AddExplosionForce(_explosionForce, transform.position + Vector3.up * 2, 5f);
            }

            Vector3 explosionPos = transform.position + Vector3.up * 15f;

            foreach (var rb in rbChilds) {
                rb.useGravity = true;
                rb.AddExplosionForce(_explosionForce, explosionPos, 5f);

                Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 1.5f), Random.Range(-1f, 1f));

                rb.AddForce(randomDirection * (_explosionForce * 0.3f), ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * _explosionForce, ForceMode.Impulse);
            }

            _collider.enabled = false;
            _destructed.SetActive(false);
            
            Destroy(_destructed, 2f);
            Destroy(gameObject, 3f);
        }
        #endregion
    }
}