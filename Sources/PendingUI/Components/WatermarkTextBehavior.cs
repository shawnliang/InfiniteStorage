#region

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

#endregion

namespace Waveface
{
    // This behavior associates a watermark onto a TextBox indicating what the user should provide as input.
    public class WatermarkTextBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(WatermarkTextBehavior),
                                        new FrameworkPropertyMetadata(string.Empty));


        private static readonly DependencyPropertyKey IsWatermarkedPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("IsWatermarked", typeof(bool), typeof(WatermarkTextBehavior),
                                                        new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty IsWatermarkedProperty = IsWatermarkedPropertyKey.DependencyProperty;

        public static bool GetIsWatermarked(TextBox tb)
        {
            return (bool)tb.GetValue(IsWatermarkedProperty);
        }

        public bool IsWatermarked
        {
            get { return GetIsWatermarked(AssociatedObject); }

            private set { AssociatedObject.SetValue(IsWatermarkedPropertyKey, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }

            set { SetValue(TextProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.GotFocus += OnGotFocus;

            AssociatedObject.LostFocus += OnLostFocus;

            OnLostFocus(null, null);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.GotFocus -= OnGotFocus;

            AssociatedObject.LostFocus -= OnLostFocus;
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (string.Compare(AssociatedObject.Text, Text, StringComparison.OrdinalIgnoreCase) == 0)
            {
                AssociatedObject.Text = string.Empty;

                IsWatermarked = false;
            }
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(AssociatedObject.Text))
            {
                AssociatedObject.Text = Text;

                IsWatermarked = true;
            }
        }
    }
}