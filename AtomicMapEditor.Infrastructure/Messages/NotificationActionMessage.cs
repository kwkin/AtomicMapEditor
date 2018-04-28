using System;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Messages
{
    public class NotificationActionMessage<TCallbackParameter>
    {
        #region fields

        #endregion fields


        #region Constructor

        public NotificationActionMessage(string notification, Action<TCallbackParameter> callback)
        {
            this.Callback = callback;
            this.Notification = notification;
        }

        #endregion Constructor


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
