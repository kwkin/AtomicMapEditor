using System;
using Ame.Infrastructure.Models;
using Ame.Modules.Windows.WindowInteractions;
using Microsoft.Practices.Unity;

namespace Ame.Modules.Windows.WindowInteractionFactories
{
    public class EditLayerFactory : IWindowInteractionFactory
    {
        #region fields

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
            this.Container = new UnityContainer();
            this.Container.RegisterInstance<ILayer>(layer);
        }

        #endregion constructors


        #region properties

        public IUnityContainer Container { get; set; }

        #endregion properties


        #region methods

        public IWindowInteraction CreateWindowInteraction()
        {
            return Container.Resolve(typeof(EditLayerInteraction)) as IWindowInteraction;
        }

        public bool AppliesTo(Type type)
        {
            return typeof(EditLayerInteraction).Equals(type);
        }

        #endregion methods
    }
}
