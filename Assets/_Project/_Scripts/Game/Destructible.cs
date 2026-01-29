using System.Collections.Generic;
using UnityEngine;

namespace Game {
    public class Destructible : MonoBehaviour {

        #region SerializedFields
        [Header("Components")]
        [SerializeField] private GameObject _nonDestructed;
        [SerializeField] private GameObject _destructed;
        [SerializeField] private Collider _collider;
        [SerializeField] private List<Rigidbody> _destructedRb;
        [Header("Properties")]
        [SerializeField] private AudioController.BreakSFX _breakSfxType;
        [SerializeField, Range(1, 99)] private float _explosionForce = 5f;
        [field: SerializeField, Range(1, 99)] public float SpeedLoss { get; private set; }
        [field: SerializeField] public int CoinsPayout { get; private set; }
        #endregion

        #region Behaviour
        private void OnEnable() {
            _destructed.SetActive(false);
            _nonDestructed.SetActive(true);
            
            foreach (var rb in _destructedRb)
                rb.isKinematic = true;
        }
        #endregion

        #region Destroy
        public void SpawnFragments() {
            _collider.enabled = false;
            _destructed.SetActive(true);
            _nonDestructed.SetActive(false);
            
            Vector3 explosionPos = transform.position + Vector3.up * 15f;
            foreach (var rb in _destructedRb) {
                rb.isKinematic = false;
                rb.AddExplosionForce(_explosionForce, explosionPos, 5f);

                Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 1.5f), Random.Range(-1f, 1f));

                rb.AddForce(randomDirection * (_explosionForce * 0.3f), ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * _explosionForce, ForceMode.Impulse);
            }

            AudioController.Instance.PlayBreak(_breakSfxType, true);
            
            Destroy(_destructed, 2f);
            Destroy(gameObject, 3f);
        }
        #endregion
    }
}