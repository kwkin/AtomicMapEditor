﻿using Ame.Infrastructure.Messages;
using Prism.Events;

namespace Ame.Infrastructure.Events
{
    public class OpenDockEvent : PubSubEvent<OpenDockMessage>
    {
    }
}