using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.BaseTypes
{
    public static class BindableProperty
    {
        public static BindableProperty<T> Prepare<T>([CallerMemberName] string propertyName = "")
        {
            return new BindableProperty<T>(default(T), propertyName);
        }

        public static BindableProperty<T> Prepare<T>(T intial, [CallerMemberName] string propertyName = "")
        {
            return new BindableProperty<T>(intial, propertyName);
        }
    }

    public interface PropertyValue
    {
        object GetValue();
    }

    public class BindableProperty<T> : BindableBase, PropertyValue
    {
        #region fields

        private T value;

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
        
        public T Value
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

        public string Name { get; private set; }

        #endregion properties


        #region methods

        public object GetValue()
        {
            return this.Value;
        }

        public static BindableProperty<T> Prepare(T value, [CallerMemberName] string propertyName = "")
        {
            return new BindableProperty<T>(value, propertyName);
        }

        public static BindableProperty<T> Prepare([CallerMemberName] string propertyName = "")
        {
            return new BindableProperty<T>(default(T), propertyName);
        }

        #endregion methods
    }
}
