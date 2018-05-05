using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Prism.Interactivity;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows
{
    public interface IWindowInteraction
    {
        #region properties

        #endregion


        #region methods
        
        void RaiseNotification(DependencyObject test, Action<INotification> callback);
        void OnWindowClosed(INotification notification);

        #endregion
    }
}
