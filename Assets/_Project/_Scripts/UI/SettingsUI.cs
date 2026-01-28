using System.Collections.Generic;
using Game;
using Helpers;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace UI {
    public class SettingsUI : MonoBehaviour {

        #region SerializedFields
        [Header("Components")]
        [SerializeField] private AudioMixer _audioMixer;
        [Header("Rect")]
        [SerializeField] private RectTransform _settingsPanel;
        [Header("Buttons")]
        [SerializeField] private List<Button> _closePanelBtn;
        [SerializeField] private Button _settingsBtn;
        [SerializeField] private Button _restartLvlBtn;
        [Header("Toggle")]
        [SerializeField] private Toggle _sfxToggle;
        [SerializeField] private Toggle _musicToggle;
        [SerializeField] private Toggle _highFpsToggle;
        #endregion

        #region PrivateFields
        private Models.Settings _settingsData;
        #endregion

        #region Initialization
        public void Initialize(Models.Settings settingsData) {
            _settingsData = settingsData;

            _settingsData.SfxActive.OnUpdate += OnSettingsToggleChange;
            _settingsData.MusicActive.OnUpdate += OnSettingsToggleChange;
            _settingsData.HighFpsActive.OnUpdate += OnSettingsToggleChange;
            
            Main.Instance.StateManager.State.OnUpdate += OnGameStateChange;
            
            _settingsBtn.onClick.AddListener(() => SetSettingsPanelState(true));
            _restartLvlBtn.onClick.AddListener(StopLevel);
            _closePanelBtn.ForEach(b => b.onClick.AddListener(() => SetSettingsPanelState(false)));
            
            _sfxToggle.onValueChanged.AddListener(OnSfxToggleClick);
            _musicToggle.onValueChanged.AddListener(OnMusicToggleClick);
            _highFpsToggle.onValueChanged.AddListener(OnLowFpsToggleClick);
            SetTogglesState();
        }

        private void SetTogglesState() {
            _sfxToggle.isOn = _settingsData.SfxActive.Value();
            _musicToggle.isOn = _settingsData.MusicActive.Value();
            _highFpsToggle.isOn = _settingsData.HighFpsActive.Value();

            _audioMixer.SetFloat("SFX", _settingsData.SfxActive.Value() ? 0 : -80);
            _audioMixer.SetFloat("Music", _settingsData.MusicActive.Value() ? 0 : -80);
            Application.targetFrameRate = _settingsData.HighFpsActive.Value() ? 60 : 30;
        }
        
        public void Deinitialize() {
            _settingsData.SfxActive.OnUpdate -= OnSettingsToggleChange;
            _settingsData.MusicActive.OnUpdate -= OnSettingsToggleChange;
            _settingsData.HighFpsActive.OnUpdate -= OnSettingsToggleChange;
            
            Main.Instance.StateManager.State.OnUpdate -= OnGameStateChange;
            
            _settingsBtn.onClick.RemoveAllListeners();
            _restartLvlBtn.onClick.RemoveAllListeners();
            _closePanelBtn.ForEach(b => b.onClick.RemoveAllListeners());
            _sfxToggle.onValueChanged.RemoveAllListeners();
            _musicToggle.onValueChanged.RemoveAllListeners();
            _highFpsToggle.onValueChanged.RemoveAllListeners();
        }
        #endregion

        #region UI
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
        #endregion

        #region Events
        private void OnSettingsToggleChange(bool arg1, bool arg2) => SetTogglesState();

        private void OnGameStateChange(StateManager.GameState _, StateManager.GameState state) => _restartLvlBtn.gameObject.SetActive(state is StateManager.GameState.Playing);
        
        private void OnSfxToggleClick(bool active) => _settingsData.SfxActive.Set(active);
        
        private void OnMusicToggleClick(bool active) => _settingsData.MusicActive.Set(active);
        
        private void OnLowFpsToggleClick(bool active) => _settingsData.HighFpsActive.Set(active);
        #endregion
    }
}