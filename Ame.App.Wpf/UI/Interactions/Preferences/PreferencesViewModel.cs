using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.App.Wpf.UI.Interactions.Preferences
{
    public class PreferencesViewModel : IInteractionRequestAware
    {
        #region fields

        #endregion fields


        #region constructor

        #endregion constructor


        #region properties

        public Action FinishInteraction { get; set; }
        public INotification Notification { get; set; }

        #endregion properties


        #region methods

        #endregion methods
    }
}
