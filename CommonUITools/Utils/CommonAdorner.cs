using System.Windows.Documents;

namespace CommonUITools.Utils;

public class CommonAdorner : Adorner {
    public readonly struct ElementInfo {
        public readonly FrameworkElement Element;
        /// <summary>
        /// 绑定对象，为 null 则绑定到 adornedElement
        /// </summary>
        public readonly FrameworkElement? TargetBindingElement;
        /// <summary>
        /// 是否绑定 Width
        /// </summary>
        public readonly bool BindWidth;
        /// <summary>
        /// 是否绑定 Height
        /// </summary>
        public readonly bool BindHeight;

        public ElementInfo(
            FrameworkElement element,
            bool bindWidth = true,
            bool bindHeight = true,
            FrameworkElement? targetBindingElement = null
        ) {
            Element = element;
            BindWidth = bindWidth;
            BindHeight = bindHeight;
            TargetBindingElement = targetBindingElement;
        }
    }

    private readonly VisualCollection VisualChildren;
    /// <summary>
    /// UIElement 集合
    /// </summary>
    private readonly IEnumerable<UIElement> Elements;

    protected override int VisualChildrenCount => VisualChildren.Count;

    public CommonAdorner(FrameworkElement adornedElement, IEnumerable<ElementInfo> elementInfos) : base(adornedElement) {
        VisualChildren = new(this);
        Elements = elementInfos.Select(e => e.Element);
        // 绑定 size
        foreach (var item in elementInfos) {
            UIUtils.BindSize(
                item.Element,
                item.TargetBindingElement ?? adornedElement,
                item.BindWidth,
                item.BindHeight
            );
            VisualChildren.Add(item.Element);
        }
    }

    protected override Visual GetVisualChild(int index) => VisualChildren[index];

    protected override Size ArrangeOverride(Size finalSize) {
        foreach (var item in Elements) {
            item.Arrange(new(finalSize));
        }
        return base.ArrangeOverride(finalSize);
    }
}