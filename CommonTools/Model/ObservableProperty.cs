namespace CommonTools.Model;

public class ObservableProperty<T> {
    public delegate void ValueChangedHandler(T oldVal, T newVal);
    public delegate void ValueSetHandler(T oldVal, T newVal);
    /// <summary>
    /// 值改变时触发
    /// </summary>
    public event ValueChangedHandler? ValueChanged;
    /// <summary>
    /// 设置时触发
    /// </summary>
    public event ValueSetHandler? ValueSet;

    private T _value = default!;
    public T Value {
        get { return _value; }
        set {
            ValueSet?.Invoke(_value, value);
            if (object.Equals(_value, value)) {
                return;
            }
            T t = _value;
            _value = value;
            ValueChanged?.Invoke(t, value);
        }
    }

    public ObservableProperty() : this(default!) { }

    public ObservableProperty(T value) {
        Value = value;
    }

    public override string ToString() {
        return $"{Value}";
    }
}
