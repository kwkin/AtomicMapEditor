using System;
using Ame.Infrastructure.Models;
using Ame.Modules.Windows.WindowInteractions;
using Microsoft.Practices.Unity;

namespace Ame.Modules.Windows.WindowInteractionFactories
{
    public class EditLayerFactory : IWindowInteractionFactory
    {
        #region fields
        
        private IUnityContainer container;

        #endregion fields


        #region constructors

        public EditLayerFactory(AmeSession session, ILayer layer)
        {
            if (layer == null)
            {
                throw new ArgumentNullException("container");
            }
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }
            this.container = new UnityContainer();
            this.container.RegisterInstance<ILayer>(layer);
        }

        #endregion constructors


        #region properties

        #endregion properties


        #region methods

        public IWindowInteraction CreateWindowInteraction()
        {
            return container.Resolve(typeof(EditLayerInteraction)) as IWindowInteraction;
        }

        public bool AppliesTo(Type type)
        {
            return typeof(EditLayerInteraction).Equals(type);
        }

        #endregion methods
    }
}
