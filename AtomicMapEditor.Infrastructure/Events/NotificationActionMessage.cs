using System;
using System.Threading.Tasks;

namespace AtomicMapEditor.Infrastructure.Events
{
    public class NotificationActionMessage<TCallbackParameter>
    {
        #region fields

        #endregion fields


        #region Constructor & destructer

        public NotificationActionMessage(string notification, Action<TCallbackParameter> callback)
        {
            this.Callback = callback;
            this.Notification = notification;
        }

        #endregion Constructor & destructer


        #region Properties

        public string Notification
        {
            get;
            private set;
        }

        public Action<TCallbackParameter> Callback
        {
            get;
            private set;
        }

        #endregion Properties


        #region methods

        public void Execute(params object[] arguments)
        {
            this.Callback.DynamicInvoke(arguments);
        }

        #endregion methods
    }
}
