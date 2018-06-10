using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ame.Infrastructure.Models;

namespace Ame.Infrastructure.Messages
{
    public class UpdateBrushMessage
    {
        #region fields

        #endregion fields


        #region Constructor

        public UpdateBrushMessage(BrushModel brushModel)
        {
            this.BrushModel = brushModel;
        }

        #endregion Constructor


        #region Properties

        public BrushModel BrushModel { get; set; }

        #endregion Properties


        #region methods


        #endregion methods
    }
}
