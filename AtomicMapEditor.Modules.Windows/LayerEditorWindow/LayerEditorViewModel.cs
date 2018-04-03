using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;

namespace Ame.Modules.Windows.LayerEditorWindow
{
    public class LayerEditorViewModel : BindableBase, IInteractionRequestAware
    {
        #region fields

        #endregion fields


        #region constructor & destructer

        public LayerEditorViewModel()
        {
            this.WindowTitle = "Layer Editor";

            this.SetLayerPropertiesCommand = new DelegateCommand(SetLayerProperties);
            this.ApplyLayerPropertiesCommand = new DelegateCommand(ApplyLayerProperties);
            this.CloseWindowCommand = new DelegateCommand(CloseWindow);
        }

        #endregion constructor & destructer


        #region properties

        public ICommand SetLayerPropertiesCommand { get; private set; }
        public ICommand CloseWindowCommand { get; private set; }
        public ICommand ApplyLayerPropertiesCommand { get; private set; }

        public string WindowTitle { get; set; }

        public Confirmation notification { get; set; }
        public INotification Notification
        {
            get { return this.notification; }
            set
            {
                this.notification = value as Confirmation;
                RaisePropertyChanged(nameof(this.Notification));
            }
        }

        public Action FinishInteraction { get; set; }

        #endregion properties


        #region methods

        private void SetLayerProperties()
        {
            ApplyLayerProperties();
            if (this.notification != null)
            {
                this.notification.Confirmed = true;
            }
            FinishInteraction();
        }

        private void ApplyLayerProperties()
        {
            Console.WriteLine("Apply Layer Properties");
        }

        private void CloseWindow()
        {
            if (this.notification != null)
            {
                this.notification.Confirmed = false;
            }
            FinishInteraction();
        }

        private void Cancel()
        {
            Console.WriteLine("Cancel");
        }

        #endregion methods
    }
}
