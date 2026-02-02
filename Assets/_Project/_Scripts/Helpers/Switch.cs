using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Helpers {
    [RequireComponent(typeof(Toggle))]
    public class Switch : MonoBehaviour {

        #region SerializedFields
        [Header("Components")]
        [SerializeField] private Toggle _toggle;
        [Header("Transforms")]
        [SerializeField] private Transform _onTransform;
        [SerializeField] private Transform _offTransform;
        [Header("Images")]
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _handleImage;
        [Header("Properties")]
        [SerializeField] private Color _backgroundOnColor;
        [SerializeField] private Color _backgroundOffColor;
        #endregion

        #region PrivateFields
        private const float LerpDuration = .15f;
        private Timer _lerpTimer;
        #endregion

        #region MonoBehaviour
        private void Awake() {
            _lerpTimer = new Timer(this);
            
            AddListener(isOn => SetUI(isOn));
            SetUI(_toggle.isOn, false);
        }

        private void OnDestroy() => RemoveAllListeners();
        #endregion

        #region Listeners
        public void AddListener(UnityAction<bool> action) => _toggle.onValueChanged.AddListener(action);

        public void RemoveAllListeners() => _toggle.onValueChanged.RemoveAllListeners();
        #endregion

        #region StateHandling
        public void SetActive(bool isOn, bool notify = true) {
            if (notify)
                _toggle.isOn = isOn;
            else
                _toggle.SetIsOnWithoutNotify(isOn);
        }
        #endregion

        #region UI
        private void SetUI(bool active, bool lerp = true) {
            _lerpTimer.Start(
                lerp ? LerpDuration : 0f,
                onStart: () => _toggle.interactable = false,
                onProgress: (progress) => {
                    _handleImage.transform.localPosition = active
                        ? Vector2.Lerp(_offTransform.localPosition, _onTransform.localPosition, progress)
                        : Vector2.Lerp(_onTransform.localPosition, _offTransform.localPosition, progress);
                },
                onComplete: () => {
                    _backgroundImage.color = active ? _backgroundOnColor : _backgroundOffColor;
                    _toggle.interactable = true;
                }
            );
        }
        #endregion
    }
}