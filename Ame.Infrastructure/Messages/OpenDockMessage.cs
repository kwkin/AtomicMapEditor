using System;
using Ame.Infrastructure.BaseTypes;
using Microsoft.Practices.Unity;

namespace Ame.Infrastructure.Messages
{
    public class OpenDockMessage
    {
        #region fields

        #endregion fields


        #region Constructor
        
        public OpenDockMessage(Type type)
        {
            if (!typeof(DockViewModelTemplate).IsAssignableFrom(type))
            {
                return;
            }
            this.Type = type;
            this.Title = "";
        }

        public OpenDockMessage(Type type, object content)
        {
            if (!typeof(DockViewModelTemplate).IsAssignableFrom(type))
            {
                return;
            }
            this.Type = type;
            this.Content = content;
            this.Title = "";
        }

        public OpenDockMessage(Type type, string title)
        {
            if (!typeof(DockViewModelTemplate).IsAssignableFrom(type))
            {
                return;
            }
            this.Type = type;
            this.Title = title;
        }

        public OpenDockMessage(Type type, object content, string title)
        {
            if (!typeof(DockViewModelTemplate).IsAssignableFrom(type))
            {
                return;
            }
            this.Type = type;
            this.Content = content;
            this.Title = title;
        }

        #endregion Constructor


        #region Properties

        public Type Type { get; set; }
        public object Content { get; set; }
        public string Title { get; set; }

        #endregion Properties


        #region methods


        #endregion methods
    }
}
