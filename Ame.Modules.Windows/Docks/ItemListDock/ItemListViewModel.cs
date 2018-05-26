using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Prism.Commands;
using Prism.Events;

namespace Ame.Modules.Windows.Docks.ItemListDock
{
    public class ItemListViewModel : DockToolViewModelTemplate
    {
        // TODO only load a resource once.

        #region fields

        private IEventAggregator eventAggregator;

        #endregion fields


        #region constructor

        public ItemListViewModel(IEventAggregator eventAggregator, AmeSession session)
        {
            this.eventAggregator = eventAggregator;
            this.Session = session;
            this.Title = "Item List";

            this.AddTilesetCommand = new DelegateCommand(() => AddTileset());
            this.AddImageCommand = new DelegateCommand(() => AddImage());
            this.AddGroupCommand = new DelegateCommand(() => AddGroup());
            this.ViewPropertiesCommand = new DelegateCommand(() => ViewProperties());
            this.CurrentItemChangedCommand = new DelegateCommand<object>((currentItem) => CurrentItemChanged((IItem)currentItem));

            this.Items = new ObservableCollection<IItem>();
            ItemGroup itemGroup1 = new ItemGroup("Item Group #1");
            itemGroup1.Items.Add(new TilesetModel("Tileset #1"));
            itemGroup1.Items.Add(new TilesetModel("Tileset #2"));

            ItemGroup itemGroup2 = new ItemGroup("Item Group #2");
            itemGroup2.Items.Add(new TilesetModel("Tileset #3"));
            ItemGroup itemGroup3 = new ItemGroup("Item Group #3");
            itemGroup3.Items.Add(new TilesetModel("Tileset #4"));
            itemGroup2.Items.Add(itemGroup3);
            itemGroup2.Items.Add(new TilesetModel("Tileset #5"));
            
            this.Items.Add(itemGroup1);
            this.Items.Add(itemGroup2);
            this.Items.Add(new TilesetModel("Tileset #6"));
        }

        #endregion constructor


        #region properties

        public ICommand ViewPropertiesCommand { get; private set; }
        public ICommand CurrentItemChangedCommand { get; private set; }
        public ICommand AddTilesetCommand { get; private set; }
        public ICommand AddImageCommand { get; private set; }
        public ICommand AddGroupCommand { get; private set; }

        public AmeSession Session { get; set; }
        public ObservableCollection<IItem> Items { get; set; }
        public IItem Item { get; set; }

        #endregion properties


        #region methods

        public void CurrentItemChanged(IItem item)
        {
            this.Item = item;
        }

        public void AddTileset()
        {
            string modelName = string.Format("Tileset #{0}", GetTilesetModelCount(this.Items) + 1);
            TilesetModel model = new TilesetModel(modelName);
            if (this.Item != null)
            {
                this.Item.Items.Add(model);
            }
            else
            {
                this.Items.Add(model);
            }
        }

        public void AddImage()
        {
            Console.WriteLine("Add Image");
        }

        public void AddGroup()
        {
            string modelName = string.Format("Item Group #{0}", GetItemGroupCount(this.Items) + 1);
            ItemGroup group = new ItemGroup(modelName);
            if (this.Item != null)
            {
                this.Item.Items.Add(group);
            }
            else
            {
                this.Items.Add(group);
            }
        }

        public void ViewProperties()
        {
            Console.WriteLine("View Properties ");
        }
        
        private int GetTilesetModelCount(ObservableCollection<IItem> items)
        {
            int tilesetModelCount = 0;
            foreach (IItem item in items)
            {
                if (item is ItemGroup)
                {
                    tilesetModelCount += GetTilesetModelCount(item.Items);
                }
                if (item is TilesetModel)
                {
                    tilesetModelCount++;
                }
            }
            return tilesetModelCount;
        }

        private int GetItemGroupCount(ObservableCollection<IItem> items)
        {
            int groupCount = 0;
            foreach (IItem item in items)
            {
                if (item is ItemGroup)
                {
                    groupCount++;
                    groupCount += GetItemGroupCount(item.Items);
                }
            }
            return groupCount;
        }

        #endregion methods
    }
}
