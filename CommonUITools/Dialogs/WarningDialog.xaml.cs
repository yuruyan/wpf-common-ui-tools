namespace CommonUITools.Controls;

public partial class WarningDialog : BaseDialog {
    /// <summary>
    /// 全局 WarningDialog
    /// </summary>
    public static readonly WarningDialog Shared = new();

    public WarningDialog(string title = "警告", string detailText = "") {
        Title = title;
        DetailText = detailText;
        InitializeComponent();
    }
}