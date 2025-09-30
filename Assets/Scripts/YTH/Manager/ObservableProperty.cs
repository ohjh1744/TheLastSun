using UnityEngine;
using UnityEngine.Events;

namespace DesignPattern
{
    [System.Serializable]
    public class ObservableProperty<T>
    {
        [SerializeField] private T _value;
        public T Value
        {
            get => _value;
            set
            {
                if (_value.Equals(value)) return;
                _value = value;
                Notify();
            }
        }
        public UnityEvent<T> OnValueChanged = new();

        public ObservableProperty(T value = default)
        {
            _value = value;
        }

        public void Subscribe(UnityAction<T> action)
        {
            OnValueChanged.AddListener(action);
        }

        public void Unsubscribe(UnityAction<T> action)
        {
            OnValueChanged.RemoveListener(action);
        }

        public void UnsbscribeAll()
        {
            OnValueChanged.RemoveAllListeners();
        }

        private void Notify()
        {
            OnValueChanged?.Invoke(Value);
        }
    }
}
