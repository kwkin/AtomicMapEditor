﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Ame.Infrastructure.Models;

namespace Ame.Infrastructure.DrawingTools
{
    public class DrawAction
    {
        #region fields

        #endregion fields


        #region constructor

        public DrawAction(string name, Stack<Tile> tiles)
        {
            this.Name = name;
            this.Tiles = tiles;
        }

        #endregion constructor


        #region properties

        public string Name { get; set; }
        public Stack<Tile> Tiles { get; set; }

        #endregion properties


        #region methods

        #endregion methods
    }
}
