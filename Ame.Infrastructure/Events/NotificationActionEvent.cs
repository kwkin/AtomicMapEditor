﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ame.Infrastructure.Events.Messages;
using Prism.Events;

namespace Ame.Infrastructure.Events
{
    public class NotificationActionEvent<TCallbackParameter> : PubSubEvent<NotificationActionMessage<TCallbackParameter>>
    {
    }
}
