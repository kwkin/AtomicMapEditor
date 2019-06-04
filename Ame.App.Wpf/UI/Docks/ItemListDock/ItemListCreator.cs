using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.BaseTypes;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.App.Wpf.UI.Docks.ItemListDock
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
