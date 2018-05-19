using System;
using Ame.Components.Behaviors;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Prism.Events;

namespace Ame.Modules.Windows.Docks.ItemEditorDock
{
    public class ItemEditorCreator : DockCreatorTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;

        #endregion fields


        #region constructors

        public ItemEditorCreator(IEventAggregator eventAggregator) : this(eventAggregator, null, null)
        {
        }

        public ItemEditorCreator(IEventAggregator eventAggregator, TilesetModel tilesetModel) : this(eventAggregator, tilesetModel, null)
        {
        }

        public ItemEditorCreator(IEventAggregator eventAggregator, ScrollModel scrollModel) : this(eventAggregator, null, scrollModel)
        {
        }

        public ItemEditorCreator(IEventAggregator eventAggregator, TilesetModel tilesetModel, ScrollModel scrollModel)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator is null");
            }
            this.eventAggregator = eventAggregator;
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
                template = new ItemEditorViewModel(this.eventAggregator, this.TilesetModel, this.ScrollModel);
            }
            else if (this.TilesetModel != null && this.ScrollModel == null)
            {
                template = new ItemEditorViewModel(this.eventAggregator, this.TilesetModel);
            }
            else if (this.TilesetModel == null && this.ScrollModel != null)
            {
                template = new ItemEditorViewModel(this.eventAggregator, this.ScrollModel);
            }
            else
            {
                template = new ItemEditorViewModel(this.eventAggregator);
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
