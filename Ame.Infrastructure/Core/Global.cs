using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Core
{
    public static class Global
    {
        public static string Version
        {
            get
            {
                return @"0.0.1";
            }
        }

        public static string DefaultProjectFilename
        {
            get
            {
                return @".project";
            }
        }

        public static string DefaultFileDirectory
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
        }

        public static string AppMetaDataDirectory
        {
            get
            {
                string documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                return Path.Combine(documentPath, ApplicationName);
            }
        }

        public static string ApplicationName
        {
            get
            {
                return @"AtomicMapEditor";
            }
        }

        public static string SessionFileName
        {
            get
            {
                return Path.Combine(AppMetaDataDirectory, @"Session.config");
            }
        }

        public static string LayoutFileName
        {
            get
            {
                return Path.Combine(AppMetaDataDirectory, @"Layout.config");
            }
        }

        public static readonly double maxGridThickness = 0.5;
        public static readonly long defaultUpdatePositionLabelDelay = 30;
        public static readonly long defaultDrawSelectLineDelay = 100;
    }
}
