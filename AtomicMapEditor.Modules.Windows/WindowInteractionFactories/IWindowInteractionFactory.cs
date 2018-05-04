using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ame.Modules.Windows.WindowInteractions;
using Microsoft.Practices.Unity;

namespace Ame.Modules.Windows.WindowInteractionFactories
{
    public interface IWindowInteractionFactory
    {
        IWindowInteraction CreateWindowInteraction();
        IUnityContainer Container { get; set; }
        bool AppliesTo(Type type);
    }
}
