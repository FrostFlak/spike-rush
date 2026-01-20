using Game;
using Helpers;
using TMPro;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class EndUI : MonoBehaviour {

        #region SerializedFields
        [Header("RectTransforms")]
        [SerializeField] private RectTransform _panel;
        [Header("Buttons")]
        [SerializeField] private Button _xButton;
        [SerializeField] private Button _takeBtn;
        [Header("Labels")]
        [SerializeField] private TMP_Text _coinsLabel;
        [SerializeField] private TMP_Text _diamondsLabel;
        [SerializeField] private TMP_Text _distanceLabel;
        [SerializeField] private TMP_Text _newRecordLabel;
        #endregion

        #region PrivateFields
        private const float PanelPopDuration = .35f;
        
        #endregion

        #region Initialization
        public void Initialize() {
            Main.Instance.IsStarted.OnUpdate += OnIsStartedChange;
            
            _xButton.onClick.AddListener(OnClickTakeWithMultiplier);
            _takeBtn.onClick.AddListener(OnClickTake);
        }

        public void Deinitialize() {
            Main.Instance.IsStarted.OnUpdate -= OnIsStartedChange;
            
            _xButton.onClick.RemoveAllListeners();
            _takeBtn.onClick.RemoveAllListeners();
        }
        #endregion

        #region UI
        private void SetState(bool active) {
            _panel.gameObject.SetActive(true);
            _panel.Pop(
                active,
                PanelPopDuration,
                onComplete: () => {
                    if (!active)
                        _panel.gameObject.SetActive(false);
                }
            );
        }
        
        private void OnClickTakeWithMultiplier() {
            // Show Rewarded Ad
        }
        
        private void OnClickTake() {
            // Grant Reward
        }
        
        private void SetLabels() {
            // _coinsLabel.SetText();
        }
        #endregion

        #region Events
        private void OnIsStartedChange(bool arg1, bool started) {
            if (started)
                return;

            SetState(true);
        }
        #endregion
    }
}