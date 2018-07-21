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
        void RaiseNotification(DependencyObject parent);

        void RaiseNotification(DependencyObject parent, Action<INotification> callback);

        void RaiseNotification(DependencyObject parent, string title);

        void RaiseNotification(DependencyObject parent, Action<INotification> callback, string title);

        #endregion methods
    }
}
