using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Ame.Infrastructure.BaseTypes;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;

namespace Ame.Modules.Windows.TilesetEditorWindow
{
    public class TilesetEditorViewModel : BindableBase, IInteractionRequestAware
    {
        #region fields

        #endregion fields


        #region constructor

        public TilesetEditorViewModel(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }
            this.WindowTitle = "New Map";

            this.CloseWindowCommand = new DelegateCommand(CloseWindow);
            this.AddCustomMetaDataCommand = new DelegateCommand(AddCustomProperty);
            this.RemoveCustomMetadataCommand = new DelegateCommand(RemoveCustomProperty);
            this.MoveMetadataUpCommand = new DelegateCommand(MoveMetadataUp);
            this.MoveMetadataDownCommand = new DelegateCommand(MoveMetadataDown);
        }

        #endregion constructor


        #region properties

        public ICommand CloseWindowCommand { get; private set; }
        public ICommand AddCustomMetaDataCommand { get; private set; }
        public ICommand RemoveCustomMetadataCommand { get; private set; }
        public ICommand MoveMetadataUpCommand { get; private set; }
        public ICommand MoveMetadataDownCommand { get; private set; }

        public string WindowTitle { get; set; }

        public IConfirmation notification { get; set; }
        public INotification Notification
        {
            get { return this.notification; }
            set
            {
                this.notification = value as IConfirmation;
                RaisePropertyChanged(nameof(this.Notification));
            }
        }

        public ICollectionView GroupedProperties { get; set; }
        public ICollectionView MapMetadata { get; set; }
        public ObservableCollection<MetadataProperty> MetadataList { get; set; }

        public MetadataProperty selectedMetadata;
        public MetadataProperty SelectedMetadata
        {
            get
            {
                return selectedMetadata;
            }
            set
            {
                this.IsCustomSelected = value.Type == MetadataType.Custom ? true : false;
                SetProperty(ref this.selectedMetadata, value);
            }
        }

        public bool isCustomSelected;
        public bool IsCustomSelected
        {
            get
            {
                return isCustomSelected;
            }
            set
            {
                SetProperty(ref this.isCustomSelected, value);
            }
        }

        public Action FinishInteraction { get; set; }

        #endregion properties


        #region methods

        private void CloseWindow()
        {
            if (this.notification != null)
            {
                this.notification.Confirmed = false;
            }
            FinishInteraction();
        }

        private void UpdateMetadata()
        {
            //this.MetadataList = MetadataPropertyUtils.GetPropertyList(this.Map);
            //this.MetadataList.Add(new MetadataProperty("Layer Count", this.Map.LayerList.Count, MetadataType.Statistic));
            //this.MapMetadata = new ListCollectionView(this.MetadataList);
            //this.MapMetadata.GroupDescriptions.Add(new PropertyGroupDescription("Type"));
        }

        private void AddCustomProperty()
        {
            int customCount = this.MetadataList.Count(p => p.Type == MetadataType.Custom);
            String customName = String.Format("Custom #{0}", customCount);
            this.MetadataList.Add(new MetadataProperty(customName, "", MetadataType.Custom));
        }

        private void RemoveCustomProperty()
        {
            if (this.SelectedMetadata.Type == MetadataType.Custom)
            {
                this.MetadataList.Remove(this.SelectedMetadata);
            }
        }

        // TODO move this into a seperate class
        private void MoveMetadataUp()
        {
            int currentIndex = this.MapMetadata.CurrentPosition;
            MetadataProperty currentItem = this.MapMetadata.CurrentItem as MetadataProperty;
            MetadataType currentItemType = currentItem.Type;

            int propertyIndex = 0;
            int statisticIndex = this.MetadataList.Count(p => p.Type == MetadataType.Property) + propertyIndex;
            int customIndex = this.MetadataList.Count(p => p.Type == MetadataType.Statistic) + statisticIndex;
            int lowestIndex = 0;
            switch (currentItemType)
            {
                case MetadataType.Property:
                    lowestIndex = propertyIndex;
                    break;

                case MetadataType.Statistic:
                    lowestIndex = statisticIndex;
                    break;

                case MetadataType.Custom:
                    lowestIndex = customIndex;
                    break;

                default:
                    break;
            }
            if (currentIndex > lowestIndex)
            {
                this.MetadataList.Move(currentIndex, currentIndex - 1);
                this.MapMetadata.Refresh();
            }
        }

        private void MoveMetadataDown()
        {
            int currentIndex = this.MapMetadata.CurrentPosition;
            MetadataProperty currentItem = this.MapMetadata.CurrentItem as MetadataProperty;
            MetadataType currentItemType = currentItem.Type;

            int propertyIndex = 0;
            int statisticIndex = this.MetadataList.Count(p => p.Type == MetadataType.Property) + propertyIndex;
            int customIndex = this.MetadataList.Count(p => p.Type == MetadataType.Statistic) + statisticIndex;
            int highestIndex = 0;
            switch (currentItemType)
            {
                case MetadataType.Property:
                    highestIndex = statisticIndex - 1;
                    break;

                case MetadataType.Statistic:
                    highestIndex = customIndex - 1;
                    break;

                case MetadataType.Custom:
                    highestIndex = this.MetadataList.Count - 1;
                    break;

                default:
                    break;
            }
            if (currentIndex < highestIndex)
            {
                this.MetadataList.Move(currentIndex, currentIndex + 1);
                this.MapMetadata.Refresh();
            }
        }

        #endregion methods
    }
}
