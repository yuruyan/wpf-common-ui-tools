using ModernWpf.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CommonUITools.View {
    public class BaseDialog : ContentDialog {
        /// <summary>
        /// DetailText
        /// </summary>
        public string DetailText {
            get { return (string)GetValue(DetailTextProperty); }
            set { SetValue(DetailTextProperty, value); }
        }
        public static readonly DependencyProperty DetailTextProperty = DependencyProperty.Register("DetailText", typeof(string), typeof(BaseDialog), new PropertyMetadata(""));

        public BaseDialog(
            string title = "",
            string detailText = "",
            string okButtonText = "确定",
            string cancelButtonText = "取消"
        ) {
            if (!string.IsNullOrEmpty(title)) {
                Title = title;
            }
            if (!string.IsNullOrEmpty(detailText)) {
                DetailText = detailText;
            }
            if (!string.IsNullOrEmpty(okButtonText)) {
                PrimaryButtonText = okButtonText;
            }
            if (!string.IsNullOrEmpty(cancelButtonText)) {
                CloseButtonText = cancelButtonText;
            }
            init();
            // 设置 ScaleAnimation
            Opened += (o, e) => {
                Utils.ScaleAnimationHelper.SetIsEnabled((DependencyObject)this.Content, true);
                Utils.ScaleAnimationHelper.SetScaleOption((DependencyObject)this.Content, Utils.ScaleAnimationHelper.ScaleOption.Center);
            };
        }

        public BaseDialog() : this("") {
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void init() {
            DataContext = this;

            DefaultButton = ContentDialogButton.Primary;
            #region 设置 TitleTemplate
            DataTemplate dataTemplate = new DataTemplate();
            #region SimpleStackPanel
            FrameworkElementFactory simpleStackPanel = new FrameworkElementFactory(typeof(SimpleStackPanel));
            simpleStackPanel.SetValue(SimpleStackPanel.OrientationProperty, Orientation.Horizontal);
            simpleStackPanel.SetValue(SimpleStackPanel.SpacingProperty, 8.0);
            #region TextBlock
            FrameworkElementFactory textblock = new FrameworkElementFactory(typeof(TextBlock));
            textblock.SetValue(TextBlock.StyleProperty, Application.Current.Resources["ContentDialogTitleStyle"]);
            textblock.SetBinding(TextBlock.TextProperty, new Binding("Title") {
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(BaseDialog), 1)
            });
            #endregion
            simpleStackPanel.AppendChild(textblock);
            #endregion
            dataTemplate.VisualTree = simpleStackPanel;
            TitleTemplate = dataTemplate;
            #endregion
        }
    }

    public class DialogResult<T> {
        public ContentDialogResult Result { get; set; }
        public T? Data { get; set; } = default;

        public override string ToString() {
            return $"{nameof(Result)}: {Result}; {nameof(Data)}: {Data}";
        }
    }
}
