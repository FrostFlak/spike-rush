using Helpers;

namespace Models {
    public class Level {
        public int ID;
        public OwnedObservable<Level, bool> IsReached;
        public Observable<int> RecordDistance;
    }
}