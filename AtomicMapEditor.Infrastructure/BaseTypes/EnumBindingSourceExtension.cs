using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Ame.Infrastructure.BaseTypes
{
    public class EnumBindingSourceExtension : MarkupExtension
    {
        #region fields

        #endregion fields


        #region constructor & destructer

        public EnumBindingSourceExtension()
        {
        }

        public EnumBindingSourceExtension(Type enumType)
        {
            this.EnumType = enumType;
        }

        #endregion constructor & destructer


        #region properties

        private Type _EnumType;

        public Type EnumType
        {
            get { return this._EnumType; }
            set
            {
                if (value != this._EnumType)
                {
                    if (null != value)
                    {
                        Type enumType = Nullable.GetUnderlyingType(value) ?? value;
                        if (!enumType.IsEnum)
                        {
                            throw new ArgumentException("Type must be an Enum.");
                        }
                    }
                    this._EnumType = value;
                }
            }
        }

        #endregion properties


        #region methods

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (null == this._EnumType)
            {
                throw new InvalidOperationException("The EnumType must be specified.");
            }
            Type actualEnumType = Nullable.GetUnderlyingType(this._EnumType) ?? this._EnumType;
            Array enumValues = Enum.GetValues(actualEnumType);
            if (actualEnumType == this._EnumType)
            {
                return enumValues;
            }
            Array tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
            enumValues.CopyTo(tempArray, 1);
            return tempArray;
        }

        #endregion methods
    }
}
