using System.Windows;

namespace Ame.Components.Behaviors
{
    public class ReadOnlyService : DependencyObject
    {
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.RegisterAttached(
                "IsReadOnly",
                typeof(bool),
                typeof(ReadOnlyService),
                new FrameworkPropertyMetadata(false));

        public static void SetIsReadOnly(UIElement element, bool value)
        {
            element.SetValue(IsReadOnlyProperty, value);
        }
        public static bool GetIsReadOnly(UIElement element)
        {
            return (bool)element.GetValue(IsReadOnlyProperty);
        }
    }
}
