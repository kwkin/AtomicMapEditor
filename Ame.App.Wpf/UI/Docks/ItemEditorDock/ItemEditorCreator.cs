using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ame.Components.Behaviors;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Core;
using Ame.Infrastructure.Models;
using Prism.Events;

namespace Ame.App.Wpf.UI.Docks.ItemEditorDock
{
    public class ItemEditorCreator : DockCreatorTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;
        private IConstants constants;
        private AmeSession session;

        #endregion fields


        #region constructors

        public ItemEditorCreator(IEventAggregator eventAggregator, IConstants constants, AmeSession session)
            : this(eventAggregator, constants, session, null, null)
        {
        }

        public ItemEditorCreator(IEventAggregator eventAggregator, IConstants constants, AmeSession session, TilesetModel tilesetModel)
            : this(eventAggregator, constants, session, tilesetModel, null)
        {
        }

        public ItemEditorCreator(IEventAggregator eventAggregator, IConstants constants, AmeSession session, ScrollModel scrollModel)
            : this(eventAggregator, constants, session, null, scrollModel)
        {
        }

        public ItemEditorCreator(IEventAggregator eventAggregator, IConstants constants, AmeSession session, TilesetModel tilesetModel, ScrollModel scrollModel)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator is null");
            this.constants = constants ?? throw new ArgumentNullException("constants is null");
            this.session = session ?? throw new ArgumentNullException("session is null");

            this.TilesetModel = tilesetModel;
            this.ScrollModel = scrollModel;
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
                template = new ItemEditorViewModel(this.eventAggregator, this.constants, this.session, this.TilesetModel, this.ScrollModel);
            }
            else if (this.TilesetModel != null && this.ScrollModel == null)
            {
                template = new ItemEditorViewModel(this.eventAggregator, this.constants, this.session, this.TilesetModel);
            }
            else if (this.TilesetModel == null && this.ScrollModel != null)
            {
                template = new ItemEditorViewModel(this.eventAggregator, this.constants, this.session, this.ScrollModel);
            }
            else
            {
                template = new ItemEditorViewModel(this.eventAggregator, this.constants, this.session);
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
