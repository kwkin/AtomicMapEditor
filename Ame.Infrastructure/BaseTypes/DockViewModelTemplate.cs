using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;

namespace Ame.Infrastructure.BaseTypes
{
    public abstract class DockViewModelTemplate : BindableBase
    {
        #region fields

        #endregion fields


        #region constructor

        #endregion constructor


        #region properties
        
        private bool isActive = false;
        public bool IsActive
        {
            get { return this.isActive; }
            set { SetProperty(ref this.isActive, value); }
        }

        private bool isSelected = false;
        public bool IsSelected
        {
            get { return this.isSelected; }
            set { SetProperty(ref this.isSelected, value); }
        }

        private bool isVisible = false;
        public bool IsVisible
        {
            get { return this.isVisible; }
            set { SetProperty(ref this.isVisible, value); }
        }

        private string title;
        public string Title
        {
            get { return this.title; }
            set { SetProperty(ref this.title, value); }
        }
        
        public string ContentId
        {
            get
            {
                return this.GetType().Name;
            }
        }

        DelegateCommand closeCommand = null;
        public ICommand CloseCommand
        {
            get
            {
                if (closeCommand == null)
                {
                    this.closeCommand = new DelegateCommand(CloseDock);
                }
                return this.closeCommand;
            }
        }

        #endregion properties


        #region methods

        public abstract void CloseDock();

        #endregion methods
    }
}
