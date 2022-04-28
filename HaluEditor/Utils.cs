using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ngprojects.HaluEditor
{
    internal static class Utils
    {
        private static readonly Brush defaultColor = Brushes.Black;

        public static Storyboard CreateOpacityBlinkStoryboard(UIElement elem, double animationInMillis = 500)
        {
            if (elem == null)
            {
                throw new ArgumentNullException("elem");
            }
            if (animationInMillis < 0)
            {
                throw new ArgumentException("AnimationTime is lower than 0");
            }

            var switchOffAnimation = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.Zero
            };

            var switchOnAnimation = new DoubleAnimation
            {
                To = 1,
                Duration = TimeSpan.Zero,
                BeginTime = TimeSpan.FromMilliseconds(animationInMillis)
            };

            var blinkStoryboard = new Storyboard
            {
                Duration = TimeSpan.FromMilliseconds(2 * animationInMillis),
                RepeatBehavior = RepeatBehavior.Forever
            };

            Storyboard.SetTarget(switchOffAnimation, elem);
            Storyboard.SetTargetProperty(switchOffAnimation, new PropertyPath(UIElement.OpacityProperty));
            blinkStoryboard.Children.Add(switchOffAnimation);

            Storyboard.SetTarget(switchOnAnimation, elem);
            Storyboard.SetTargetProperty(switchOnAnimation, new PropertyPath(UIElement.OpacityProperty));
            blinkStoryboard.Children.Add(switchOnAnimation);
            return blinkStoryboard;
        }

        public static FormattedText FormatText(string Text, double fontSize, Brush color = null, FontStyle style = default, FontWeight weight = default, DpiScale? dpi = null)
        {
            return new FormattedText(
                Text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(new FontFamily("Consolas"), style, weight, FontStretches.Normal),
                fontSize < 0 ? 1 : fontSize,
                color == null ? defaultColor : color,
                new NumberSubstitution(),
                (dpi != null) ? dpi.Value.PixelsPerDip : 1);
        }
    }
}