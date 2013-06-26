#region

using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

#endregion

namespace Waveface.Client
{
    public class SliderWithDraggingEvents : Slider
    {
        public delegate void ThumbDragStartedHandler(object sender, DragStartedEventArgs e);
        public delegate void ThumbDragCompletedHandler(object sender, DragCompletedEventArgs e);

        public event ThumbDragStartedHandler ThumbDragStarted;
        public event ThumbDragCompletedHandler ThumbDragCompleted;

        public SliderWithDraggingEvents()
        {
            MouseMove += OnMouseMove;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) 
                return;

            var _args = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left) { RoutedEvent = MouseLeftButtonDownEvent };

            base.OnPreviewMouseLeftButtonDown(_args);
        }

        protected override void OnThumbDragStarted(DragStartedEventArgs e)
        {
            if (ThumbDragStarted != null)
                ThumbDragStarted(this, e);

            base.OnThumbDragStarted(e);
        }

        protected override void OnThumbDragCompleted(DragCompletedEventArgs e)
        {
            if (ThumbDragCompleted != null)
                ThumbDragCompleted(this, e);

            base.OnThumbDragCompleted(e);
        }
    }
}