using UnityEngine;

namespace Game {
    public class Destructibile : MonoBehaviour {

        public GameObject fracturedObj;
        public float explosionForce = 5f;

        [Header("Настройки прочности")]
        public float speedLoss = 5f; // Потеря скорости при пробитии
        public float minSpeedLoss = 0.5f;
        public float heatPenalty = 10f; // Нагрев от удара

        public void SpawnFragments() {
            Rigidbody[] rbChilds = fracturedObj.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in rbChilds) {
                rb.useGravity = true;
                rb.AddExplosionForce(explosionForce, transform.position + Vector3.up * 2, 5f);
            }

            Vector3 explosionPos = transform.position + Vector3.up * 15f;

            foreach (Rigidbody rb in rbChilds) {
                rb.useGravity = true;
                rb.AddExplosionForce(explosionForce, explosionPos, 5f);

                Vector3 randomDirection = new Vector3(
                    Random.Range(-1f, 1f), Random.Range(0.5f, 1.5f), Random.Range(-1f, 1f));

                rb.AddForce(randomDirection * (explosionForce * 0.3f), ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * explosionForce, ForceMode.Impulse);
            }

            Destroy(fracturedObj, 3f);
        }
        
        public float GetSpeedLoss(float maxSpeed) {
            // Выбираем, что больше: процент от макс. скорости или минимальный порог
            return Mathf.Max(maxSpeed * speedLoss, minSpeedLoss);
        }
    }
}