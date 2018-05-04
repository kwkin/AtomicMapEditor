using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ame.Modules.Windows.WindowInteractions;

namespace Ame.Modules.Windows.WindowInteractionFactories
{
    public class WindowStrategy : IWindowStrategy
    {
        #region fields

        private readonly IWindowInteractionFactory[] windowInteractionFactories;

        #endregion fields


        #region constructors
        
        public WindowStrategy(IWindowInteractionFactory[] windowInteractionFactories)
        {
            this.windowInteractionFactories = windowInteractionFactories;
        }

        #endregion constructors


        #region properties

        #endregion properties


        #region methods

        public IWindowInteraction CreateWindowInteraction(Type type)
        {
            var windowInteractionFactory = this.windowInteractionFactories.FirstOrDefault(factory => factory.AppliesTo(type));
            if (windowInteractionFactory == null)
            {
                throw new Exception(string.Format("{0} type is not registered", type));
            }
            return windowInteractionFactory.CreateWindowInteraction();
        }

        #endregion methods
    }
}
