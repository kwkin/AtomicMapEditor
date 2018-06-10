using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Ame.Infrastructure.BaseTypes;
using Microsoft.Practices.Unity;

namespace Ame.Infrastructure.Messages
{
    public class OpenWindowMessage
    {
        #region fields

        #endregion fields


        #region Constructor

        public OpenWindowMessage(Type type)
        {
            if (!typeof(IWindowInteraction).IsAssignableFrom(type))
            {
                return;
            }
            this.Type = type;
            this.Title = "";
        }

        public OpenWindowMessage(Type type, object content)
        {
            if (!typeof(IWindowInteraction).IsAssignableFrom(type))
            {
                return;
            }
            this.Type = type;
            this.Content = content;
            this.Title = "";
        }

        public OpenWindowMessage(Type type, string title)
        {
            if (!typeof(IWindowInteraction).IsAssignableFrom(type))
            {
                return;
            }
            this.Type = type;
            this.Title = title;
        }

        public OpenWindowMessage(Type type, object content, string title)
        {
            if (!typeof(IWindowInteraction).IsAssignableFrom(type))
            {
                return;
            }
            this.Type = type;
            this.Content = content;
            this.Title = title;
        }

        #endregion Constructor


        #region Properties

        public Type Type;
        public string Title { get; set; }
        public object Content { get; set; }

        #endregion Properties


        #region methods


        #endregion methods
    }
}
