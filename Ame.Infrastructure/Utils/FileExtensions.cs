using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Ame.Infrastructure.Attributes;

namespace Ame.Infrastructure.Utils
{
    public enum OpenFileExtensions
    {
        [FileExtensionAttribute("PNG", "*.png")]
        PNG,

        [FileExtensionAttribute("JPEG", "*.jpg", "*.jpeg")]
        JPEG,

        [FileExtensionAttribute("TIFF", "*.tif", "*.tiff")]
        TIFF
    }


    public enum ImageExtensions
    {
        PNG = OpenFileExtensions.PNG,
        JPEG = OpenFileExtensions.JPEG,
        TIFF = OpenFileExtensions.TIFF
    }


    public static class ImageExtension
    {
        #region methods

        public static string GetOpenFileImageExtensions()
        {
            StringBuilder openFileFormatBuilder = new StringBuilder();
            foreach (OpenFileExtensions extension in Enum.GetValues(typeof(ImageExtensions)))
            {
                openFileFormatBuilder.Append(extension.GetOpenFileFormat());
                openFileFormatBuilder.Append("|");
            }
            openFileFormatBuilder.Append("All Files (*.*)|*.*");

            return openFileFormatBuilder.ToString();
        }

        #endregion methods
    }

    public static class FileExtensionMethods
    {
        #region methods

        public static string GetNameAndExtensions(this OpenFileExtensions extension)
        {
            FileExtensionAttribute attr = GetAttr(extension);
            return string.Format("{0} ({1})", attr.Name, attr.Extensions);
        }

        public static string GetName(this OpenFileExtensions extension)
        {
            FileExtensionAttribute attr = GetAttr(extension);
            return string.Format("{0}", attr.Name);
        }

        public static string[] GetExtensions(this OpenFileExtensions extension)
        {
            FileExtensionAttribute attr = GetAttr(extension);
            return attr.Extensions;
        }

        public static string GetOpenFileFormat(this OpenFileExtensions extension)
        {
            FileExtensionAttribute attr = GetAttr(extension);
            return string.Format("{0} ({1})|{2}",
                attr.Name,
                string.Join(", ", attr.Extensions),
                string.Join(";", attr.Extensions));
        }

        private static FileExtensionAttribute GetAttr(OpenFileExtensions p)
        {
            return (FileExtensionAttribute)Attribute.GetCustomAttribute(ForValue(p), typeof(FileExtensionAttribute));
        }

        private static MemberInfo ForValue(OpenFileExtensions p)
        {
            return typeof(OpenFileExtensions).GetField(Enum.GetName(typeof(OpenFileExtensions), p));
        }

        #endregion methods
    }
}
