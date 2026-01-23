using UnityEngine;

namespace Game {
    public class LevelData : MonoBehaviour {

        [field: SerializeField] public Transform StartTransform { get; private set; }
        [field: SerializeField] public Transform EndTransform { get; private set; }
    }
}