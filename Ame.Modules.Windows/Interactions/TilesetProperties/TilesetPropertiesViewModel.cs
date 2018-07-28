using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Ame.Infrastructure.Attributes;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.Utils;
using Emgu.CV;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;

namespace Ame.Modules.Windows.Interactions.TilesetProperties
{
    // TODO create model class for handeling metadata
    // TODO set up cancel button
    // TODO add image of tileset
    public class TilesetPropertiesViewModel : BindableBase, IInteractionRequestAware
    {
        #region fields

        #endregion fields


        #region constructor

        public TilesetPropertiesViewModel(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }
            this.WindowTitle = "New Map";

            // TODO change to lambda
            this.SetTilesetCommand = new DelegateCommand(SetTileset);
            this.CloseWindowCommand = new DelegateCommand(CloseWindow);
            this.AddCustomMetaDataCommand = new DelegateCommand(AddCustomProperty);
            this.RemoveCustomMetadataCommand = new DelegateCommand(RemoveCustomProperty);
            this.MoveMetadataUpCommand = new DelegateCommand(MoveMetadataUp);
            this.MoveMetadataDownCommand = new DelegateCommand(MoveMetadataDown);
            this.BrowseSourceCommand = new DelegateCommand(BrowseSource);
        }

        #endregion constructor


        #region properties

        public ICommand SetTilesetCommand { get; private set; }
        public ICommand CloseWindowCommand { get; private set; }
        public ICommand AddCustomMetaDataCommand { get; private set; }
        public ICommand RemoveCustomMetadataCommand { get; private set; }
        public ICommand MoveMetadataUpCommand { get; private set; }
        public ICommand MoveMetadataDownCommand { get; private set; }
        public ICommand BrowseSourceCommand { get; set; }

        public string WindowTitle { get; set; }

        public IConfirmation notification { get; set; }
        public INotification Notification
        {
            get { return this.notification; }
            set
            {
                this.notification = value as IConfirmation;
                this.TilesetModel = this.notification.Content as TilesetModel;
                if (this.TilesetModel != null)
                {
                    UpdateMetadata();
                }
                RaisePropertyChanged(nameof(this.Notification));
            }
        }

        public TilesetModel TilesetModel { get; set; }

        public ICollectionView GroupedProperties { get; set; }
        public ICollectionView TilesetMetadata { get; set; }
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

        public string fileSourcePath;
        public string FileSourcePath
        {
            get
            {
                return fileSourcePath;
            }
            set
            {
                SetProperty(ref this.fileSourcePath, value);
            }
        }

        public bool IsTransparent { get; set; }
        public Color TransparentColor { get; set; }

        public Action FinishInteraction { get; set; }

        #endregion properties


        #region methods

        private void SetTileset()
        {
            if (this.notification != null)
            {
                this.notification.Confirmed = true;
            }
            FinishInteraction();
        }

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
            this.MetadataList = MetadataPropertyUtils.GetPropertyList(this.TilesetModel);
            this.TilesetMetadata = new ListCollectionView(this.MetadataList);
            this.TilesetMetadata.GroupDescriptions.Add(new PropertyGroupDescription("Type"));
        }

        private void AddCustomProperty()
        {
            int customCount = this.MetadataList.Count(p => p.Type == MetadataType.Custom);
            string customName = string.Format("Custom #{0}", customCount);
            this.MetadataList.Add(new MetadataProperty(customName, "", MetadataType.Custom));
        }

        private void RemoveCustomProperty()
        {
            if (this.SelectedMetadata.Type == MetadataType.Custom)
            {
                this.MetadataList.Remove(this.SelectedMetadata);
            }
        }

        private void MoveMetadataUp()
        {
            int currentIndex = this.TilesetMetadata.CurrentPosition;
            MetadataProperty currentItem = this.TilesetMetadata.CurrentItem as MetadataProperty;
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
                this.TilesetMetadata.Refresh();
            }
        }

        private void MoveMetadataDown()
        {
            int currentIndex = this.TilesetMetadata.CurrentPosition;
            MetadataProperty currentItem = this.TilesetMetadata.CurrentItem as MetadataProperty;
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
                this.TilesetMetadata.Refresh();
            }
        }

        private void BrowseSource()
        {
            OpenFileDialog openTilesetDilog = new OpenFileDialog();
            openTilesetDilog.Title = "Select a Tileset";
            openTilesetDilog.Filter = ImageExtension.GetOpenFileImageExtensions();
            if (openTilesetDilog.ShowDialog() == true)
            {
                string tileFilePath = openTilesetDilog.FileName;
                if (File.Exists(tileFilePath))
                {
                    this.FileSourcePath = tileFilePath;
                    this.TilesetModel.SourcePath = tileFilePath;
                    Mat matImage = CvInvoke.Imread(tileFilePath, Emgu.CV.CvEnum.ImreadModes.Unchanged);
                    this.TilesetModel.PixelWidth = matImage.Width;
                    this.TilesetModel.PixelHeight = matImage.Height;
                }
            }
        }

        #endregion methods
    }
}
