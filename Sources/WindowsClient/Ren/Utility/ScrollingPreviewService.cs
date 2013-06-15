#region

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;

#endregion

namespace Waveface
{
	public static class ScrollingPreviewService
	{
		public static DataTemplate GetVerticalScrollingPreviewTemplate(DependencyObject obj)
		{
			return (DataTemplate)obj.GetValue(VerticalScrollingPreviewTemplateProperty);
		}

		public static void SetVerticalScrollingPreviewTemplate(DependencyObject obj, DataTemplate value)
		{
			obj.SetValue(VerticalScrollingPreviewTemplateProperty, value);
		}

		public static readonly DependencyProperty VerticalScrollingPreviewTemplateProperty =
			DependencyProperty.RegisterAttached("VerticalScrollingPreviewTemplate", typeof(DataTemplate), typeof(ScrollingPreviewService),
												new FrameworkPropertyMetadata(null, OnVerticalScrollingPreviewTemplateChanged));

		private static void OnVerticalScrollingPreviewTemplateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			if ((e.OldValue == null) && (e.NewValue != null))
			{
				PostAttachToEvents(obj, (DataTemplate)e.NewValue, true);
			}
			else
			{
				throw new NotSupportedException("Cannot change the template once it has been set.");
			}
		}

		public static DataTemplate GetHorizontalScrollingPreviewTemplate(DependencyObject obj)
		{
			return (DataTemplate)obj.GetValue(HorizontalScrollingPreviewTemplateProperty);
		}

		public static void SetHorizontalScrollingPreviewTemplate(DependencyObject obj, DataTemplate value)
		{
			obj.SetValue(HorizontalScrollingPreviewTemplateProperty, value);
		}

		public static readonly DependencyProperty HorizontalScrollingPreviewTemplateProperty =
			DependencyProperty.RegisterAttached("HorizontalScrollingPreviewTemplate", typeof(DataTemplate), typeof(ScrollingPreviewService),
												new FrameworkPropertyMetadata(null, OnHorizontalScrollingPreviewTemplateChanged));

