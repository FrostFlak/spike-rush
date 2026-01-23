using System;

namespace Helpers {
    public class OwnedObservable<TOwner, T> {
        
        private readonly TOwner _owner;
        private T _value;
        
        public event Action<TOwner, T, T> OnUpdate;

        public OwnedObservable(TOwner owner, T value) {
            _owner = owner;
            _value = value;
        }
        
        public T Value() {
            return _value;
        }
        
        public T Set(T newVal, bool withoutNotify = false) {
            var oldVal = _value;
            if (oldVal.Equals(newVal)) { return oldVal; }

            lock (_value) {
                _value = newVal;
            }

            if (!withoutNotify)
                OnUpdate?.Invoke(_owner, oldVal, _value);

            return _value;
        }
    }
}