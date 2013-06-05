using System;
using System.Windows;
using System.Windows.Controls;
using Waveface.Model;

namespace Waveface.Client
{
    /// <summary>
    /// Interaction logic for SourceTree.xaml
    /// </summary>
    public partial class SourceTree : TreeView
    {
        #region Event
        public event EventHandler TreeViewItemClick;
        public event EventHandler<UnSortedItemEventArgs> UnSortedItemClick;
        #endregion

        public SourceTree()
        {
            this.InitializeComponent();
        }


        protected void OnUnSortedItemClick(UnSortedItemEventArgs e)
        {
            if (UnSortedItemClick == null)
                return;
            UnSortedItemClick(this, e);
        }

        private void TreeViewItem_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (TreeViewItemClick == null)
                return;

            TreeViewItemClick(sender, EventArgs.Empty);
        }

        private void unsortedItem_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            IService _service = SelectedItem as IService;

            if (_service != null)
            {
                OnUnSortedItemClick(new UnSortedItemEventArgs(_service.ID));
            }
        }
    }
}