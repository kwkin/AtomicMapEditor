﻿using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.App.Wpf.UI.Docks.MinimapDock
{
    public class MinimapCreator : DockCreatorTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;
        private IAmeSession session;

        #endregion fields


        #region constructors

        public MinimapCreator(IEventAggregator eventAggregator, IAmeSession session)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator is null");
            this.session = session ?? throw new ArgumentNullException("session");
        }

        #endregion constructors


        #region properties

        #endregion properties


        #region methods

        public override DockViewModelTemplate CreateDock()
        {
            return new MinimapViewModel(this.eventAggregator, this.session);
        }

        public override bool AppliesTo(Type type)
        {
            return typeof(MinimapViewModel).Equals(type);
        }

        #endregion methods
    }
}
