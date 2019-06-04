using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Messages;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ame.App.Wpf.UI.Docks.ClipboardDock
{
    public class ClipboardViewModel : DockToolViewModelTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;

        #endregion fields


        #region Constructors

        public ClipboardViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.Title = "Clipboard";

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

        public override void CloseDock()
        {
            CloseDockMessage closeMessage = new CloseDockMessage(this);
            this.eventAggregator.GetEvent<CloseDockEvent>().Publish(closeMessage);
        }

        #endregion methods
    }
}
