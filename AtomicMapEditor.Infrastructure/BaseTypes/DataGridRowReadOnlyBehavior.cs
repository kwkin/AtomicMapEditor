﻿using System;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Ame.Infrastructure.BaseTypes
{
    public class DataGridRowReadOnlyBehavior : Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            if (this.AssociatedObject == null)
            {
                throw new InvalidOperationException("AssociatedObject must not be null");
            }
            AssociatedObject.BeginningEdit += AssociatedObject_BeginningEdit;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.BeginningEdit -= AssociatedObject_BeginningEdit;
        }

        private void AssociatedObject_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            var isReadOnlyRow = ReadOnlyService.GetIsReadOnly(e.Row);
            if (isReadOnlyRow)
            {
                e.Cancel = true;
            }
        }
    }
}
