using System;
using System.Reflection;
using Ame.Infrastructure.BaseTypes;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.Interactions
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
