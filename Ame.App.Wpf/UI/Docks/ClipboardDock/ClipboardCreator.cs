﻿using Ame.Infrastructure.BaseTypes;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.App.Wpf.UI.Docks.ClipboardDock
{
    public class ClipboardCreator : DockCreatorTemplate
    {
        #region fields

        public IEventAggregator eventAggregator;

        #endregion fields


        #region constructors

        public ClipboardCreator(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator is null");
        }

        #endregion constructors


        #region properties

        #endregion properties


        #region methods

        public override DockViewModelTemplate CreateDock()
        {
            return new ClipboardViewModel(this.eventAggregator);
        }

        public override bool AppliesTo(Type type)
        {
            return typeof(ClipboardViewModel).Equals(type);
        }

        #endregion methods
    }
}
