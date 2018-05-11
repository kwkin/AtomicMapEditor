using System;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Microsoft.Practices.Unity;
using Prism.Events;

namespace Ame.Modules.Windows.MapEditorWindow
{
    public class NewMapInteractionCreator : IWindowInteractionCreator
    {
        #region fields

        #endregion fields


        #region constructors

        public NewMapInteractionCreator(AmeSession session, IEventAggregator eventAggregator)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session is null");
            }
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator is null");
            }
            this.Container = new UnityContainer();
            this.Container.RegisterInstance<AmeSession>(session);
            this.Container.RegisterInstance<IEventAggregator>(eventAggregator);
        }

        #endregion constructors


        #region properties

        public IUnityContainer Container { get; set; }

        #endregion properties


        #region methods

        public IWindowInteraction CreateWindowInteraction()
        {
            return this.Container.Resolve<NewMapInteraction>();
        }

        public bool AppliesTo(Type type)
        {
            return typeof(NewMapInteraction).Equals(type);
        }

        #endregion methods
    }
}
