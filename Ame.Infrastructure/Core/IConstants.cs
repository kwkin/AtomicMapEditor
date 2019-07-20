using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Core
{
    public interface IConstants
    {
        #region properties

        string Version{ get; }
        string DefaultProjectFilename{ get; }
        string DefaultFileDirectory{ get; }
        string AppMetaDataDirectory{ get; }
        string ApplicationName{ get; }
        string SessionFileName{ get; }
        string LayoutFileName{ get; }
        double MaxGridThickness{ get; }
        long DefaultUpdatePositionLabelMsDelay{ get; }
        long DefaultUpdateSelectLineMsDelay { get; }
        long DefaultUpdateTransparentColorMsDelay { get; }

        #endregion properties
    }
}
