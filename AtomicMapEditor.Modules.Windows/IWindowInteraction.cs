using System;
using System.Windows;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows
{
    public interface IWindowInteraction
    {
        #region properties

        #endregion properties


        #region methods

        void RaiseNotification(DependencyObject test, Action<INotification> callback);

        void OnWindowClosed(INotification notification);

        #endregion methods
    }
}
