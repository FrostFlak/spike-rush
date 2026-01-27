using Unity.Logging;
using UnityEngine;

namespace Game {
    public class LevelManager {

        public LevelData ActiveLevel { get; private set; }

        public LevelManager() {
            Main.Instance.StateManager.State.OnUpdate += OnGameStateChange;
            SpawnLevel();
        }
        
        public void Deinitialize() {
            Main.Instance.StateManager.State.OnUpdate -= OnGameStateChange;
        }
        
        private void SpawnLevel() {
            if (ActiveLevel != null)
                Object.Destroy(ActiveLevel.gameObject);
            
            var levelPb = Main.Instance.PredifinedLevelsPb[Main.Instance.CurrentLevelID];
            
            ActiveLevel = Object.Instantiate(levelPb, Vector3.zero, Quaternion.identity);
            Main.Instance.Shredder.ReturnToStart(ActiveLevel.StartTransform.position);
            Log.Debug($"Spawned level: [{Main.Instance.CurrentLevelID}]");
        }
        
        private void OnGameStateChange(StateManager.GameState oldState, StateManager.GameState state) {
            if (oldState is StateManager.GameState.Win && state is StateManager.GameState.GatheredReward) {
                Main.Instance.LevelsData[Main.Instance.CurrentLevelID].IsReached.Set(true);
                Main.Instance.CurrentLevelID++;
                SpawnLevel();
            }
            else if (state is StateManager.GameState.GatheredReward) {
                Main.Instance.Shredder.ReturnToStart(ActiveLevel.StartTransform.position);
                SpawnLevel();
            }
        }
    }
}