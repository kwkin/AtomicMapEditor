using System;
using Ame.Components.Behaviors;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Prism.Events;

namespace Ame.Modules.Windows.Docks.ItemEditorDock
{
    public class ItemEditorCreator : IDockCreator
    {
        #region fields

        private IEventAggregator eventAggregator;
        private ScrollModel scrollModel;
        private TilesetModel tilesetModel;

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
            this.scrollModel = scrollModel;
            this.tilesetModel = tilesetModel;
        }

        #endregion constructors


        #region properties

        #endregion properties


        #region methods

        public DockViewModelTemplate CreateDock()
        {
            DockViewModelTemplate template;
            if (this.tilesetModel != null && this.scrollModel != null)
            {
                template = new ItemEditorViewModel(this.eventAggregator, this.tilesetModel, this.scrollModel);
            }
            else if (this.tilesetModel != null && this.scrollModel == null)
            {
                template = new ItemEditorViewModel(this.eventAggregator, this.tilesetModel);
            }
            else if (this.tilesetModel == null && this.scrollModel != null)
            {
                template = new ItemEditorViewModel(this.eventAggregator, this.scrollModel);
            }
            else
            {
                template = new ItemEditorViewModel(this.eventAggregator);
            }
            return template;
        }

        public bool AppliesTo(Type type)
        {
            return typeof(ItemEditorViewModel).Equals(type);
        }

        public void UpdateContent(Type type, object value)
        {
            if (typeof(IEventAggregator).Equals(type))
            {
                this.eventAggregator = value as IEventAggregator;
            }
            else if (typeof(TilesetModel).Equals(type))
            {
                this.tilesetModel = value as TilesetModel;
            }
            else if (typeof(ScrollModel).Equals(type))
            {
                this.scrollModel = value as ScrollModel;
            }
        }

        #endregion methods
    }
}
