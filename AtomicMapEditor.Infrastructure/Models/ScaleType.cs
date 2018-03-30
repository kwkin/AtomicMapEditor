using System.ComponentModel;
using AtomicMapEditor.Infrastructure.Converters;

namespace AtomicMapEditor.Infrastructure.Models
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
