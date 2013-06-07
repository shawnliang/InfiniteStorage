#region

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

#endregion

namespace Waveface
{
    public class ScrollingPreviewData : INotifyPropertyChanged
    {
        private double m_offset;
        private double m_viewport;
        private double m_extent;
        private object m_firstItem;
        private object m_lastItem;

        #region Properties

        public double Offset
        {
            get { return m_offset; }

            internal set
            {
                m_offset = value;
                OnPropertyChanged("Offset");
            }
        }

        public double Viewport
        {
            get { return m_viewport; }
            internal set
            {
                m_viewport = value;
                OnPropertyChanged("Viewport");
            }
        }

        public double Extent
        {
            get { return m_extent; }
            internal set
            {
                m_extent = value;
                OnPropertyChanged("Extent");
            }
        }

        public object FirstItem
        {
            get { return m_firstItem; }
            private set
            {
                m_firstItem = value;
                OnPropertyChanged("FirstItem");
            }
        }

        public object LastItem
        {
            get { return m_lastItem; }
            private set
            {
                m_lastItem = value;
                OnPropertyChanged("LastItem");
            }
        }

        #endregion

        internal void UpdateScrollingValues(ScrollBar scrollBar)
        {
            Offset = scrollBar.Value;
            Viewport = scrollBar.ViewportSize;
            Extent = scrollBar.Maximum - scrollBar.Minimum;
        }

        internal void UpdateItem(ItemsControl itemsControl, bool vertical)
        {
            if (itemsControl != null)
            {
                int _numItems = itemsControl.Items.Count;

                if (_numItems > 0)
                {
                    /*
                    if (VirtualizingStackPanel.GetIsVirtualizing(itemsControl))
                    {
                        // Items scrolling (value == index)
                        int _firstIndex = (int) m_offset;
                        int _lastIndex = (int) m_offset + (int) m_viewport - 1;

                        if ((_firstIndex >= 0) && (_firstIndex < _numItems))
                        {
                            FirstItem = itemsControl.Items[_firstIndex];
                        }
                        else
                        {
                            FirstItem = null;
                        }

                        if ((_lastIndex >= 0) && (_lastIndex < _numItems))
                        {
                            LastItem = itemsControl.Items[_lastIndex];
                        }
                        else
                        {
                            LastItem = null;
                        }
                    }
                    else
                    */
                    {
                        // Pixel scrolling (no virtualization)

                        // This will do a linear search through all of the items.
                        // It will assume that the first item encountered that is within view is
                        // the first visible item and the last item encountered that is
                        // within view is the last visible item.
                        // Improvements could be made to this algorithm depending on the
                        // number of items in the collection and the their order relative
                        // to each other on-screen.

                        ScrollContentPresenter _scp = null;
                        bool _foundFirstItem = false;
                        int _bestLastItemIndex = -1;
                        object _firstVisibleItem = null;
                        object _lastVisibleItem = null;

                        for (int i = 0; i < _numItems; i++)
                        {
                            UIElement _child = itemsControl.ItemContainerGenerator.ContainerFromIndex(i) as UIElement;
                            
                            if (_child != null)
                            {
                                if (_scp == null)
                                {
                                    _scp = FindParent<ScrollContentPresenter>(_child);

                                    if (_scp == null)
                                    {
                                        // Not in a ScrollViewer that we understand
                                        return;
                                    }
                                }

                                // Transform the origin of the child element to see if it is within view
                                GeneralTransform _t = _child.TransformToAncestor(_scp);
                                Point _p = _t.Transform(_foundFirstItem ? new Point(_child.RenderSize.Width, _child.RenderSize.Height) : new Point());

                                if (!_foundFirstItem && ((vertical ? _p.Y : _p.X) >= 0.0))
                                {
                                    // Found the first visible item
                                    _firstVisibleItem = itemsControl.Items[i];
                                    _bestLastItemIndex = i;
                                    _foundFirstItem = true;
                                }
                                else if (_foundFirstItem && ((vertical ? _p.Y : _p.X) < _scp.ActualHeight))
                                {
                                    // Found a candidate for the last visible item
                                    _bestLastItemIndex = i;
                                }
                            }
                        }

                        if (_bestLastItemIndex >= 0)
                        {
                            _lastVisibleItem = itemsControl.Items[_bestLastItemIndex];
                        }

                        // Update the item properties
                        FirstItem = _firstVisibleItem;
                        LastItem = _lastVisibleItem;
                    }
                }
            }
        }

        private static T FindParent<T>(Visual v) where T : Visual
        {
            v = VisualTreeHelper.GetParent(v) as Visual;

            while (v != null)
            {
                T _correctlyTyped = v as T;

                if (_correctlyTyped != null)
                {
                    return _correctlyTyped;
                }
                else
                {
                    v = VisualTreeHelper.GetParent(v) as Visual;
                }
            }

            return null;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
}