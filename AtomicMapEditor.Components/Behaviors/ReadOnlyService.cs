using System.Windows;

namespace Ame.Components.Behaviors
{
    public class ReadOnlyService : DependencyObject
    {
        private static readonly DependencyProperty BehaviorProperty =
            DependencyProperty.RegisterAttached(
                "IsReadOnly",
                typeof(bool),
                typeof(ReadOnlyService),
                new FrameworkPropertyMetadata(false));

        public static bool GetIsReadOnly(DependencyObject d)
        {
            return (bool)d.GetValue(BehaviorProperty);
        }

        public static void SetIsReadOnly(DependencyObject d, bool value)
        {
            d.SetValue(BehaviorProperty, value);
        }
    }
}
