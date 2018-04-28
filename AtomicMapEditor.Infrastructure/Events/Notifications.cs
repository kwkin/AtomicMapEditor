using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Events
{
    public enum Notification
    {
        MergeCurrentLayerDown, MergeCurrentLayerUp, MergeVisibleLayers, DeleteCurrentLayer, DuplicateCurrentLayer
    }
}
