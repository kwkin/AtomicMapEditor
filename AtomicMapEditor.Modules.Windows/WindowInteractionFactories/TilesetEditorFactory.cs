using System;
using Ame.Infrastructure.Models;
using Ame.Modules.Windows.WindowInteractions;
using Microsoft.Practices.Unity;

namespace Ame.Modules.Windows.WindowInteractionFactories
{
    public class TilesetEditorFactory : IWindowInteractionFactory
    {
        #region fields

        #endregion fields


        #region constructors

        public TilesetEditorFactory()
        {
            this.Container = new UnityContainer();
        }

        #endregion constructors


        #region properties

        public IUnityContainer Container { get; set; }

        #endregion properties


        #region methods

        public IWindowInteraction CreateWindowInteraction()
        {
            return Container.Resolve(typeof(TilesetInteraction)) as IWindowInteraction;
        }

        public bool AppliesTo(Type type)
        {
            return typeof(TilesetInteraction).Equals(type);
        }

        #endregion methods
    }
}
