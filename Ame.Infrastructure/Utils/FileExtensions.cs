using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Ame.Infrastructure.Attributes;
using System.Windows.Media.Imaging;

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


    public enum ExportMapExtensions
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


    public static class ExportMapExtension
    {
        #region methods

        public static string GetOpenFileExportMapExtensions()
        {
            StringBuilder openFileFormatBuilder = new StringBuilder();
            foreach (OpenFileExtensions extension in Enum.GetValues(typeof(ExportMapExtensions)))
            {
                openFileFormatBuilder.Append(extension.GetOpenFileFormat());
                openFileFormatBuilder.Append("|");
            }
            openFileFormatBuilder.Append("All Files (*.*)|*.*");

            return openFileFormatBuilder.ToString();
        }

        public static BitmapEncoder getEncoder(string type)
        {
            BitmapEncoder encoder;
            switch (type)
            {
                case "PNG":
                    encoder = new PngBitmapEncoder();
                    break;
                case "JPEG":
                    encoder = new JpegBitmapEncoder();
                    break;
                case "TIFF":
                    encoder = new TiffBitmapEncoder();
                    break;
                default:
                    encoder = new PngBitmapEncoder();
                    break;
            }
            return encoder;
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
