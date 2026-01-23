using Helpers;

namespace Game {
    public class StateManager {
        
        public Observable<GameState> State { get; } = new(GameState.None);

        public enum GameState {
            None,
            Playing,
            Win,
            Lose,
            GatheredReward,
        }

        public void FinishLvl() {
            if (State.Value() != GameState.Playing)
                return;

            State.Set(GameState.Win);
        }

        public void FailLvl() {
            if (State.Value() != GameState.Playing) 
                return;

            State.Set(GameState.Lose);
        }
    }
}