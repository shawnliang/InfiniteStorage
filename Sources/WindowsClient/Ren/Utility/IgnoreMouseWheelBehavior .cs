#region

using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

#endregion

namespace Waveface.Client
{
    // Captures and eats MouseWheel events so that a nested ListBox does not
    // prevent an outer scrollable control from scrolling.
    public sealed class IgnoreMouseWheelBehavior : Behavior<UIElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.PreviewMouseWheel += AssociatedObject_PreviewMouseWheel;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseWheel -= AssociatedObject_PreviewMouseWheel;

            base.OnDetaching();
        }

        private void AssociatedObject_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            MouseWheelEventArgs _args = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                                            {
                                                RoutedEvent = UIElement.MouseWheelEvent
                                            };

            AssociatedObject.RaiseEvent(_args);
        }
    }
}