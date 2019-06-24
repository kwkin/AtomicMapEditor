using Ame.Infrastructure.BaseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.App.Wpf.UI
{
    public interface ILayoutViewModel
    {
        BindableProperty<bool> IsBusy { get; set; }
    }
}
