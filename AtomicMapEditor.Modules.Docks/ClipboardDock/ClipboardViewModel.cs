using System;
using System.Windows.Input;
using Ame.Infrastructure.BaseTypes;
using Prism.Commands;

namespace Ame.Modules.Docks.ClipboardDock
{
    public class ClipboardViewModel : DockToolViewModelTemplate
    {
        #region fields

        #endregion fields


        #region Constructors

        public ClipboardViewModel()
        {
            this.Title = "Clipboard";
            this.ContentId = "Clipboard";

            this.RemoveItemCommand = new DelegateCommand(() => RemoveItem());
            this.SortCommand = new DelegateCommand(() => Sort());
            this.SetViewCommand = new DelegateCommand(() => SetView());
            this.ShowMapsCommand = new DelegateCommand(() => ShowMaps());
            this.ShowTilesetsCommand = new DelegateCommand(() => ShowTilesets());
            this.ShowImagesCommand = new DelegateCommand(() => ShowImages());
            this.ZoomInCommand = new DelegateCommand(() => ZoomIn());
            this.ZoomOutCommand = new DelegateCommand(() => ZoomOut());
            this.SetItemSizeCommand = new DelegateCommand(() => SetItemSize());
            this.ExpandAllCommand = new DelegateCommand(() => ExpandAll());
            this.CollapseAllCommand = new DelegateCommand(() => CollapseAll());
            this.ShowGroupsCommand = new DelegateCommand(() => ShowGroups());
        }

        #endregion Constructors


        #region properties

        public ICommand RemoveItemCommand { get; private set; }
        public ICommand SortCommand { get; private set; }
        public ICommand SetViewCommand { get; private set; }
        public ICommand ShowMapsCommand { get; private set; }
        public ICommand ShowTilesetsCommand { get; private set; }
        public ICommand ShowImagesCommand { get; private set; }
        public ICommand ZoomInCommand { get; private set; }
        public ICommand ZoomOutCommand { get; private set; }
        public ICommand SetItemSizeCommand { get; private set; }
        public ICommand ExpandAllCommand { get; private set; }
        public ICommand CollapseAllCommand { get; private set; }
        public ICommand ShowGroupsCommand { get; private set; }

        #endregion properties


        #region methods

        public void RemoveItem()
        {
            Console.WriteLine("RemoveItem");
        }

        public void Sort()
        {
            Console.WriteLine("Sort");
        }

        public void SetView()
        {
            Console.WriteLine("SetView");
        }

        public void ShowMaps()
        {
            Console.WriteLine("ShowMaps");
        }

        public void ShowTilesets()
        {
            Console.WriteLine("ShowTilesets");
        }

        public void ShowImages()
        {
            Console.WriteLine("ShowImages");
        }

        public void ZoomIn()
        {
            Console.WriteLine("ZoomIn");
        }

        public void ZoomOut()
        {
            Console.WriteLine("ZoomOut");
        }

        public void SetItemSize()
        {
            Console.WriteLine("SetItemSize");
        }

        public void ExpandAll()
        {
            Console.WriteLine("ExpandAll");
        }

        public void CollapseAll()
        {
            Console.WriteLine("CollapseAll");
        }

        public void ShowGroups()
        {
            Console.WriteLine("ShowGroups");
        }

        #endregion methods
    }
}
