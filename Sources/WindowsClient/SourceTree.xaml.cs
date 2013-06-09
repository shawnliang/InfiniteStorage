using System;
using System.Windows;
using System.Windows.Controls;
using Waveface.Model;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace Waveface.Client
{
    /// <summary>
    /// Interaction logic for SourceTree.xaml
    /// </summary>
    public partial class SourceTree : TreeView
    {
        #region Event
        public event EventHandler TreeViewItemClick;
        #endregion

        public SourceTree()
        {
            this.InitializeComponent();
        }


        private void TreeViewItem_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (TreeViewItemClick == null)
                return;

            TreeViewItemClick(sender, EventArgs.Empty);
        }

		private void recvingIcon_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var recvIcon = (Image)sender;

			if (recvIcon.Visibility == System.Windows.Visibility.Visible)
			{
				var da = new DoubleAnimation(0, 360, new Duration(TimeSpan.FromSeconds(1.0)));
				var rotate = new RotateTransform();

				recvIcon.RenderTransform = rotate;
				recvIcon.RenderTransformOrigin = new Point(0.5, 0.5);
				da.RepeatBehavior = RepeatBehavior.Forever;
				rotate.BeginAnimation(RotateTransform.AngleProperty, da);
			}
			else
			{
				// QUESTION: need to stop animation????? CPU is a little bit high ....
				recvIcon.RenderTransform = null;
			}
		}

    }
}