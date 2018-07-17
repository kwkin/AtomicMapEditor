﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ame.Infrastructure.Messages;
using Ame.Infrastructure.Models;
using Prism.Events;

namespace Ame.Infrastructure.Events
{
    public class UpdateBrushEvent : PubSubEvent<BrushModel>
    {
    }
}
