﻿using Ame.App.Wpf.UI.Interactions.TilesetProperties;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Events.Messages;
using Ame.Infrastructure.Models;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ame.App.Wpf.UI.Docks.ItemListDock
{
    public class ItemListViewModel : DockToolViewModelTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;

        #endregion fields


        #region constructor

        public ItemListViewModel(IEventAggregator eventAggregator, IAmeSession session)
        {
            this.eventAggregator = eventAggregator;
            this.Session = session;
            this.Title.Value = "Item List";

            this.CurrentItemChangedCommand = new DelegateCommand<object>((currentItem) => CurrentItemChanged((IItem)currentItem));
            this.AddTilesetCommand = new DelegateCommand(() => AddTileset());
            this.AddImageCommand = new DelegateCommand(() => AddImage());
            this.AddGroupCommand = new DelegateCommand(() => AddGroup());
            this.AddDirectoryCommand = new DelegateCommand(() => AddDirectory());
            this.ViewPropertiesCommand = new DelegateCommand(() => ViewProperties());
            this.RemoveItemCommand = new DelegateCommand(() => RemoveItem());
            this.SortItemListCommand = new DelegateCommand(() => SortItemList());
            this.ShowTilesetsCommand = new DelegateCommand(() => ShowTilesets());
            this.ZoomInCommand = new DelegateCommand(() => ZoomIn());
            this.ZoomOutCommand = new DelegateCommand(() => ZoomOut());
            this.ShowImagesCommand = new DelegateCommand(() => ShowImages());
            this.SetImageSizeCommand = new DelegateCommand(() => SetImageSize());
            this.ExpandAllCommand = new DelegateCommand(() => ExpandAll());
            this.CollapseAllCommand = new DelegateCommand(() => CollapseAll());
            this.ShowGroupsCommand = new DelegateCommand(() => ShowGroups());
            this.EditCollisionsCommand = new DelegateCommand(() => EditCollisions());
            this.RenameItemCommand = new DelegateCommand(() => RenameItem());

            this.Items = new ObservableCollection<IItem>();
            ItemGroup itemGroup1 = new ItemGroup("Item Group #1");
            itemGroup1.Items.Add(new TilesetModel(1, "Tileset #1"));
            itemGroup1.Items.Add(new TilesetModel(2, "Tileset #2"));

            ItemGroup itemGroup2 = new ItemGroup("Item Group #2");
            itemGroup2.Items.Add(new TilesetModel(3, "Tileset #3"));
            ItemGroup itemGroup3 = new ItemGroup("Item Group #3");
            itemGroup3.Items.Add(new TilesetModel(4, "Tileset #4"));
            itemGroup2.Items.Add(itemGroup3);
            itemGroup2.Items.Add(new TilesetModel(5, "Tileset #5"));

            this.Items.Add(itemGroup1);
            this.Items.Add(itemGroup2);
            this.Items.Add(new TilesetModel(6, "Tileset #6"));
        }

        #endregion constructor


        #region properties

        public ICommand CurrentItemChangedCommand { get; private set; }
        public ICommand AddTilesetCommand { get; private set; }
        public ICommand AddImageCommand { get; private set; }
        public ICommand AddDirectoryCommand { get; private set; }
        public ICommand AddGroupCommand { get; private set; }
        public ICommand RemoveItemCommand { get; private set; }
        public ICommand SortItemListCommand { get; private set; }
        public ICommand ShowTilesetsCommand { get; private set; }
        public ICommand ShowImagesCommand { get; private set; }
        public ICommand ZoomInCommand { get; private set; }
        public ICommand ZoomOutCommand { get; private set; }
        public ICommand SetImageSizeCommand { get; private set; }
        public ICommand ExpandAllCommand { get; private set; }
        public ICommand CollapseAllCommand { get; private set; }
        public ICommand ShowGroupsCommand { get; private set; }
        public ICommand ViewPropertiesCommand { get; private set; }
        public ICommand EditCollisionsCommand { get; private set; }
        public ICommand RenameItemCommand { get; private set; }

        public IAmeSession Session { get; set; }
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
            int tilesetCount = GetTilesetModelCount(this.Items);
            string modelName = string.Format("Tileset #{0}", tilesetCount + 1);
            TilesetModel model = new TilesetModel(tilesetCount, modelName);
            if (this.Item != null && (this.Item is ItemGroup))
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

        public void AddDirectory()
        {
            Console.WriteLine("Add Directory");
        }

        public void RemoveItem()
        {
            if (this.Item == null)
            {
                return;
            }
            this.Items.Remove(this.Item);
        }

        public void SortItemList()
        {
            Console.WriteLine("Sort Item List");
        }

        public void ShowTilesets()
        {
            Console.WriteLine("Show Tilesets");
        }

        public void ShowImages()
        {
            Console.WriteLine("Show Images");
        }

        public void ZoomIn()
        {
            Console.WriteLine("Zoom In");
        }

        public void ZoomOut()
        {
            Console.WriteLine("Zoom Out");
        }

        public void SetImageSize()
        {
            Console.WriteLine("Set Image Size");
        }

        public void ExpandAll()
        {
            Console.WriteLine("Expand All");
        }

        public void CollapseAll()
        {
            Console.WriteLine("Collapse All");
        }

        public void ShowGroups()
        {
            Console.WriteLine("Show Groups");
        }

        public void EditCollisions()
        {
            Console.WriteLine("Edit Collisions");
        }

        public void RenameItem()
        {
            Console.WriteLine("Rename Item");
        }

        public void AddGroup()
        {
            string modelName = string.Format("Item Group #{0}", GetItemGroupCount(this.Items) + 1);
            ItemGroup group = new ItemGroup(modelName);
            if (this.Item != null && (this.Item is ItemGroup))
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
            EditTilesetInteraction interaction = new EditTilesetInteraction();
            this.eventAggregator.GetEvent<OpenWindowEvent>().Publish(interaction);
        }

        public override void CloseDock()
        {
            CloseDockMessage closeMessage = new CloseDockMessage(this);
            this.eventAggregator.GetEvent<CloseDockEvent>().Publish(closeMessage);
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
