using Ame.Components.Behaviors;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.App.Wpf.UI.Editor.MapEditor
{
    public class MapEditorCreator : DockCreatorTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;
        private AmeSession session;

        #endregion fields


        #region constructors

        public MapEditorCreator(IEventAggregator eventAggregator, AmeSession session)
            : this(eventAggregator, session, null, null)
        {
        }

        public MapEditorCreator(IEventAggregator eventAggregator, AmeSession session, Map map, ScrollModel scrollModel)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator is null");
            this.session = session ?? throw new ArgumentNullException("session is null");

            this.ScrollModel = scrollModel;
            string mapTitle = string.Format("Map #{0}", session.MapCount);
            this.Map = map ?? new Map(mapTitle);

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
                template = new MapEditorViewModel(this.eventAggregator, this.session, this.Map, this.ScrollModel);
            }
            else
            {
                template = new MapEditorViewModel(this.eventAggregator, this.session, this.Map);
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
