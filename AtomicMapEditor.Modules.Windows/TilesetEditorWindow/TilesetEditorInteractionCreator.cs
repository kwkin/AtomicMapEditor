using System;
using Ame.Infrastructure.BaseTypes;
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
            return Container.Resolve<TilesetEditorInteraction>();
        }

        public bool AppliesTo(Type type)
        {
            return typeof(TilesetEditorInteraction).Equals(type);
        }

        #endregion methods
    }
}
