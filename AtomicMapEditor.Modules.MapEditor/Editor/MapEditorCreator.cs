using System;
using Ame.Components.Behaviors;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Ame.Modules.Windows;
using Prism.Events;

namespace Ame.Modules.MapEditor.Editor
{
    public class MapEditorCreator : IDockCreator
    {
        #region fields

        private IEventAggregator eventAggregator;
        private Map map;
        private ScrollModel scrollModel;

        #endregion fields


        #region constructors

        public MapEditorCreator(IEventAggregator eventAggregator, Map map) : this(eventAggregator, map, null)
        {
        }

        public MapEditorCreator(IEventAggregator eventAggregator, Map map, ScrollModel scrollModel)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator is null");
            }
            if (map == null)
            {
                throw new ArgumentNullException("map is null");
            }
            this.eventAggregator = eventAggregator;
            this.map = map;
            this.scrollModel = scrollModel;
        }

        #endregion constructors


        #region properties

        #endregion properties


        #region methods

        public DockViewModelTemplate CreateDock()
        {
            DockViewModelTemplate template;
            if (this.scrollModel != null)
            {
                template = new MapEditorViewModel(this.eventAggregator, this.map, this.scrollModel);
            }
            else
            {
                template = new MapEditorViewModel(this.eventAggregator, this.map);
            }
            return template;
        }

        public bool AppliesTo(Type type)
        {
            return typeof(MapEditorViewModel).Equals(type);
        }

        public void UpdateContent(Type type, object value)
        {
            if (typeof(IEventAggregator).Equals(type))
            {
                this.eventAggregator = value as IEventAggregator;
            }
            else if (typeof(Map).Equals(type))
            {
                this.map = value as Map;
            }
            else if (typeof(ScrollModel).Equals(type))
            {
                this.scrollModel = value as ScrollModel;
            }
        }

        #endregion methods
    }
}
