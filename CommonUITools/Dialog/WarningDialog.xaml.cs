namespace CommonUITools.View;

public partial class WarningDialog : BaseDialog {
    public WarningDialog(string title = "警告", string detailText = "") : base(title, detailText) {
        InitializeComponent();
    }

    /// <summary>
    /// 全局 WarningDialog
    /// </summary>
    public static readonly WarningDialog Shared = new();
}