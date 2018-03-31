using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtomicMapEditor.Infrastructure.Events
{
    public class NotificationMessage<T>
    {
        #region fields

        #endregion fields


        #region Constructor & destructer

        public NotificationMessage(string notification)
        {
            this.Notification = notification;
        }

        public NotificationMessage(T content, string notification)
        {
            this.Content = content;
            this.Notification = notification;
        }
        
        #endregion Constructor & destructer


        #region Properties

        public string Notification
        {
            get;
            private set;
        }

        public T Content
        {
            get;
            private set;
        }

        #endregion Properties


        #region methods

        #endregion methods
    }
}
