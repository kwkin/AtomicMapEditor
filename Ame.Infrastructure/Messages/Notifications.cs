using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Messages
{
    public enum LayerNotification
    {
        MergeCurrentLayerDown,
        MergeCurrentLayerUp,
        MergeVisibleLayers,
        DeleteCurrentLayer,
        DuplicateCurrentLayer,
        NewLayerGroup,
    }

    public enum ViewNotification
    {
        ZoomInDocument,
        ZoomOutDocument,
    }

    public enum StateNotification
    {
        ExportAs
    }
}
