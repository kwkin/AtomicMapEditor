using Ame.Infrastructure.Attributes;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.Utils;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;

namespace Ame.App.Wpf.UI.Interactions.ProjectProperties
{
    public class ProjectPropertiesViewModel : IInteractionRequestAware
    {
        #region fields

        private IEventAggregator eventAggregator;
        private AmeSession session;

        #endregion fields


        #region constructor

        public ProjectPropertiesViewModel(IEventAggregator eventAggregator, AmeSession session)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator");
            this.session = session ?? throw new ArgumentNullException("session");

            this.WindowTitle.Value = "New Project";

            this.SelectedMetadata.PropertyChanged += SelectedMetadataChanged;

            this.SetProjectPropertiesCommand = new DelegateCommand(() => SetProjectProperties());
            this.CloseWindowCommand = new DelegateCommand(() => CloseWindow());
            this.BrowseSourceCommand = new DelegateCommand(() => BrowseSource());
            this.AddCustomMetaDataCommand = new DelegateCommand(() => AddCustomProperty());
            this.RemoveCustomMetadataCommand = new DelegateCommand(() => RemoveCustomProperty());
            this.MoveMetadataUpCommand = new DelegateCommand(() => MoveMetadataUp());
            this.MoveMetadataDownCommand = new DelegateCommand(() => MoveMetadataDown());
        }

        #endregion constructor


        #region properties

        public ICommand SetProjectPropertiesCommand { get; private set; }
        public ICommand CloseWindowCommand { get; private set; }
        public ICommand BrowseSourceCommand { get; set; }
        public ICommand AddCustomMetaDataCommand { get; private set; }
        public ICommand RemoveCustomMetadataCommand { get; private set; }
        public ICommand MoveMetadataUpCommand { get; private set; }
        public ICommand MoveMetadataDownCommand { get; private set; }

        public BindableProperty<string> WindowTitle { get; set; } = BindableProperty<string>.Prepare(string.Empty);

        public BindableProperty<string> SourcePath { get; set; } = BindableProperty<string>.Prepare(string.Empty);

        public BindableProperty<string> Name { get; set; } = BindableProperty<string>.Prepare();

        public BindableProperty<int> TileWidth { get; set; } = BindableProperty<int>.Prepare();

        public BindableProperty<int> TileHeight { get; set; } = BindableProperty<int>.Prepare();

        public BindableProperty<int> PixelScale { get; set; } = BindableProperty<int>.Prepare();

        public BindableProperty<string> Description { get; set; } = BindableProperty<string>.Prepare();

        public BindableProperty<Project> Project { get; set; } = BindableProperty<Project>.Prepare();

        public IConfirmation notification { get; set; }
        public INotification Notification
        {
            get { return this.notification; }
            set
            {
                this.notification = value as IConfirmation;
                this.Project.Value = this.notification.Content as Project;
                UpdateUIusingProject(this.Project.Value);
                if (this.Project.Value != null)
                {
                    UpdateMetadata();
                }
            }
        }

        public ICollectionView GroupedProperties { get; set; }
        public ICollectionView ProjectMetadata { get; set; }
        public ObservableCollection<MetadataProperty> MetadataList { get; set; }

        public BindableProperty<MetadataProperty> SelectedMetadata { get; set; } = BindableProperty<MetadataProperty>.Prepare();

        public BindableProperty<bool> IsCustomSelected { get; set; } = BindableProperty<bool>.Prepare();

        public Action FinishInteraction { get; set; }

        public BindableProperty<bool> IsSourceSpecified { get; set; } = BindableProperty<bool>.Prepare();

        #endregion properties


        #region methods

        private void SetProjectProperties()
        {
            UpdateProjectProperties(this.Project.Value);
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

        private void UpdateProjectProperties()
        {
            UpdateProjectProperties(this.Project.Value);
        }

        private void UpdateProjectProperties(Project project)
        {
            project.Name.Value = this.Name.Value;
            project.SourcePath.Value = this.SourcePath.Value;
            project.DefaultTileWidth.Value = this.TileWidth.Value;
            project.DefaultTileHeight.Value = this.TileHeight.Value;
            project.DefaultPixelScale.Value = this.PixelScale.Value;
            project.Description.Value = this.Description.Value;
        }

        private void UpdateUIusingProject()
        {
            UpdateUIusingProject(this.Project.Value);
        }

        private void UpdateUIusingProject(Project project)
        {
            this.Name.Value = project.Name.Value;
            this.SourcePath.Value = project.SourcePath.Value;
            this.TileWidth.Value = project.DefaultTileWidth.Value;
            this.TileHeight.Value = project.DefaultTileHeight.Value;
            this.PixelScale.Value = project.DefaultPixelScale.Value;
            this.Description.Value = project.Description.Value;

            if (this.SourcePath.Value != null && this.SourcePath.Value != string.Empty)
            {
                this.IsSourceSpecified.Value = true;
            }
            else
            {
                this.IsSourceSpecified.Value = false;
            }
        }

        private void BrowseSource()
        {
            SaveFileDialog saveProjectDialog = new SaveFileDialog();
            saveProjectDialog.Title = "Create a new Project";
            saveProjectDialog.InitialDirectory = this.session.LastMapDirectory;
            saveProjectDialog.Filter = SaveProjectExtension.GetOpenProjectSaveExtensions();
            if (saveProjectDialog.ShowDialog() == DialogResult.OK)
            {
                string projectFilePath = saveProjectDialog.FileName;
                this.IsSourceSpecified.Value = true;
                this.SourcePath.Value = projectFilePath;
                this.session.LastMapDirectory = Directory.GetParent(projectFilePath).FullName;
            }
        }

        // TODO remove duplicate metadata code across other interaction view model classes
        private void UpdateMetadata()
        {
            this.MetadataList = MetadataPropertyUtils.GetPropertyList(this.Project.Value);

            this.MetadataList.Add(new MetadataProperty("Map Count", this.Project.Value.MapCount, MetadataType.Statistic));

            this.ProjectMetadata = new ListCollectionView(this.MetadataList);
            this.ProjectMetadata.GroupDescriptions.Add(new PropertyGroupDescription("Type"));
            foreach (MetadataProperty property in this.Project.Value.CustomProperties)
            {
                this.MetadataList.Add(property);
            }
        }

        private void SelectedMetadataChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.SelectedMetadata.Value != null)
            {
                this.IsCustomSelected.Value = this.SelectedMetadata.Value.Type == MetadataType.Custom ? true : false;
            }
        }

        private void AddCustomProperty()
        {
            int customCount = this.MetadataList.Count(p => p.Type == MetadataType.Custom);
            string customName = string.Format("Custom #{0}", customCount);
            MetadataProperty property = new MetadataProperty(customName, "", MetadataType.Custom);
            this.Project.Value.CustomProperties.Add(property);
            this.MetadataList.Add(property);
        }

        private void RemoveCustomProperty()
        {
            if (this.SelectedMetadata.Value.Type == MetadataType.Custom)
            {
                this.MetadataList.Remove(this.SelectedMetadata.Value);
            }
        }

        private void MoveMetadataUp()
        {
            int currentIndex = this.ProjectMetadata.CurrentPosition;
            MetadataProperty currentItem = this.ProjectMetadata.CurrentItem as MetadataProperty;
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
                this.ProjectMetadata.Refresh();
            }
        }

        private void MoveMetadataDown()
        {
            int currentIndex = this.ProjectMetadata.CurrentPosition;
            MetadataProperty currentItem = this.ProjectMetadata.CurrentItem as MetadataProperty;
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
                this.ProjectMetadata.Refresh();
            }
        }

        #endregion methods
    }
}
