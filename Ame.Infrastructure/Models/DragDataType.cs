using Ame.Infrastructure.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Models
{
    public enum DragDataType
    {
        [SerializableName("ExplorerProjectNode")]
        ExplorerProjectNode,

        [SerializableName("ExplorerMapNode")]
        ExplorerMapNode,

        [SerializableName("ExplorerLayerNode")]
        ExplorerLayerNode,

        [SerializableName("LayerListNode")]
        LayerListNode
    }
}
