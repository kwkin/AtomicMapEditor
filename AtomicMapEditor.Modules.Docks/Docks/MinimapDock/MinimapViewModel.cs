using System;
using System.Windows.Input;
using Ame.Infrastructure.BaseTypes;
using Prism.Commands;
using Prism.Events;

namespace Ame.Modules.Windows.Docks.MinimapDock
{
    [DockContentId("Minimap")]
    public class MinimapViewModel : DockToolViewModelTemplate
    {
        #region fields

        #endregion fields

        #region constructor

        public MinimapViewModel(IEventAggregator eventAggregator)
        {
            this.Title = "Minimap";

            this.FitMinimapCommand = new DelegateCommand(() => FitMinimap());
            this.ToggleGridCommand = new DelegateCommand(() => ToggleGrid());
            this.ToggleCollisionCommand = new DelegateCommand(() => ToggleCollision());
            this.CenterOnPointCommand = new DelegateCommand(() => CenterOnPoint());
        }

        #endregion constructor


        #region properties

        public ICommand FitMinimapCommand { get; private set; }
        public ICommand ToggleGridCommand { get; private set; }
        public ICommand ToggleCollisionCommand { get; private set; }
        public ICommand CenterOnPointCommand { get; private set; }
        
        #endregion properties


        #region methods

        private void FitMinimap()
        {
            Console.WriteLine("Fit minimap");
        }

        private void ToggleGrid()
        {
            Console.WriteLine("Toggle grid minimap");
        }

        private void ToggleCollision()
        {
            Console.WriteLine("Toggle collision minimap");
        }

        private void CenterOnPoint()
        {
            Console.WriteLine("Center On Point");
        }

        #endregion methods
    }
}
