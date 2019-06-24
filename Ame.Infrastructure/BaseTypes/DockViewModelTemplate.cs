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

        public BindableProperty<bool> IsActive { get; set; } = BindableProperty<bool>.Prepare();

        public BindableProperty<bool> IsSelected { get; set; } = BindableProperty<bool>.Prepare();

        public BindableProperty<bool> IsVisible { get; set; } = BindableProperty<bool>.Prepare();

        public BindableProperty<string> Title { get; set; } = BindableProperty<string>.Prepare();
        
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
