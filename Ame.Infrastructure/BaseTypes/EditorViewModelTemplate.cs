using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;

namespace Ame.Infrastructure.BaseTypes
{
    public abstract class EditorViewModelTemplate : DockViewModelTemplate
    {
        #region fields

        #endregion fields


        #region constructor

        #endregion constructor


        #region properties

        #endregion properties


        #region methods

        public abstract void ZoomIn();
        public abstract void ZoomOut();
        public abstract void SetZoom(int zoomIndex);
        public abstract void SetZoom(ZoomLevel zoomLevel);
        public abstract object GetContent();
        public abstract void ExportAs(string path, BitmapEncoder encoder);

        #endregion methods
    }
}
