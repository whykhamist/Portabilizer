using System;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Launcher
{
    public static class ProgressBarExtensions
    {
        public static void SetPercent(this ProgressBar progressBar, double percentage, TimeSpan? duration = null)
        {
            DoubleAnimation animation = new DoubleAnimation(percentage, duration ?? TimeSpan.FromMilliseconds(750));
            progressBar.BeginAnimation(ProgressBar.ValueProperty, animation);
        }
    }
}
