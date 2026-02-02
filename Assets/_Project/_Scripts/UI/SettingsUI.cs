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
        [SerializeField] private ResetProgressUI _resetProgressUI;
        [SerializeField] private AudioMixer _audioMixer;
        [Header("Rect")]
        [SerializeField] private RectTransform _settingsPanel;
        [Header("Buttons")]
        [SerializeField] private List<Button> _closePanelBtn;
        [SerializeField] private Button _settingsBtn;
        [SerializeField] private Button _restartLvlBtn;
        [SerializeField] private Button _resetProgressBtn;
        [Header("Toggle")]
        [SerializeField] private Switch _sfxSwitch;
        [SerializeField] private Switch _musicSwitch;
        [SerializeField] private Switch _highFpsSwitch;
        #endregion

        #region PrivateFields
        private Models.Settings _settingsData;
        #endregion

        #region Initialization
        public void Initialize(Models.Settings settingsData) {
            _settingsData = settingsData;

            Main.Instance.StateManager.State.OnUpdate += OnGameStateChange;
            
            _closePanelBtn.ForEach(b => b.onClick.AddListener(() => SetSettingsPanelState(false)));
            _settingsBtn.onClick.AddListener(() => SetSettingsPanelState(true));
            _restartLvlBtn.onClick.AddListener(StopLevel);
            _resetProgressBtn.onClick.AddListener(OnClickResetProgress);
            
            _sfxSwitch.SetActive(_settingsData.SfxActive.Value(), false);
            _musicSwitch.SetActive(_settingsData.MusicActive.Value(), false);
            _highFpsSwitch.SetActive(_settingsData.HighFpsActive.Value(), false);

            _sfxSwitch.AddListener(OnSfxToggleValueChanged);
            _musicSwitch.AddListener(OnMusicToggleValueChanged);
            _highFpsSwitch.AddListener(OnLowFpsToggleValueChanged);
        }

        public void Deinitialize() {
            Main.Instance.StateManager.State.OnUpdate -= OnGameStateChange;
            
            _closePanelBtn.ForEach(b => b.onClick.RemoveAllListeners());
            _settingsBtn.onClick.RemoveAllListeners();
            _restartLvlBtn.onClick.RemoveAllListeners();
            _resetProgressBtn.onClick.RemoveAllListeners();
            _sfxSwitch.RemoveAllListeners();
            _musicSwitch.RemoveAllListeners();
            _highFpsSwitch.RemoveAllListeners();
        }
        #endregion

        #region UI
        private void StopLevel() {
            AudioController.Instance.PlayUI(AudioController.UISFX.PopClick, true);
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
            
            AudioController.Instance.PlayUI(AudioController.UISFX.PopClick, true);
        }
        
        private void OnClickResetProgress() => _resetProgressUI.gameObject.SetActive(true);
        #endregion

        #region Events
        private void OnGameStateChange(StateManager.GameState _, StateManager.GameState state) => _restartLvlBtn.gameObject.SetActive(state is StateManager.GameState.Playing);
        
        private void OnSfxToggleValueChanged(bool active) {
            _settingsData.SfxActive.Set(active);
            
            _audioMixer.SetFloat("SFX", _settingsData.SfxActive.Value() ? 0 : -80);
            if (_settingsPanel.gameObject.activeInHierarchy)
                AudioController.Instance?.PlayUI(AudioController.UISFX.Switch, true);
        }

        private void OnMusicToggleValueChanged(bool active) {
            _settingsData.MusicActive.Set(active);
            
            _audioMixer.SetFloat("Music", _settingsData.MusicActive.Value() ? 0 : -80);
            if (_settingsPanel.gameObject.activeInHierarchy)
                AudioController.Instance?.PlayUI(AudioController.UISFX.Switch, true);
        }

        private void OnLowFpsToggleValueChanged(bool active) {
            _settingsData.HighFpsActive.Set(active);
            
            Application.targetFrameRate = _settingsData.HighFpsActive.Value() ? 60 : 30;
            if (_settingsPanel.gameObject.activeInHierarchy)
                AudioController.Instance?.PlayUI(AudioController.UISFX.Switch, true);
        }
        #endregion
    }
}