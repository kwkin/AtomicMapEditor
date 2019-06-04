using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ame.Components.Behaviors;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.BaseTypes;
using Prism.Events;

namespace Ame.App.Wpf.UI.Docks.ItemEditorDock
{
    public class ItemEditorCreator : DockCreatorTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;
        private AmeSession session;

        #endregion fields


        #region constructors

        public ItemEditorCreator(IEventAggregator eventAggregator, AmeSession session)
            : this(eventAggregator, session, null, null)
        {
        }

        public ItemEditorCreator(IEventAggregator eventAggregator, AmeSession session, TilesetModel tilesetModel)
            : this(eventAggregator, session, tilesetModel, null)
        {
        }

        public ItemEditorCreator(IEventAggregator eventAggregator, AmeSession session, ScrollModel scrollModel)
            : this(eventAggregator, session, null, scrollModel)
        {
        }

        public ItemEditorCreator(IEventAggregator eventAggregator, AmeSession session, TilesetModel tilesetModel, ScrollModel scrollModel)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator is null");
            }
            if (session == null)
            {
                throw new ArgumentNullException("session is null");
            }
            this.eventAggregator = eventAggregator;
            this.session = session;
            this.ScrollModel = scrollModel;
            this.TilesetModel = tilesetModel;
        }

        #endregion constructors


        #region properties

        public ScrollModel ScrollModel { get; set; }
        public TilesetModel TilesetModel { get; set; }

        #endregion properties


        #region methods

        public override DockViewModelTemplate CreateDock()
        {
            DockViewModelTemplate template;
            if (this.TilesetModel != null && this.ScrollModel != null)
            {
                template = new ItemEditorViewModel(this.eventAggregator, this.session, this.TilesetModel, this.ScrollModel);
            }
            else if (this.TilesetModel != null && this.ScrollModel == null)
            {
                template = new ItemEditorViewModel(this.eventAggregator, this.session, this.TilesetModel);
            }
            else if (this.TilesetModel == null && this.ScrollModel != null)
            {
                template = new ItemEditorViewModel(this.eventAggregator, this.session, this.ScrollModel);
            }
            else
            {
                template = new ItemEditorViewModel(this.eventAggregator, this.session);
            }
            return template;
        }

        public override bool AppliesTo(Type type)
        {
            return typeof(ItemEditorViewModel).Equals(type);
        }

        #endregion methods
    }
}
