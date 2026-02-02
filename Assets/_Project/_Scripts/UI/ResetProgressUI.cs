using Game;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class ResetProgressUI : MonoBehaviour {

        [SerializeField] private Button _okBtn;
        [SerializeField] private Button _noBtn;

        private void Awake() {
            _okBtn.onClick.AddListener(OnClickOk);
            _noBtn.onClick.AddListener(OnClickNo);
        }

        private void OnDestroy() {
            _okBtn.onClick.RemoveAllListeners();
            _noBtn.onClick.RemoveAllListeners();
        }

        private void OnClickOk() => Main.Instance.ResetProgress();

        private void OnClickNo() => gameObject.SetActive(false);
    }
}