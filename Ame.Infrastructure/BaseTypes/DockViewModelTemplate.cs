using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;

namespace Ame.Infrastructure.BaseTypes
{
    public abstract class DockViewModelTemplate
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

        public object GetNewValue(object sender, PropertyChangedEventArgs e)
        {
            var property = sender.GetType().GetProperty(e.PropertyName);
            return property.GetValue(sender, null);
        }

        #endregion methods
    }
}
