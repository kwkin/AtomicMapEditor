using System;
using Ame.Infrastructure.Models;
using Ame.Modules.Windows.WindowInteractions;
using Microsoft.Practices.Unity;

namespace Ame.Modules.Windows.WindowInteractionFactories
{
    public class TilesetEditorFactory : IWindowInteractionFactory
    {
        #region fields

        private IUnityContainer container;

        #endregion fields


        #region constructors

        public TilesetEditorFactory()
        {
            this.container = new UnityContainer();
        }

        #endregion constructors


        #region properties

        #endregion properties


        #region methods

        public IWindowInteraction CreateWindowInteraction()
        {
            return container.Resolve(typeof(TilesetInteraction)) as IWindowInteraction;
        }

        public bool AppliesTo(Type type)
        {
            return typeof(TilesetInteraction).Equals(type);
        }

        #endregion methods
    }
}
