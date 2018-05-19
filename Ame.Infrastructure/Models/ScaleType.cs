using System.ComponentModel;
using Ame.Infrastructure.Converters;

namespace Ame.Infrastructure.Models
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum ScaleType
    {
        [Description("px")]
        Pixel,

        [Description("tile")]
        Tile
    }
}
