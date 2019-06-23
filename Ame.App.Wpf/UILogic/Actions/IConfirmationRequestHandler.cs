using Ame.Infrastructure.BaseTypes;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.App.Wpf.UILogic.Actions
{
    public interface IInteractionRequestHandler
    {
        BindableProperty<INotification> Notification { get; set; }

        Action FinishInteraction { get; set; }
    }
}
