using System.Collections.Generic;
using Game;
using Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class SettingsUI : MonoBehaviour {

        [Header("Rect")]
        [SerializeField] private RectTransform _settingsPanel;
        [Header("Buttons")]
        [SerializeField] private List<Button> _closePanelBtn;
        [SerializeField] private Button _settingsBtn;
        [SerializeField] private Button _restartLvlBtn;

        public void Initialize() {
            Main.Instance.StateManager.State.OnUpdate += OnGameStateChange;
            
            _settingsBtn.onClick.AddListener(() => SetSettingsPanelState(true));
            _restartLvlBtn.onClick.AddListener(StopLevel);
            _closePanelBtn.ForEach(b => b.onClick.AddListener(() => SetSettingsPanelState(false)));
        }

        public void Deinitialize() {
            Main.Instance.StateManager.State.OnUpdate -= OnGameStateChange;
            
            _settingsBtn.onClick.RemoveAllListeners();
            _restartLvlBtn.onClick.RemoveAllListeners();
            _closePanelBtn.ForEach(b => b.onClick.RemoveAllListeners());
        }
        
        private void StopLevel() {
            Main.Instance.StateManager.State.Set(StateManager.GameState.Lose);
        }
        
        private void SetSettingsPanelState(bool active) {
            _closePanelBtn.ForEach(b => {
                b.interactable = active;
                b.image.raycastTarget = active;
            });

            if (active)
                _settingsPanel.gameObject.SetActive(true);
            
            _settingsPanel.Pop(
                active,
                onComplete: () => {
                    if (!active)
                        _settingsPanel.gameObject.SetActive(false);
                }
            );
        }

        private void OnGameStateChange(StateManager.GameState _, StateManager.GameState state) {
            _restartLvlBtn.gameObject.SetActive(state is StateManager.GameState.Playing);
        }
    }
}