using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ame.Infrastructure.BaseTypes;

namespace Ame.Infrastructure.Events.Messages
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
        public bool IgnoreIfExists { get; set; }

        #endregion Properties


        #region methods


        #endregion methods
    }
}
