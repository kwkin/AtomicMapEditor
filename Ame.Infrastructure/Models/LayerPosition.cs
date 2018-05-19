using System.ComponentModel;
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
