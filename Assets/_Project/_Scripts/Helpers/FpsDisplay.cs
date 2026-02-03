using TMPro;
using UnityEngine;

namespace Helpers {
    public class FpsDisplay : MonoBehaviour {

        #region SerializedFields
        [SerializeField] private TMP_Text _fpsLabel;
        #endregion

        #region PrivateFields
        private float _refreshTime = 1f;
        private float _lastFramerate;
        private int _frameCounter;
        private float _timeCounter;
        #endregion
        
        #region MonoBehaviour
        private void Awake() => gameObject.SetActive(Debug.isDebugBuild || Application.isEditor);

        private void Update() => DisplayFps();
        #endregion
        
        #region FPS
        private void DisplayFps() {
            if (_timeCounter < _refreshTime) {
                _timeCounter += Time.deltaTime;
                _frameCounter++;
            }
            else {
                _lastFramerate = _frameCounter / _timeCounter;
                _fpsLabel.SetText($"FPS: {_lastFramerate:0}");
                _frameCounter = 0;
                _timeCounter = 0.0f;
            }
        }
        #endregion
    }
}