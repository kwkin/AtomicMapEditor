using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Ame.Infrastructure.Models
{
    public interface ILayer : INotifyPropertyChanged
    {
        #region fields

        #endregion fields
          
        
        #region properties

        string Name { get; set; }
        bool IsImmutable { get; set; }
        bool IsVisible { get; set; }
        DrawingGroup Group { get; set; }
        LayerGroup Parent { get; set; }

        #endregion properties


        #region methods

        int GetPixelWidth();

        int GetPixelHeight();

        void AddWith(ILayer layer);

        #endregion methods
    }
}
