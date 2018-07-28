using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Exceptions
{
    public class InteractionConfigurationException : Exception
    {
        public InteractionConfigurationException()
        {
        }

        public InteractionConfigurationException(string message)
            : base(message)
        {
        }

        public InteractionConfigurationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
