using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Core
{
    public class Constants : IConstants
    {
        public string Version
        {
            get
            {
                return @"0.0.1";
            }
        }

        public string DefaultProjectFilename
        {
            get
            {
                return @".project";
            }
        }

        public string DefaultWorkspaceDirectory
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
        }

        public string AppMetaDataDirectory
        {
            get
            {
                string documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                return Path.Combine(documentPath, ApplicationName);
            }
        }

        public string ApplicationName
        {
            get
            {
                return @"AtomicMapEditor";
            }
        }

        public string SessionFileName
        {
            get
            {
                return Path.Combine(this.AppMetaDataDirectory, @"Session.config");
            }
        }

        public string LayoutFileName
        {
            get
            {
                return Path.Combine(this.AppMetaDataDirectory, @"Layout.config");
            }
        }

        public double MaxGridThickness
        {
            get
            {
                return 0.5;
            }
        }

        public long DefaultUpdatePositionLabelMsDelay
        {
            get
            {
                return 30;
            }
        }

        public long DefaultUpdateSelectLineMsDelay
        {
            get
            {
                return 100;
            }
        }

        public long DefaultUpdateTransparentColorMsDelay
        {
            get
            {
                return 50;
            }
        }
    }
}
