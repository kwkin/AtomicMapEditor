using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace Ame.Themes.Material.UILogic
{
    public class DropShadowAnimation : AnimationTimeline
    {
        #region fields

        #endregion fields


        #region constructor

        protected override Freezable CreateInstanceCore()
        {
            return new DropShadowAnimation();
        }

        #endregion constructor


        #region methods

        public override object GetCurrentValue(object defaultOriginValue,
                                               object defaultDestinationValue,
                                               AnimationClock animationClock)
        {
            return GetCurrentValue(defaultOriginValue as DropShadowEffect,
                                   defaultDestinationValue as DropShadowEffect,
                                   animationClock);
        }

        public object GetCurrentValue(DropShadowEffect defaultOriginValue,
                                      DropShadowEffect defaultDestinationValue,
                                      AnimationClock animationClock)
        {
            if (!animationClock.CurrentProgress.HasValue)
            {
                return new DropShadowEffect();
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
            
            double interpolation = animationClock.CurrentProgress.Value;
            double blurRadius = (defaultDestinationValue.BlurRadius - defaultOriginValue.BlurRadius) * interpolation + defaultOriginValue.BlurRadius;
            double direction = (defaultDestinationValue.Direction - defaultOriginValue.Direction) * interpolation + defaultOriginValue.Direction;
            double shadowDepth = (defaultDestinationValue.ShadowDepth - defaultOriginValue.ShadowDepth) * interpolation + defaultOriginValue.ShadowDepth;
            double opacity = (defaultDestinationValue.Opacity - defaultOriginValue.Opacity) * interpolation + defaultOriginValue.Opacity;
            DropShadowEffect appliedDropShadow = this.CurrentEffect ?? new DropShadowEffect();
            appliedDropShadow.BlurRadius = blurRadius;
            appliedDropShadow.Direction = direction;
            appliedDropShadow.ShadowDepth = shadowDepth;
            appliedDropShadow.Opacity = opacity;
            return appliedDropShadow;
        }

        #endregion methods


        #region properties

        public DropShadowEffect From
        {
            get
            {
                return GetValue(FromProperty) as DropShadowEffect;
            }
            set
            {
                SetValue(FromProperty, value);
            }
        }

        public DropShadowEffect To
        {
            get
            {
                return GetValue(ToProperty) as DropShadowEffect;
            }
            set
            {
                SetValue(ToProperty, value);
            }
        }

        public DropShadowEffect CurrentEffect
        {
            get; set;
        }

        public override Type TargetPropertyType
        {
            get
            {
                return typeof(DropShadowEffect);
            }
        }

        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(DropShadowEffect), typeof(DropShadowAnimation));

        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(DropShadowEffect), typeof(DropShadowAnimation));

        #endregion properties
    }
}
