#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Waveface.Model;

#endregion

namespace Waveface.Client
{
	public class EventItem : FrameworkElement, INotifyPropertyChanged
	{
		public BitmapSource BitmapImage { get; set; }
		public double MyWidth { get; set; }
		public double MyHeight { get; set; }
		public bool IsVideo { get; set; }
		public bool IsPhoto { get; set; }
		public BitmapSource MediaSource { get; set; }
		public string FileID { get; set; }

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

	public partial class EventUC : UserControl
	{
		public List<FileChange> Event { get; set; }
		public int VideosCount { get; set; }
		public int PhotosCount { get; set; }

		private Point startPoint;

		public EventUC()
		{
			InitializeComponent();
		}

		public void SetUI()
		{
			GetCounts();

			SetInfor();

			List<EventItem> _controls = new List<EventItem>();

			foreach (FileChange _file in Event)
			{
				if (_file.type == 0)
				{
					string _path = _file.tiny_path;

					try
					{
						//if (File.Exists(_path))
						{
							EventItem _eventItem = new EventItem
													   {
														   FileID = _file.id,
														   IsVideo = false,
														   IsPhoto = true
													   };

							BitmapImage _bi = new BitmapImage();
							_bi.BeginInit();
							_bi.UriSource = new Uri(_path, UriKind.Absolute);
							_bi.EndInit();

							_eventItem.BitmapImage = _bi;

							_controls.Add(_eventItem);
						}
					}
					catch
					{
					}
				}
				else
				{
					EventItem _eventItem = new EventItem
											   {
												   FileID = _file.id,
												   IsVideo = true,
												   IsPhoto = false
											   };

					BitmapImage _bi = new BitmapImage();
					_bi.BeginInit();
					_bi.UriSource = new Uri("pack://application:,,,/Resource/video_ph.png");
					_bi.EndInit();

					_eventItem.BitmapImage = _bi;

					BitmapImage _vidoeThumb = new BitmapImage();
					_vidoeThumb.BeginInit();

					if (File.Exists(_file.tiny_path))
						_vidoeThumb.UriSource = new Uri(_file.tiny_path, UriKind.Absolute);
					else
						_vidoeThumb.UriSource = new Uri("pack://application:,,,/Ren/Images/video_130x110.png");

					_vidoeThumb.EndInit();

					_eventItem.MediaSource = _vidoeThumb;

					_controls.Add(_eventItem);
				}
			}

			lbEvent.Items.Clear();
			lbEvent.ItemsSource = _controls;
		}

		private void SetInfor()
		{
			string _timeInterval = GetTimeDisplayString();

			tbTitle.Text = _timeInterval;

			tbTimeAgo.Visibility = Visibility.Collapsed;

			tbTotalCount.Text = SourceAllFilesUC.GetCountsString(PhotosCount, VideosCount);
		}

		private string GetTimeDisplayString()
		{
			string _timeInterval = string.Empty;
			DateTime _startDateTime = SourceAllFilesUC.Current.Rt.DateTimeCache[Event[0].taken_time];

			_timeInterval = _startDateTime.ToString("MMM yyyy");

			return _timeInterval;
		}

		public void GetCounts()
		{
			VideosCount = 0;
			PhotosCount = 0;

			foreach (FileChange _file in Event)
			{
				if (_file.type == 0)
				{
					PhotosCount++;
				}
				else
				{
					VideosCount++;
				}
			}
		}

		#region lbEvent

		private void lbEvent_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			Point _clickPoint = e.GetPosition(lbEvent);

			object _htElement = lbEvent.InputHitTest(_clickPoint);

			if (_htElement != null)
			{
				ListBoxItem _clickedListBoxItem = GetVisualParent<ListBoxItem>(_htElement, 6);

				if (_clickedListBoxItem != null)
				{
					if (_clickedListBoxItem.IsSelected)
					{
					}
				}
			}
		}

		private void lbEvent_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}

		private void lbEvent_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			startPoint = e.GetPosition(null);
		}

		private void lbEvent_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
		}

		private void lbEvent_MouseMove(object sender, MouseEventArgs e)
		{
			// Get the current mouse position
			Point _mousePos = e.GetPosition(null);
			Vector _diff = startPoint - _mousePos;

			if (e.LeftButton == MouseButtonState.Pressed &&
				(Math.Abs(_diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
				 Math.Abs(_diff.Y) > SystemParameters.MinimumVerticalDragDistance))
			{
				Point _clickPoint = e.GetPosition(lbEvent);

				object _htElement = lbEvent.InputHitTest(_clickPoint);

				if (_htElement != null)
				{
					ListBoxItem _clickedListBoxItem = GetVisualParent<ListBoxItem>(_htElement, 6);

					if (_clickedListBoxItem != null)
					{
						_clickedListBoxItem.IsSelected = true;
					}
				}

				List<Content> _contents = SourceAllFilesUC.Current.Sub_GetAllSelectedFiles_ContentEntitys(true);

				DataObject _dragData = new DataObject(typeof(IEnumerable<IContentEntity>), _contents);
				DragDrop.DoDragDrop(this, _dragData, DragDropEffects.Move);
			}
		}

		#endregion

		public List<FileChange> GetSelectedFiles()
		{
			List<FileChange> _fileChanges = new List<FileChange>();

			for (int i = 0; i < lbEvent.Items.Count; i++)
			{
				ListBoxItem _lbi = lbEvent.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;

				if (_lbi.IsSelected)
				{
					_fileChanges.Add(Event[i]);
				}
			}

			return _fileChanges;
		}

		#region Misc

		public T GetVisualParent<T>(object childObject, int level) where T : Visual
		{
			DependencyObject _child = childObject as DependencyObject;

			int k = 0;

			while ((_child != null) && !(_child is T))
			{
				_child = VisualTreeHelper.GetParent(_child);

				if (++k == level)
					break;
			}

			return _child as T;
		}

		#endregion

		private void contextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			if (SourceAllFilesUC.Current.GetAllSelectedFiles().Count == 0)
			{
				e.Handled = true;
			}
		}
	}
}