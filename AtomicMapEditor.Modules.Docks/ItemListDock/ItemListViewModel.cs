using System;
using System.Windows.Input;
using AtomicMapEditor.Infrastructure.BaseTypes;
using Prism.Commands;

namespace AtomicMapEditor.Modules.Docks.ItemListDock
{
    public class ItemListViewModel : DockViewModelTemplate
    {
        #region fields

        #endregion fields


        #region constructor & destructer

        public ItemListViewModel()
        {
            this.Title = "Item List";
            this.ContentId = "Item List";

            this.AddTilesetCommand = new DelegateCommand(() => AddTileset());
            this.ViewPropertiesCommand = new DelegateCommand(() => ViewProperties());
        }

        #endregion constructor & destructer


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


        #region properties

        public ICommand ViewPropertiesCommand { get; private set; }
        public ICommand AddTilesetCommand { get; private set; }

        #endregion properties
    }
}
