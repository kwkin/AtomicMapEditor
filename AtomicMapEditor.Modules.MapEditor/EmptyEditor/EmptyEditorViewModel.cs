using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;

namespace Ame.Modules.MapEditor.EmptyEditor
{
    [DockContentId("EmptyEditor")]
    public class EmptyEditorViewModel : EditorViewModelTemplate
    {
        #region fields

        #endregion fields


        #region constructor

        #endregion constructor


        #region properties

        #endregion properties


        #region methods

        public override object GetContent()
        {
            throw new NotImplementedException();
        }

        public override void SetZoom(ZoomLevel zoomLevel)
        {
            throw new NotImplementedException();
        }

        public override void SetZoom(int zoomIndex)
        {
            throw new NotImplementedException();
        }

        public override void ZoomIn()
        {
            throw new NotImplementedException();
        }

        public override void ZoomOut()
        {
            throw new NotImplementedException();
        }

        #endregion methods
    }
}
