﻿using System.Windows.Controls;

namespace Ame.Modules.Docks
{
    /// <summary>
    /// Interaction logic for DockManager.xaml
    /// </summary>
    public partial class DockManager : UserControl
    {
        public DockManager()
        {
            InitializeComponent();

            DockManagerViewModel viewModel = this.DataContext as DockManagerViewModel;
            viewModel.DockManager = this.dockManager;
        }
    }
}