﻿using Ame.Infrastructure.BaseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.App.Wpf.UI.Docks
{
    public class DockCreator
    {
        #region fields

        private readonly DockCreatorTemplate[] dockCreators;

        #endregion fields


        #region constructors

        public DockCreator(DockCreatorTemplate[] windowInteractionFactories)
        {
            this.dockCreators = windowInteractionFactories;
        }

        #endregion constructors


        #region properties

        #endregion properties


        #region methods

        public DockViewModelTemplate CreateDock(Type type)
        {
            var dockCreator = this.dockCreators.FirstOrDefault(factory => factory.AppliesTo(type));
            if (dockCreator == null)
            {
                throw new Exception(string.Format("{0} type is not registered", type));
            }
            return dockCreator.CreateDock();
        }

        public void UpdateContainer(Type creatorType, Type valueType, object value)
        {
            var dockCreator = this.dockCreators.FirstOrDefault(factory => factory.AppliesTo(creatorType));
            if (dockCreator == null)
            {
                throw new Exception(string.Format("{0} type is not registered", creatorType));
            }
            dockCreator.UpdateContent(valueType, value);
        }

        #endregion methods
    }
}
