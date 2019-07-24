using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Ame.Infrastructure.Models;
using Prism.Events;
using Prism.Interactivity;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Infrastructure.BaseTypes
{
    public interface IWindowInteraction
    {
        #region properties

        string Title { get; set; }

        Action<INotification> Callback { get; set; }

        IEventAggregator EventAggregator { get; set; }

        double Width { get; set; }

        double Height { get; set; }

        #endregion properties


        #region methods

        void UpdateMissingContent(IAmeSession session);

        void RaiseNotification(DependencyObject parent);

        #endregion methods
    }
}
