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
    public enum LayerPosition
    {
        [XmlEnum(Name = "base")]
        [Description("Base")]
        Base,

        [XmlEnum(Name = "front")]
        [Description("Front")]
        Front,

        [XmlEnum(Name = "back")]
        [Description("Back")]
        Back
    }
}
