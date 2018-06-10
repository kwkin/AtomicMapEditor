using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ame.Infrastructure.Converters;

namespace Ame.Infrastructure.Models
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum LayerPosition
    {
        [Description("Base")]
        Base,

        [Description("Front")]
        Front,

        [Description("Back")]
        Back
    }
}
