using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Ame.Infrastructure.Converters;

namespace Ame.Infrastructure.Models
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum ScaleType
    {
        [XmlEnum(Name = "px")]
        [Description("Px")]
        Pixel,

        [XmlEnum(Name = "tile")]
        [Description("Tile")]
        Tile
    }
}
