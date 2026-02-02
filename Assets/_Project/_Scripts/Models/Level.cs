using Helpers;

namespace Models {
    public class Level {
        public int ID;
        public Observable<bool> IsReached;
        public Observable<int> RecordDistance;
    }
}