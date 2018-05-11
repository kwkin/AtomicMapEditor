using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;

namespace Ame.Modules.Windows.Interactions.PreferencesInteraction
{
    public class PreferencesViewModel : BindableBase, IInteractionRequestAware
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
