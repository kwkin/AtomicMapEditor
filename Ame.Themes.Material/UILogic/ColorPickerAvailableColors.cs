using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace Ame.Themes.Material.UILogic
{
    public class ColorPickerAvailableColors
    {
        public ColorPickerAvailableColors()
        {
            AvailableColors = new ObservableCollection<ColorItem>();

            // Red
            AvailableColors.Add(CreateColor("#FFFFEBEE"));
            AvailableColors.Add(CreateColor("#FFFFCDD2"));
            AvailableColors.Add(CreateColor("#FFEF9A9A"));
            AvailableColors.Add(CreateColor("#FFE57373"));
            AvailableColors.Add(CreateColor("#FFEF5350"));
            AvailableColors.Add(CreateColor("#FFF44336"));
            AvailableColors.Add(CreateColor("#FFE53935"));
            AvailableColors.Add(CreateColor("#FFD32F2F"));
            AvailableColors.Add(CreateColor("#FFC62828"));
            AvailableColors.Add(CreateColor("#FFB71C1C"));

            // Pink
            AvailableColors.Add(CreateColor("#FFFCE4EC"));
            AvailableColors.Add(CreateColor("#FFF8BBD0"));
            AvailableColors.Add(CreateColor("#FFF48FB1"));
            AvailableColors.Add(CreateColor("#FFF06292"));
            AvailableColors.Add(CreateColor("#FFEC407A"));
            AvailableColors.Add(CreateColor("#FFE91E63"));
            AvailableColors.Add(CreateColor("#FFD81B60"));
            AvailableColors.Add(CreateColor("#FFC2185B"));
            AvailableColors.Add(CreateColor("#FFAD1457"));
            AvailableColors.Add(CreateColor("#FF880E47"));

            // Purple
            AvailableColors.Add(CreateColor("#FFF3E5F5"));
            AvailableColors.Add(CreateColor("#FFE1BEE7"));
            AvailableColors.Add(CreateColor("#FFCE93D8"));
            AvailableColors.Add(CreateColor("#FFBA68C8"));
            AvailableColors.Add(CreateColor("#FFAB47BC"));
            AvailableColors.Add(CreateColor("#FF9C27B0"));
            AvailableColors.Add(CreateColor("#FF8E24AA"));
            AvailableColors.Add(CreateColor("#FF7B1FA2"));
            AvailableColors.Add(CreateColor("#FF6A1B9A"));
            AvailableColors.Add(CreateColor("#FF4A148C"));

            // Deep Purple
            AvailableColors.Add(CreateColor("#FFEDE7F6"));
            AvailableColors.Add(CreateColor("#FFD1C4E9"));
            AvailableColors.Add(CreateColor("#FFB39DDB"));
            AvailableColors.Add(CreateColor("#FF9575CD"));
            AvailableColors.Add(CreateColor("#FF7E57C2"));
            AvailableColors.Add(CreateColor("#FF673AB7"));
            AvailableColors.Add(CreateColor("#FF5E35B1"));
            AvailableColors.Add(CreateColor("#FF512DA8"));
            AvailableColors.Add(CreateColor("#FF4527A0"));
            AvailableColors.Add(CreateColor("#FF311B92"));

            // Indigo
            AvailableColors.Add(CreateColor("#FFE8EAF6"));
            AvailableColors.Add(CreateColor("#FFC5CAE9"));
            AvailableColors.Add(CreateColor("#FF9FA8DA"));
            AvailableColors.Add(CreateColor("#FF7986CB"));
            AvailableColors.Add(CreateColor("#FF5C6BC0"));
            AvailableColors.Add(CreateColor("#FF3F51B5"));
            AvailableColors.Add(CreateColor("#FF3949AB"));
            AvailableColors.Add(CreateColor("#FF303F9F"));
            AvailableColors.Add(CreateColor("#FF283593"));
            AvailableColors.Add(CreateColor("#FF1A237E"));

            // Blue
            AvailableColors.Add(CreateColor("#FFE3F2FD"));
            AvailableColors.Add(CreateColor("#FFBBDEFB"));
            AvailableColors.Add(CreateColor("#FF90CAF9"));
            AvailableColors.Add(CreateColor("#FF64B5F6"));
            AvailableColors.Add(CreateColor("#FF42A5F5"));
            AvailableColors.Add(CreateColor("#FF2196F3"));
            AvailableColors.Add(CreateColor("#FF1E88E5"));
            AvailableColors.Add(CreateColor("#FF1976D2"));
            AvailableColors.Add(CreateColor("#FF1565C0"));
            AvailableColors.Add(CreateColor("#FF0D47A1"));

            // Light Blue
            AvailableColors.Add(CreateColor("#FFE1F5FE"));
            AvailableColors.Add(CreateColor("#FFB3E5FC"));
            AvailableColors.Add(CreateColor("#FF81D4FA"));
            AvailableColors.Add(CreateColor("#FF4FC3F7"));
            AvailableColors.Add(CreateColor("#FF29B6F6"));
            AvailableColors.Add(CreateColor("#FF03A9F4"));
            AvailableColors.Add(CreateColor("#FF039BE5"));
            AvailableColors.Add(CreateColor("#FF0288D1"));
            AvailableColors.Add(CreateColor("#FF0277BD"));
            AvailableColors.Add(CreateColor("#FF01579B"));

            // Cyan
            AvailableColors.Add(CreateColor("#FFE0F7FA"));
            AvailableColors.Add(CreateColor("#FFB2EBF2"));
            AvailableColors.Add(CreateColor("#FF80DEEA"));
            AvailableColors.Add(CreateColor("#FF4DD0E1"));
            AvailableColors.Add(CreateColor("#FF26C6DA"));
            AvailableColors.Add(CreateColor("#FF00BCD4"));
            AvailableColors.Add(CreateColor("#FF00ACC1"));
            AvailableColors.Add(CreateColor("#FF0097A7"));
            AvailableColors.Add(CreateColor("#FF00838F"));
            AvailableColors.Add(CreateColor("#FF006064"));

            // Teal
            AvailableColors.Add(CreateColor("#FFE0F2F1"));
            AvailableColors.Add(CreateColor("#FFB2DFDB"));
            AvailableColors.Add(CreateColor("#FF80CBC4"));
            AvailableColors.Add(CreateColor("#FF4DB6AC"));
            AvailableColors.Add(CreateColor("#FF26A69A"));
            AvailableColors.Add(CreateColor("#FF009688"));
            AvailableColors.Add(CreateColor("#FF00897B"));
            AvailableColors.Add(CreateColor("#FF00796B"));
            AvailableColors.Add(CreateColor("#FF00695C"));
            AvailableColors.Add(CreateColor("#FF004D40"));

            // Green
            AvailableColors.Add(CreateColor("#FFE8F5E9"));
            AvailableColors.Add(CreateColor("#FFC8E6C9"));
            AvailableColors.Add(CreateColor("#FFA5D6A7"));
            AvailableColors.Add(CreateColor("#FF81C784"));
            AvailableColors.Add(CreateColor("#FF66BB6A"));
            AvailableColors.Add(CreateColor("#FF4CAF50"));
            AvailableColors.Add(CreateColor("#FF43A047"));
            AvailableColors.Add(CreateColor("#FF388E3C"));
            AvailableColors.Add(CreateColor("#FF2E7D32"));
            AvailableColors.Add(CreateColor("#FF1B5E20"));

            // Light Green
            AvailableColors.Add(CreateColor("#FFF1F8E9"));
            AvailableColors.Add(CreateColor("#FFDCEDC8"));
            AvailableColors.Add(CreateColor("#FFC5E1A5"));
            AvailableColors.Add(CreateColor("#FFAED581"));
            AvailableColors.Add(CreateColor("#FF9CCC65"));
            AvailableColors.Add(CreateColor("#FF8BC34A"));
            AvailableColors.Add(CreateColor("#FF7CB342"));
            AvailableColors.Add(CreateColor("#FF689F38"));
            AvailableColors.Add(CreateColor("#FF558B2F"));
            AvailableColors.Add(CreateColor("#FF33691E"));

            // Lime
            AvailableColors.Add(CreateColor("#FFF9FBE7"));
            AvailableColors.Add(CreateColor("#FFF0F4C3"));
            AvailableColors.Add(CreateColor("#FFE6EE9C"));
            AvailableColors.Add(CreateColor("#FFDCE775"));
            AvailableColors.Add(CreateColor("#FFD4E157"));
            AvailableColors.Add(CreateColor("#FFCDDC39"));
            AvailableColors.Add(CreateColor("#FFC0CA33"));
            AvailableColors.Add(CreateColor("#FFAFB42B"));
            AvailableColors.Add(CreateColor("#FF9E9D24"));
            AvailableColors.Add(CreateColor("#FF827717"));

            // Yellow
            AvailableColors.Add(CreateColor("#FFFFFDE7"));
            AvailableColors.Add(CreateColor("#FFFFF9C4"));
            AvailableColors.Add(CreateColor("#FFFFF59D"));
            AvailableColors.Add(CreateColor("#FFFFF176"));
            AvailableColors.Add(CreateColor("#FFFFEE58"));
            AvailableColors.Add(CreateColor("#FFFFEB3B"));
            AvailableColors.Add(CreateColor("#FFFDD835"));
            AvailableColors.Add(CreateColor("#FFFBC02D"));
            AvailableColors.Add(CreateColor("#FFF9A825"));
            AvailableColors.Add(CreateColor("#FFF57F17"));

            // Amber
            AvailableColors.Add(CreateColor("#FFFFF8E1"));
            AvailableColors.Add(CreateColor("#FFFFECB3"));
            AvailableColors.Add(CreateColor("#FFFFE082"));
            AvailableColors.Add(CreateColor("#FFFFD54F"));
            AvailableColors.Add(CreateColor("#FFFFCA28"));
            AvailableColors.Add(CreateColor("#FFFFC107"));
            AvailableColors.Add(CreateColor("#FFFFB300"));
            AvailableColors.Add(CreateColor("#FFFFA000"));
            AvailableColors.Add(CreateColor("#FFFF8F00"));
            AvailableColors.Add(CreateColor("#FFFF6F00"));

            // Orange
            AvailableColors.Add(CreateColor("#FFFFF3E0"));
            AvailableColors.Add(CreateColor("#FFFFE0B2"));
            AvailableColors.Add(CreateColor("#FFFFCC80"));
            AvailableColors.Add(CreateColor("#FFFFB74D"));
            AvailableColors.Add(CreateColor("#FFFFA726"));
            AvailableColors.Add(CreateColor("#FFFF9800"));
            AvailableColors.Add(CreateColor("#FFFB8C00"));
            AvailableColors.Add(CreateColor("#FFF57C00"));
            AvailableColors.Add(CreateColor("#FFEF6C00"));
            AvailableColors.Add(CreateColor("#FFE65100"));

            // Deep Orange
            AvailableColors.Add(CreateColor("#FFFBE9E7"));
            AvailableColors.Add(CreateColor("#FFFFCCBC"));
            AvailableColors.Add(CreateColor("#FFFFAB91"));
            AvailableColors.Add(CreateColor("#FFFF8A65"));
            AvailableColors.Add(CreateColor("#FFFF7043"));
            AvailableColors.Add(CreateColor("#FFFF5722"));
            AvailableColors.Add(CreateColor("#FFF4511E"));
            AvailableColors.Add(CreateColor("#FFE64A19"));
            AvailableColors.Add(CreateColor("#FFD84315"));
            AvailableColors.Add(CreateColor("#FFBF360C"));

            // Brown
            AvailableColors.Add(CreateColor("#FFEFEBE9"));
            AvailableColors.Add(CreateColor("#FFD7CCC8"));
            AvailableColors.Add(CreateColor("#FFBCAAA4"));
            AvailableColors.Add(CreateColor("#FFA1887F"));
            AvailableColors.Add(CreateColor("#FF8D6E63"));
            AvailableColors.Add(CreateColor("#FF795548"));
            AvailableColors.Add(CreateColor("#FF6D4C41"));
            AvailableColors.Add(CreateColor("#FF5D4037"));
            AvailableColors.Add(CreateColor("#FF4E342E"));
            AvailableColors.Add(CreateColor("#FF3E2723"));
        }

        public static ColorItem CreateColor(string value)
        {
            return CreateColor(value, value);
        }

        public static ColorItem CreateColor(string value, string name)
        {
            LastColor = (Color)ColorConverter.ConvertFromString(value);
            LastColorItem = new ColorItem(LastColor, value);
            return LastColorItem;
        }


        public static Color LastColor { get; set; }
        public static ColorItem LastColorItem { get; set; }

        public ObservableCollection<ColorItem> AvailableColors { get; set; }
    }
}
