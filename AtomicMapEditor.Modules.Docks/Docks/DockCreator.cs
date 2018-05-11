using System;
using System.Linq;
using Ame.Infrastructure.BaseTypes;
using Microsoft.Practices.Unity;

namespace Ame.Modules.Windows.Docks
{
    public class DockCreator
    {
        #region fields

        private readonly IDockCreator[] dockCreators;

        #endregion fields


        #region constructors

        public DockCreator(IDockCreator[] windowInteractionFactories)
        {
            this.dockCreators = windowInteractionFactories;
        }

        #endregion constructors


        #region properties

        #endregion properties


        #region methods

        public DockViewModelTemplate CreateDock(Type type)
        {
            var dockCreator = this.dockCreators.FirstOrDefault(factory => factory.AppliesTo(type));
            if (dockCreator == null)
            {
                throw new Exception(string.Format("{0} type is not registered", type));
            }
            return dockCreator.CreateDock();
        }

        public void UpdateContainer(Type type, IUnityContainer container)
        {
            var dockCreator = this.dockCreators.FirstOrDefault(factory => factory.AppliesTo(type));
            if (dockCreator == null)
            {
                throw new Exception(string.Format("{0} type is not registered", type));
            }
            dockCreator.Container = container;
        }

        #endregion methods
    }
}
