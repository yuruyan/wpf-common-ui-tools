namespace CommonUITools.Controls;

public class InplaceTextBoxArgs : RoutedEventArgs {
    public string Text { get; set; } = string.Empty;

    public InplaceTextBoxArgs() {

    }

    public InplaceTextBoxArgs(RoutedEvent routedEvent) : base(routedEvent) {

    }

    public InplaceTextBoxArgs(RoutedEvent routedEvent, object source) : base(routedEvent, source) {

    }

    public InplaceTextBoxArgs(RoutedEvent routedEvent, object source, string text) : base(routedEvent, source) {
        Text = text;
    }

    public InplaceTextBoxArgs(string text) {
        Text = text;
    }
}

public partial class InplaceTextBox : UserControl {
    private static readonly DependencyProperty IsTextBoxVisibleProperty = DependencyProperty.Register("IsTextBoxVisible", typeof(bool), typeof(InplaceTextBox), new PropertyMetadata(false));
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(InplaceTextBox), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, TextPropertyChangedHandler));
    public static readonly DependencyProperty TextBlockStyleProperty = DependencyProperty.Register("TextBlockStyle", typeof(Style), typeof(InplaceTextBox), new PropertyMetadata());
    public static readonly DependencyProperty TextBoxStyleProperty = DependencyProperty.Register("TextBoxStyle", typeof(Style), typeof(InplaceTextBox), new PropertyMetadata());
    public static readonly RoutedEvent TextChangedEvent = EventManager.RegisterRoutedEvent(nameof(TextChanged), RoutingStrategy.Direct, typeof(TextChangedEventHandler), typeof(InplaceTextBox));
    public delegate void TextChangedEventHandler(object sender, InplaceTextBoxArgs e);
    public event TextChangedEventHandler TextChanged {
        add { this.AddHandler(TextChangedEvent, value); }
        remove { this.RemoveHandler(TextChangedEvent, value); }
    }
    private bool IsMouseClick;

    /// <summary>
    /// 编辑框是否可见
    /// </summary>
    private bool IsTextBoxVisible {
        get { return (bool)GetValue(IsTextBoxVisibleProperty); }
        set { SetValue(IsTextBoxVisibleProperty, value); }
    }
    /// <summary>
    /// 文本
    /// </summary>
    public string Text {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }
    public Style TextBlockStyle {
        get { return (Style)GetValue(TextBlockStyleProperty); }
        set { SetValue(TextBlockStyleProperty, value); }
    }
    public Style TextBoxStyle {
        get { return (Style)GetValue(TextBoxStyleProperty); }
        set { SetValue(TextBoxStyleProperty, value); }
    }

    public InplaceTextBox() {
        InitializeComponent();
        PreviewMouseDown += (_, _) => IsMouseClick = true;
        // Set auto changed focus
        this.SetLoadedOnceEventHandler(static (obj, _) => {
            if (obj is not InplaceTextBox self) {
                return;
            }
            self.TextBlockStyle ??= (Style)self.FindResource("GlobalTextBlockStyle");
            self.TextBoxStyle ??= (Style)self.FindResource("GlobalTextBoxStyle");
            Window.GetWindow(self).PreviewMouseUp += (sender, e) => {
                if (!self.IsMouseClick) {
                    self.IsTextBoxVisible = false;
                }
                self.IsMouseClick = false;
            };
        });
    }

    private static void TextPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is InplaceTextBox self) {
            self.RaiseEvent(new InplaceTextBoxArgs(
                InplaceTextBox.TextChangedEvent,
                self,
                (string)e.NewValue)
            );
        }
    }

    /// <summary>
    /// Submit
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void UpdateTextHandler(object sender, KeyEventArgs e) {
        if (e.Key == Key.Enter) {
            IsTextBoxVisible = false;
        }
    }

    /// <summary>
    /// Submit
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TextBoxLostFocusHandler(object sender, RoutedEventArgs e) {
        IsTextBoxVisible = false;
    }

    /// <summary>
    /// Show textbox
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ShowTextBoxHandler(object sender, MouseButtonEventArgs e) {
        IsTextBoxVisible = true;
    }

    /// <summary>
    /// Focus TextBox
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TextBoxIsVisibleChangedHandler(object sender, DependencyPropertyChangedEventArgs e) {
        if (sender is TextBox box && e.NewValue is true) {
            box.Focus();
        }
    }
}
