using System;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Microsoft.Practices.Unity;

namespace Ame.Modules.Windows.LayerEditorWindow
{
    public class EditLayerInteractionCreator : IWindowInteractionCreator
    {
        #region fields

        #endregion fields


        #region constructors

        public EditLayerInteractionCreator(AmeSession session, ILayer layer)
        {
            if (layer == null)
            {
                throw new ArgumentNullException("layer is null");
            }
            if (session == null)
            {
                throw new ArgumentNullException("session is null");
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
            return this.Container.Resolve(typeof(EditLayerInteraction)) as IWindowInteraction;
        }

        public bool AppliesTo(Type type)
        {
            return typeof(EditLayerInteraction).Equals(type);
        }

        #endregion methods
    }
}
