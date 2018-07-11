using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Ame.Infrastructure.BaseTypes;

namespace Ame.Infrastructure.Models.DrawingBrushes
{
    public class TileDrawer
    {
        #region fields

        private List<IBrushCommand> commands;
        private int current = 0;

        #endregion fields


        #region constructor

        public TileDrawer(DrawingGroup drawingGroup)
        {
            this.commands = new List<IBrushCommand>();
            this.DrawingGroup = drawingGroup;
        }

        #endregion constructor


        #region properties

        private DrawingGroup DrawingGroup { get; set; }

        #endregion properties


        #region methods

        public void Draw(IBrush brush, Point point)
        {
            IBrushCommand command = new BrushCommand(this.DrawingGroup, brush, point);
            command.Execute();

            this.commands.Add(command);
            this.current++;
        }

        public void Undo()
        {
            UndoLevels(1);
        }

        public void UndoLevels(int levels)
        {
            for (int level = 0; level < levels; ++level)
            {
                if (current < commands.Count - 1)
                {
                    IBrushCommand command = this.commands[this.current++];
                    command.Execute();
                }
            }
        }

        public void Redo()
        {
            RedoLevels(1);
        }

        public void RedoLevels(int levels)
        {
            for (int level = 0; level < levels; ++level)
            {
                if (current < commands.Count - 1)
                {
                    IBrushCommand command = this.commands[--this.current];
                    command.UnExecute();
                }
            }
        }

        #endregion methods
    }
}
