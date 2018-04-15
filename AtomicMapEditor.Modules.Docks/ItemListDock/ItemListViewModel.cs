using System;
using System.Windows.Input;
using Ame.Infrastructure.BaseTypes;
using Prism.Commands;

namespace Ame.Modules.Docks.ItemListDock
{
    public class ItemListViewModel : DockToolViewModelTemplate
    {
        #region fields

        #endregion fields


        #region constructor

        public ItemListViewModel()
        {
            this.Title = "Item List";

            this.AddTilesetCommand = new DelegateCommand(() => AddTileset());
            this.ViewPropertiesCommand = new DelegateCommand(() => ViewProperties());
        }

        #endregion constructor


        #region properties

        public ICommand ViewPropertiesCommand { get; private set; }
        public ICommand AddTilesetCommand { get; private set; }

        public override DockType DockType
        {
            get
            {
                return DockType.ItemList;
            }
        }

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