		private static void OnHorizontalScrollingPreviewTemplateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			if ((e.OldValue == null) && (e.NewValue != null))
			{
				PostAttachToEvents(obj, (DataTemplate)e.NewValue, false);
			}
			else
			{
				throw new NotSupportedException("Cannot change the template once it has been set.");
			}
		}

		#region Event Handling

		private static void PostAttachToEvents(DependencyObject obj, DataTemplate dataTemplate, bool vertical)
		{
			// Most likely, the control hasn't expanded its template, wait until
			// Loaded priority is reached before looking for elements.
			obj.Dispatcher.BeginInvoke((NoParamCallback)(() => AttachToEvents(obj, dataTemplate, vertical)), DispatcherPriority.Loaded);
		}

		private delegate void NoParamCallback();

		private static void AttachToEvents(DependencyObject obj, DataTemplate dataTemplate, bool vertical)
		{
			DependencyObject _source = obj;
			ScrollViewer _scrollViewer = FindElementOfType<ScrollViewer>(obj as FrameworkElement);

			if (_scrollViewer != null)
			{
				string _scrollBarPartName = vertical ? "PART_VerticalScrollBar" : "PART_HorizontalScrollBar";
				ScrollBar _scrollBar = FindName<ScrollBar>(_scrollBarPartName, _scrollViewer);

				if (_scrollBar != null)
				{
					Track _track = FindName<Track>("PART_Track", _scrollBar);

					if (_track != null)
					{
						Thumb _thumb = _track.Thumb;

						if (_thumb != null)
						{
							// At this point, all of the control parts have been found.

							// Attach to DragStarted to open the tooltip
							_thumb.DragStarted += delegate
													 {
														 ScrollingPreviewData _data;

														 if (s_previewToolTip == null)
														 {
															 // Create the ToolTip if this is the first time it's been used.
															 s_previewToolTip = new ToolTip();
															 s_previewToolTip.Effect = new DropShadowEffect();

															 _data = new ScrollingPreviewData();
															 s_previewToolTip.Content = _data;
														 }
														 else
														 {
															 _data = s_previewToolTip.Content as ScrollingPreviewData;
														 }

														 // Update the content in the ToolTip
														 _data.UpdateScrollingValues(_scrollBar);
														 _data.UpdateItem(_source as ItemsControl, vertical);

														 // Set the Placement and the PlacementTarget
														 s_previewToolTip.PlacementTarget = _thumb;
														 s_previewToolTip.Placement = vertical ? PlacementMode.Left : PlacementMode.Top;

														 s_previewToolTip.VerticalOffset = 0.0;
														 s_previewToolTip.HorizontalOffset = 0.0;

														 s_previewToolTip.ContentTemplate = dataTemplate;
														 s_previewToolTip.IsOpen = true;
													 };

							// Attach to DragDelta to update the ToolTip's position
							_thumb.DragDelta += delegate
												   {
													   if ((s_previewToolTip != null) &&
														   // Check that we're within the range of the ScrollBar
														   (_scrollBar.Value > _scrollBar.Minimum) && (_scrollBar.Value < _scrollBar.Maximum))
													   {
														   // This is a little trick to cause the ToolTip to update its position next to the Thumb
														   if (vertical)
														   {
															   s_previewToolTip.VerticalOffset = s_previewToolTip.VerticalOffset == 0.0 ? 0.001 : 0.0;
														   }
														   else
														   {
															   s_previewToolTip.HorizontalOffset = s_previewToolTip.HorizontalOffset == 0.0 ? 0.001 : 0.0;
														   }
													   }
												   };

							// Attach to DragCompleted to close the ToolTip
							_thumb.DragCompleted += delegate
													   {
														   if (s_previewToolTip != null)
														   {
															   s_previewToolTip.IsOpen = false;
														   }
													   };

							// Attach to the Scroll event to update the ToolTip content
							_scrollBar.Scroll += delegate
													{
														if (s_previewToolTip != null)
														{
															// The ScrollBar's value isn't updated quite yet, so
															// wait until Input priority
															_scrollBar.Dispatcher.BeginInvoke((NoParamCallback)delegate
																												   {
																													   var data = (ScrollingPreviewData)s_previewToolTip.Content;
																													   data.UpdateScrollingValues(_scrollBar);
																													   data.UpdateItem(_source as ItemsControl, vertical);
																												   }, DispatcherPriority.Input);
														}
													};

							return;
						}
					}
				}

				// At this point, something wasn't found. If the computed visibility is not visible,
				// then add a handler to wait for it to become visible.
				if ((vertical ? _scrollViewer.ComputedVerticalScrollBarVisibility : _scrollViewer.ComputedHorizontalScrollBarVisibility) != Visibility.Visible)
				{
					var _propertyDescriptor =
						DependencyPropertyDescriptor.FromProperty(
							vertical ? ScrollViewer.ComputedVerticalScrollBarVisibilityProperty : ScrollViewer.ComputedHorizontalScrollBarVisibilityProperty, typeof(ScrollViewer));

					if (_propertyDescriptor != null)
					{
						EventHandler _handler = delegate
												   {
													   if ((vertical ? _scrollViewer.ComputedVerticalScrollBarVisibility : _scrollViewer.ComputedHorizontalScrollBarVisibility) ==
														   Visibility.Visible)
													   {
														   EventHandler storedHandler = GetWaitForVisibleScrollBar(_source);
														   _propertyDescriptor.RemoveValueChanged(_scrollViewer, storedHandler);
														   PostAttachToEvents(obj, dataTemplate, vertical);
													   }
												   };
						SetWaitForVisibleScrollBar(_source, _handler);
						_propertyDescriptor.AddValueChanged(_scrollViewer, _handler);
					}
				}
			}
		}

		private static EventHandler GetWaitForVisibleScrollBar(DependencyObject obj)
		{
			return (EventHandler)obj.GetValue(WaitForVisibleScrollBarProperty);
		}

		private static void SetWaitForVisibleScrollBar(DependencyObject obj, EventHandler value)
		{
			obj.SetValue(WaitForVisibleScrollBarProperty, value);
		}

		private static readonly DependencyProperty WaitForVisibleScrollBarProperty =
			DependencyProperty.RegisterAttached("WaitForVisibleScrollBar", typeof(EventHandler), typeof(ScrollingPreviewService), new UIPropertyMetadata(null));

		// Keep one instance of a ToolTip and re-use it
		[ThreadStatic]
		private static ToolTip s_previewToolTip;

		#endregion

		#region Element Search Helpers

		private static T FindName<T>(string name, Control control) where T : FrameworkElement
		{
			ControlTemplate _template = control.Template;

			if (_template != null)
			{
				return _template.FindName(name, control) as T;
			}

			return null;
		}

		private static T FindElementOfType<T>(FrameworkElement element) where T : FrameworkElement
		{
			T _correctlyTyped = element as T;

			if (_correctlyTyped != null)
			{
				return _correctlyTyped;
			}

			if (element != null)
			{
				int _numChildren = VisualTreeHelper.GetChildrenCount(element);

				for (int i = 0; i < _numChildren; i++)
				{
					T _child = FindElementOfType<T>(VisualTreeHelper.GetChild(element, i) as FrameworkElement);

					if (_child != null)
					{
						return _child;
					}
				}

				// Popups continue in another window, jump to that tree
				Popup _popup = element as Popup;

				if (_popup != null)
				{
					return FindElementOfType<T>(_popup.Child as FrameworkElement);
				}
			}

			return null;
		}

		#endregion
	}
}