using System.Windows;

namespace TrimesterPlaner.Extensions
{
    public static class WpfExtensions
    {
        public static T? FindAncestor<T>(this FrameworkElement frameworkElement) where T : FrameworkElement
        {
            if (frameworkElement is T ancestor)
            {
                return ancestor;
            }

            if (frameworkElement.Parent is FrameworkElement parent)
            {
                return parent.FindAncestor<T>();
            }

            return null;
        }
    }
}