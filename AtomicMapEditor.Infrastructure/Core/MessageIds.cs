using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Core
{
    public class MessageIds
    {
        public static readonly string SaveWorkspaceLayout = Guid.NewGuid().ToString();
        public static readonly string LoadWorkspaceLayout = Guid.NewGuid().ToString();
    }
}
