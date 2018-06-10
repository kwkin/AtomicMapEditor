using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Infrastructure.BaseTypes
{
    public interface IWindowInteraction
    {
        #region properties

        #endregion properties


        #region methods

        // Calls the default notification
        void RaiseNotification(DependencyObject test);

        void RaiseNotification(DependencyObject test, Action<INotification> callback);
        
        #endregion methods
    }
}
