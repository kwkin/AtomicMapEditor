using System;
using System.Collections.Generic;
using System.Linq;
using AtomicMapEditor.Infrastructure.BaseTypes;
using AtomicMapEditor.Infrastructure.Models;

namespace AtomicMapEditor.Modules.Docks.ItemEditorDock
{
    public class ItemEditorViewModel : DockViewModelTemplate
    {
        #region constructor & destructer

        public ItemEditorViewModel()
        {
            this.ZoomLevels = new List<ZoomLevel>();
            this.ZoomLevels.Add(new ZoomLevel("12.5%", 0.125));
            this.ZoomLevels.Add(new ZoomLevel("25%", 0.25));
            this.ZoomLevels.Add(new ZoomLevel("50%", 0.5));
            this.ZoomLevels.Add(new ZoomLevel("100%", 1));
            this.ZoomLevels.Add(new ZoomLevel("200%", 2));
            this.ZoomLevels.Add(new ZoomLevel("400%", 4));
            this.ZoomLevels.Add(new ZoomLevel("800%", 8));
            this.ZoomLevels.Add(new ZoomLevel("1600%", 16));
            this.ZoomLevels.Add(new ZoomLevel("3200%", 32));
            this.ZoomLevels = this.ZoomLevels.OrderBy(f => f.zoom).ToList();
            this.ZoomIndex = 3;
            this.Scale = ScaleType.Tile;
            this.PositionText = "0, 0";
        }

        #endregion constructor & destructer


        #region properties

        public ScaleType Scale { get; set; }
        public String PositionText { get; set; }
        public List<ZoomLevel> ZoomLevels { get; set; }
        private int _ZoomIndex;
        public int ZoomIndex
        {
            get { return _ZoomIndex; }
            set { SetProperty(ref _ZoomIndex, value); }
        }

        #endregion properties
    }
}
