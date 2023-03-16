using ModernWpf.Media.Animation;

namespace CommonUITools.Utils;

internal static class ConstantCollections {
    public static readonly IReadOnlyDictionary<NavigationTransitionEffect, NavigationTransitionInfo> NavigationTransitionEffectDict = new Dictionary<NavigationTransitionEffect, NavigationTransitionInfo>() {
        {NavigationTransitionEffect.DrillIn, new DrillInNavigationTransitionInfo()},
        {NavigationTransitionEffect.Entrance, new EntranceNavigationTransitionInfo()},
        {NavigationTransitionEffect.Suppress, new SuppressNavigationTransitionInfo()},
        {NavigationTransitionEffect.Slide, new SlideNavigationTransitionInfo()},
    };

}
