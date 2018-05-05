using System;
using Microsoft.Practices.Unity;

namespace Ame.Modules.Windows.TilesetEditorWindow
{
    public class TilesetEditorInteractionCreator : IWindowInteractionCreator
    {
        #region fields

        #endregion fields


        #region constructors

        public TilesetEditorInteractionCreator()
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
            return Container.Resolve(typeof(TilesetEditorInteraction)) as IWindowInteraction;
        }

        public bool AppliesTo(Type type)
        {
            return typeof(TilesetEditorInteraction).Equals(type);
        }

        #endregion methods
    }
}
