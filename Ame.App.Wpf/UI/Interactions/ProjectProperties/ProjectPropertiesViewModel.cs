using Ame.Infrastructure.Attributes;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Handlers;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.Utils;
using Microsoft.WindowsAPICodePack.Dialogs;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace Ame.App.Wpf.UI.Interactions.ProjectProperties
{
    // TODO remove event aggregator where ever it is not used.
    public class ProjectPropertiesViewModel : IInteractionRequestAware
    {
        #region fields

        private AmeSession session;

        #endregion fields


        #region constructor

        public ProjectPropertiesViewModel(AmeSession session)
        {
            this.session = session ?? throw new ArgumentNullException("session");

            this.WindowTitle.Value = "New Project";

            this.Name.PropertyChanged += SpecifiedPathChanged;
            this.SpecifiedPath.PropertyChanged += SpecifiedPathChanged;
            this.IsSourceSpecified.Value = true;

            this.SetProjectPropertiesCommand = new DelegateCommand(() => SetProjectProperties());
            this.CloseWindowCommand = new DelegateCommand(() => CloseWindow());
            this.BrowseSourceCommand = new DelegateCommand(() => BrowseSource());
        }

        #endregion constructor


        #region properties

        public ICommand SetProjectPropertiesCommand { get; private set; }
        public ICommand CloseWindowCommand { get; private set; }
        public ICommand BrowseSourceCommand { get; set; }

        public BindableProperty<string> WindowTitle { get; set; } = BindableProperty<string>.Prepare(string.Empty);

        public BindableProperty<string> SpecifiedPath { get; set; } = BindableProperty<string>.Prepare(string.Empty);

        public BindableProperty<string> Name { get; set; } = BindableProperty<string>.Prepare(string.Empty);

        public BindableProperty<int> TileWidth { get; set; } = BindableProperty<int>.Prepare();

        public BindableProperty<int> TileHeight { get; set; } = BindableProperty<int>.Prepare();

        public BindableProperty<int> PixelScale { get; set; } = BindableProperty<int>.Prepare();

        public BindableProperty<string> Description { get; set; } = BindableProperty<string>.Prepare();

        public BindableProperty<Project> Project { get; set; } = BindableProperty<Project>.Prepare();

        public BindableProperty<string> FullLocation { get; set; } = BindableProperty<string>.Prepare(string.Empty);

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

        public MetadataHandler MetadataHandler { get; set; }

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
            project.SourcePath.Value = this.FullLocation.Value;
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
            this.SpecifiedPath.Value = project.SourcePath.Value;
            this.TileWidth.Value = project.DefaultTileWidth.Value;
            this.TileHeight.Value = project.DefaultTileHeight.Value;
            this.PixelScale.Value = project.DefaultPixelScale.Value;
            this.Description.Value = project.Description.Value;

            this.SpecifiedPath.Value = session.DefaultWorkspaceDirectory.Value;
            this.IsSourceSpecified.Value = true;
        }

        private void BrowseSource()
        {
            CommonOpenFileDialog folderDialog = new CommonOpenFileDialog();
            folderDialog.Title = "Select the Project Location";
            folderDialog.InitialDirectory = this.session.LastMapDirectory.Value;
            folderDialog.IsFolderPicker = true;
            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.SpecifiedPath.Value = folderDialog.FileName;
                this.IsSourceSpecified.Value = true;
                this.session.LastMapDirectory.Value = Directory.GetParent(this.SpecifiedPath.Value).FullName;
            }
        }

        private void UpdateMetadata()
        {
            ObservableCollection<MetadataProperty> properties = new ObservableCollection<MetadataProperty>();
            properties.Add(new MetadataProperty("Map Count", this.Project.Value.MapCount, MetadataType.Statistic));

            this.MetadataHandler = new MetadataHandler(this.Project.Value, properties);
        }

        private void SpecifiedPathChanged(object sender, PropertyChangedEventArgs e)
        {
            SpecifiedPathChanged();
        }

        private void SpecifiedPathChanged()
        {
            if (this.SpecifiedPath.Value == null || this.Name.Value == null)
            {
                return;
            }
            this.FullLocation.Value = Path.Combine(this.SpecifiedPath.Value, this.Name.Value);
        }

        #endregion methods
    }
}
