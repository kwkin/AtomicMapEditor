using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ame.Components.Extended
{
    public class IconRadioButton : RadioButton
    {
        #region fields

        #endregion fields


        #region constructor & destructer

        public IconRadioButton()
        {
            this.Background = System.Windows.Media.Brushes.Transparent;
            this.BorderBrush = Brushes.Transparent;
            this.ImageStretch = Stretch.None;
        }

        #endregion constructor & destructer


        #region properties

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register(
                "ImageSourceProperty",
                typeof(ImageSource),
                typeof(IconRadioButton),
                new PropertyMetadata(default(ImageSource)));

        public ImageSource ImageSource
        {
            get
            {
                return GetValue(ImageSourceProperty) as ImageSource;
            }
            set
            {
                SetValue(ImageSourceProperty, value);
                Image newImage = new Image();
                newImage.Source = value;
                this.Content = newImage;
            }
        }

        public static readonly DependencyProperty ImageStretchProperty =
            DependencyProperty.Register(
                "ImageStretchProperty",
                typeof(Stretch),
                typeof(IconRadioButton),
                new PropertyMetadata(default(Stretch)));

        public Stretch ImageStretch
        {
            get
            {
                return (Stretch)GetValue(ImageStretchProperty);
            }
            set
            {
                SetValue(ImageStretchProperty, value);
                if (this.Content != null)
                {
                    Image imageContent = this.Content as Image;
                    imageContent.Stretch = value;
                }
            }
        }

        #endregion properties


        #region methods

        #endregion methods
    }
}
