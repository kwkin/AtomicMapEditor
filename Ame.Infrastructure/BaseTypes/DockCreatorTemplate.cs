using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ame.Infrastructure.BaseTypes;

namespace Ame.Modules.Windows
{
    public abstract class DockCreatorTemplate
    {
        #region properties

        #endregion properties


        #region methods

        public abstract DockViewModelTemplate CreateDock();

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
