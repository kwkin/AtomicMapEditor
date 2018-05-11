﻿using System;
using Ame.Components.Behaviors;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Ame.Modules.Docks;
using Microsoft.Practices.Unity;
using Prism.Events;

namespace Ame.Modules.MapEditor.Editor
{
    public class MapEditorCreator : IDockCreator
    {
        #region fields

        #endregion fields


        #region constructors

        public MapEditorCreator(IEventAggregator eventAggregator, IScrollModel scrollModel, Map map)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator is null");
            }
            if (scrollModel == null)
            {
                throw new ArgumentNullException("scrollModel is null");
            }
            this.Container = new UnityContainer();
            this.Container.RegisterInstance<IEventAggregator>(eventAggregator);
            this.Container.RegisterInstance<IScrollModel>(scrollModel);
            this.Container.RegisterInstance<Map>(map);
        }

        #endregion constructors


        #region properties

        public IUnityContainer Container { get; set; }

        #endregion properties


        #region methods

        public DockViewModelTemplate CreateDock()
        {
            return this.Container.Resolve<MapEditorViewModel>();
        }

        public bool AppliesTo(Type type)
        {
            return typeof(MapEditorViewModel).Equals(type);
        }

        #endregion methods
    }
}
