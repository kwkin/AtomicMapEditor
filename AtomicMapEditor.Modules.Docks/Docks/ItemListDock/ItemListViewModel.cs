using System;
using System.Windows.Input;
using Ame.Infrastructure.BaseTypes;
using Prism.Commands;
using Prism.Events;

namespace Ame.Modules.Windows.Docks.ItemListDock
{
    [DockContentId("ItemList")]
    public class ItemListViewModel : DockToolViewModelTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;

        #endregion fields


        #region constructor

        public ItemListViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.Title = "Item List";

            this.AddTilesetCommand = new DelegateCommand(() => AddTileset());
            this.ViewPropertiesCommand = new DelegateCommand(() => ViewProperties());
        }

        #endregion constructor


        #region properties

        public ICommand ViewPropertiesCommand { get; private set; }
        public ICommand AddTilesetCommand { get; private set; }

        #endregion properties


        #region methods

        public void AddTileset()
        {
            Console.WriteLine("Add map");
        }

        public void ViewProperties()
        {
            Console.WriteLine("View Properties ");
        }

        #endregion methods
    }
}
