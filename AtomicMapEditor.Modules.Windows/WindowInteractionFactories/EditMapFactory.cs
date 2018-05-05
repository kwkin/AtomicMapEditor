using System;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Ame.Modules.Windows.WindowInteractions;
using Microsoft.Practices.Unity;

namespace Ame.Modules.Windows.WindowInteractionFactories
{
    public class EditMapFactory : IWindowInteractionFactory
    {
        #region fields

        #endregion fields


        #region constructors

        public EditMapFactory(AmeSession session, DockViewModelTemplate activeDocument)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }
            this.Container = new UnityContainer();
            this.Container.RegisterInstance<AmeSession>(session);
            this.Container.RegisterInstance<DockViewModelTemplate>(activeDocument);
        }

        #endregion constructors


        #region properties

        public IUnityContainer Container { get; set; }

        #endregion properties


        #region methods

        public IWindowInteraction CreateWindowInteraction()
        {
            return Container.Resolve(typeof(EditMapInteraction)) as IWindowInteraction;
        }

        public bool AppliesTo(Type type)
        {
            return typeof(EditMapInteraction).Equals(type);
        }

        #endregion methods
    }
}
