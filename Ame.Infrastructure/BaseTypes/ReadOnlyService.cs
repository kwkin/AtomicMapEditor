using System.Windows;

namespace Ame.Infrastructure.BaseTypes
{
    public class ReadOnlyService : DependencyObject
    {
        #region IsReadOnly
        
        private static readonly DependencyProperty BehaviorProperty =
            DependencyProperty.RegisterAttached("IsReadOnly", typeof(bool), typeof(ReadOnlyService),
                new FrameworkPropertyMetadata(false));
        
        public static bool GetIsReadOnly(DependencyObject d)
        {
            return (bool)d.GetValue(BehaviorProperty);
        }
        
        public static void SetIsReadOnly(DependencyObject d, bool value)
        {
            d.SetValue(BehaviorProperty, value);
        }

        #endregion IsReadOnly
    }
}
