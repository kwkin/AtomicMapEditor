﻿using Ame.Infrastructure.Models;

namespace Ame.Infrastructure.Events
{
    public class UpdateBrushMessage
    {
        #region fields

        #endregion fields


        #region Constructor & destructer

        public UpdateBrushMessage(BrushModel brushModel)
        {
            this.BrushModel = brushModel;
        }

        #endregion Constructor & destructer


        #region Properties

        public BrushModel BrushModel { get; set; }

        #endregion Properties


        #region methods


        #endregion methods
    }
}
