using Ame.Infrastructure.Events.Messages;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Events
{
    public class CloseApplicationEvent : PubSubEvent<CloseApplicationMessage>
    {
    }
}
