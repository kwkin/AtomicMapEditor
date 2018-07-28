﻿using System;
using Ame.Components.Behaviors;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Ame.Modules.Windows;
using Prism.Events;

namespace Ame.Modules.MapEditor.Editor
{
    public class MapEditorCreator : DockCreatorTemplate
    {
        #region fields

        public IEventAggregator eventAggregator;

        #endregion fields


        #region constructors

        public MapEditorCreator(IEventAggregator eventAggregator, AmeSession session)
            : this(eventAggregator, session, null, null)
        {
        }

        public MapEditorCreator(IEventAggregator eventAggregator, AmeSession session, Map map, ScrollModel scrollModel)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator is null");
            }
            string mapTitle = string.Format("Map #{0}", session.MapCount);
            map = map ?? new Map(mapTitle);

            this.eventAggregator = eventAggregator;
            this.Map = map;
            this.ScrollModel = scrollModel;
        }

        #endregion constructors


        #region properties

        public Map Map { get; set; }
        public ScrollModel ScrollModel { get; set; }

        #endregion properties


        #region methods

        public override DockViewModelTemplate CreateDock()
        {
            DockViewModelTemplate template;
            if (this.ScrollModel != null)
            {
                template = new MapEditorViewModel(this.eventAggregator, this.Map, this.ScrollModel);
            }
            else
            {
                template = new MapEditorViewModel(this.eventAggregator, this.Map);
            }
            return template;
        }

        public override bool AppliesTo(Type type)
        {
            return typeof(MapEditorViewModel).Equals(type);
        }

        #endregion methods
    }
}
