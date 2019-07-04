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
    public static class BindableProperty
    {
        public static BindableProperty<T> Prepare<T>(T intial, [CallerMemberName] string propertyName = "")
        {
            return new BindableProperty<T>(intial, propertyName);
        }

        public static BindableProperty<T> Prepare<T>([CallerMemberName] string propertyName = "")
        {
            return new BindableProperty<T>(default(T), propertyName);
        }
    }

    public interface PropertyValue
    {
        object GetValue();
    }

    public class BindableProperty<T> : ReadOnlyBindableProperty<T>, PropertyValue
    {
        #region fields

        #endregion fields


        #region constructor

        public BindableProperty(T value, [CallerMemberName] string propertyName = "")
        {
            this.value = value;
            this.Name = propertyName;
        }

        public BindableProperty([CallerMemberName] string propertyName = "")
        {
            this.value = default(T);
            this.Name = propertyName;
        }

        #endregion constructor


        #region properties


        private T value;
        public new T Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.SetProperty(ref this.value, value);
            }
        }
        
        #endregion properties


        #region methods

        public static BindableProperty<T> Prepare(T value, [CallerMemberName] string propertyName = "")
        {
            return new BindableProperty<T>(value, propertyName);
        }

        public static BindableProperty<T> Prepare([CallerMemberName] string propertyName = "")
        {
            return new BindableProperty<T>(default(T), propertyName);
        }

        public ReadOnlyBindableProperty<T> ReadOnlyProperty()
        {
            ReadOnlyBindableProperty<T> property = ReadOnlyBindableProperty.Prepare<T>(this.value, this.Name);
            this.PropertyChanged += property.UpdateValue;
            return property;
        }

        #endregion methods
    }
}
