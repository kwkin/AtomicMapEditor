using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.BaseTypes
{
    public static class ReadOnlyBindableProperty
    {
        public static ReadOnlyBindableProperty<T> Prepare<T>(T intial, [CallerMemberName] string propertyName = "")
        {
            return new ReadOnlyBindableProperty<T>(intial, propertyName);
        }

        public static ReadOnlyBindableProperty<T> Prepare<T>([CallerMemberName] string propertyName = "")
        {
            return new ReadOnlyBindableProperty<T>(default(T), propertyName);
        }
    }

    public class ReadOnlyBindableProperty<T> : BindableBase, PropertyValue
    {
        #region fields


        #endregion fields


        #region constructor

        public ReadOnlyBindableProperty(T value, [CallerMemberName] string propertyName = "")
        {
            this.value = value;
            this.Name = propertyName;
        }

        public ReadOnlyBindableProperty([CallerMemberName] string propertyName = "")
        {
            this.value = default(T);
            this.Name = propertyName;
        }

        #endregion constructor


        #region properties

        private T value;
        public T Value
        {
            get
            {
                return this.value;
            }
            private set
            {
                this.SetProperty(ref this.value, value);
            }
        }

        public string Name { get; protected set; }

        #endregion properties


        #region methods

        public object GetValue()
        {
            return this.Value;
        }

        public void UpdateValue(object sender, PropertyChangedEventArgs e)
        {
            BindableProperty<T> property = (BindableProperty<T>)sender;
            this.Value = property.Value;
        }

        #endregion methods
    }
}
