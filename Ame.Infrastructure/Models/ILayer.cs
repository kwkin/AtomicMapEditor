using Ame.Infrastructure.BaseTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Ame.Infrastructure.Models
{
    public interface ILayer
    {
        #region fields

        #endregion fields


        #region properties

        BindableProperty<string> Name { get; set; }
        BindableProperty<bool> IsImmutable { get; set; }
        BindableProperty<bool> IsVisible { get; set; }
        BindableProperty<int> Columns { get; set; }
        BindableProperty<int> Rows { get; set; }
        BindableProperty<int> OffsetX { get; set; }
        BindableProperty<int> OffsetY { get; set; }
        ReadOnlyBindableProperty<int> PixelWidth { get; }
        ReadOnlyBindableProperty<int> PixelHeight { get; }
        DrawingGroup Group { get; set; }
        LayerGroup Parent { get; set; }

        #endregion properties


        #region methods

        // TODO change to a property
        int GetPixelWidth();

        int GetPixelHeight();

        void AddWith(ILayer layer);

        #endregion methods
    }
}
