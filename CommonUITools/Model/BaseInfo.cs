namespace CommonUITools.Model;

public class BaseInfo {
    public long Id { get; set; }
}

public class BaseInfoDO : DependencyObject {
    public static readonly DependencyProperty IdProperty = DependencyProperty.Register("Id", typeof(long), typeof(BaseInfoDO), new PropertyMetadata(0L));

    public long Id {
        get { return (long)GetValue(IdProperty); }
        set { SetValue(IdProperty, value); }
    }
}
