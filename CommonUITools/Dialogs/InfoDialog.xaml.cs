namespace CommonUITools.Controls;

public partial class InfoDialog : BaseDialog {
    public InfoDialog(string title = "提示", string detailText = "") {
        Title = title;
        DetailText = detailText;
        InitializeComponent();
    }
}
