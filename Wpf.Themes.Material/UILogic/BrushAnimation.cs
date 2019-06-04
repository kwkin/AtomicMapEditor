using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Wpf.Themes.Material.UILogic
{
    public class BrushAnimation : AnimationTimeline
    {
        #region fields
        
        #endregion fields


        #region constructor

        protected override Freezable CreateInstanceCore()
        {
            return new BrushAnimation();
        }

        #endregion constructor


        #region methods

        public override object GetCurrentValue(object defaultOriginValue,
                                               object defaultDestinationValue,
                                               AnimationClock animationClock)
        {
            return GetCurrentValue(defaultOriginValue as Brush,
                                   defaultDestinationValue as Brush,
                                   animationClock);
        }

        public object GetCurrentValue(Brush defaultOriginValue,
                                      Brush defaultDestinationValue,
                                      AnimationClock animationClock)
        {
            if (!animationClock.CurrentProgress.HasValue)
            {
                return Brushes.Transparent;
            }

            defaultOriginValue = this.From ?? defaultOriginValue;
            if (animationClock.CurrentProgress.Value == 0)
            {
                return defaultOriginValue;
            }

            defaultDestinationValue = this.To ?? defaultDestinationValue;
            if (animationClock.CurrentProgress.Value == 1)
            {
                return defaultDestinationValue;
            }

            VisualBrush appliedBrush = this.CurrentBrush ?? createVisualBrush();
            (appliedBrush.Visual as Border).Background = defaultOriginValue;
            ((appliedBrush.Visual as Border).Child as Border).Background = defaultDestinationValue;
            ((appliedBrush.Visual as Border).Child as Border).Opacity = animationClock.CurrentProgress.Value;
            return appliedBrush;
        }

        public VisualBrush createVisualBrush()
        {
            CurrentBrush = new VisualBrush(new Border()
            {
                Width = 1,
                Height = 1,
                Child = new Border()
            });
            return CurrentBrush;
        }

        #endregion methods


        #region properties

        public Brush From
        {
            get
            {
                return (Brush)GetValue(FromProperty);
            }
            set
            {
                SetValue(FromProperty, value);
            }
        }

        public Brush To
        {
            get
            {
                return GetValue(ToProperty) as Brush;
            }
            set
            {
                SetValue(ToProperty, value);
            }
        }

        public VisualBrush CurrentBrush
        {
            get; set;
        }

        public override Type TargetPropertyType
        {
            get
            {
                return typeof(Brush);
            }
        }

        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(Brush), typeof(BrushAnimation));

        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(Brush), typeof(BrushAnimation));

        #endregion properties
    }
}
