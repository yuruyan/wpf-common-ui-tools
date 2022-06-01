using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CommonTools.View {
    public partial class WarningDialog : BaseDialog {

        public WarningDialog(string title = "警告", string detailText = "") : base(title, detailText) {
            InitializeComponent();
        }
    }
}
