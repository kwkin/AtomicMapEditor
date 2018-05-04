using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ame.Modules.Windows.WindowInteractions;

namespace Ame.Modules.Windows.WindowInteractionFactories
{
    public interface IWindowStrategy
    {
        IWindowInteraction CreateWindowInteraction(Type type);
    }
}
