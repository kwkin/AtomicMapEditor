using System;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Microsoft.Practices.Unity;
using Prism.Events;

namespace Ame.Modules.Windows.Docks.ItemListDock
{
    public class ItemListCreator : DockCreatorTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;
        public AmeSession session;

        #endregion fields


        #region constructors

        public ItemListCreator(IEventAggregator eventAggregator, AmeSession session)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator is null");
            }
            this.eventAggregator = eventAggregator;
            this.session = session;
        }

        #endregion constructors


        #region properties

        #endregion properties


        #region methods

        public override DockViewModelTemplate CreateDock()
        {
            return new ItemListViewModel(this.eventAggregator, this.session);
        }

        public override bool AppliesTo(Type type)
        {
            return typeof(ItemListViewModel).Equals(type);
        }

        #endregion methods
    }
}
