using Ame.Infrastructure.BaseTypes;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ame.App.Wpf.UI.Interactions
{
    public abstract class WindowInteractionCreatorTemplate
    {
        #region properties

        #endregion properties


        #region methods

        public abstract IWindowInteraction CreateWindowInteraction();
        public abstract IWindowInteraction CreateWindowInteraction(Action<INotification> callback);
        public abstract bool AppliesTo(Type type);

        public void UpdateContent(Type type, object value)
        {
            PropertyInfo[] properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo property in properties)
            {
                if (property.PropertyType == type)
                {
                    property.SetValue(this, value);
                    break;
                }
            }
        }

        #endregion methods
    }
}
