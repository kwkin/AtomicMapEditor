﻿namespace Ame.Modules.Windows
{
    public interface ILayoutViewModel
    {
        bool IsBusy { get; set; }
        string AppDataDirectory { get; }
    }
}