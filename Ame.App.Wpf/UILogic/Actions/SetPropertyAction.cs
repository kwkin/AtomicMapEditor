using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows;
using System.Windows.Interactivity;

namespace Ame.App.Wpf.UILogic.Actions
{
    public class SetPropertyAction : TriggerAction<FrameworkElement>
    {
        public static readonly DependencyProperty PropertyNameProperty = 
            DependencyProperty.Register(
                "PropertyName", 
                typeof(string),
                typeof(SetPropertyAction),
                new PropertyMetadata(default(string)));

        public string PropertyName
        {
            get { return (string)GetValue(PropertyNameProperty); }
            set { SetValue(PropertyNameProperty, value); }
        }

        public static readonly DependencyProperty PropertyValueProperty = 
            DependencyProperty.Register("PropertyValue", 
                typeof(object),
                typeof(SetPropertyAction),
                new PropertyMetadata(default(object)));

        public object PropertyValue
        {
            get { return GetValue(PropertyValueProperty); }
            set { SetValue(PropertyValueProperty, value); }
        }

        public static readonly DependencyProperty TargetObjectProperty = 
            DependencyProperty.Register("TargetObject", 
                typeof(object),
                typeof(SetPropertyAction),
                new PropertyMetadata(default(object)));
        
        public object TargetObject
        {
            get { return GetValue(TargetObjectProperty); }
            set { SetValue(TargetObjectProperty, value); }
        }
        
        protected override void Invoke(object parameter)
        {
            object target = TargetObject ?? AssociatedObject;
            PropertyInfo propertyInfo = target.GetType().GetProperty(PropertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(target, PropertyValue);
            }
        }
    }
}
