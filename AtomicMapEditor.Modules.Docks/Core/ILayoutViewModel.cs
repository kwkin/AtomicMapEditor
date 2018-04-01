using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Modules.Docks.Core
{
    public interface ILayoutViewModel
    {
        bool IsBusy { get; set; }
        string AppDataDirectory { get; }
    }
}
