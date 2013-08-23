#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Waveface.ClientFramework;
using Waveface.Model;

#endregion

namespace Waveface.Client
{
	public partial class RecentListBox : ListBox
	{
		private Point m_startPoint;

		public RecentListBox()
		{
			InitializeComponent();
		}

		private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			e.Handled = true;
		}

		private void UserControl_MouseMove(object sender, MouseEventArgs e)
		{
			Point _mousePos = e.GetPosition(null);
			Vector _diff = m_startPoint - _mousePos;

			if (e.LeftButton == MouseButtonState.Pressed &&
				(Math.Abs(_diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
				 Math.Abs(_diff.Y) > SystemParameters.MinimumVerticalDragDistance))
			{
				if (SelectedItem != null)
				{
					BunnyLabelContentGroup _contentGroup = SelectedItem as BunnyLabelContentGroup;

					if (_contentGroup != null)
					{
						try
						{
							string _tempPathBase = Path.GetTempPath() + "Waveface Photos" + "\\";
							string _path = _tempPathBase + "\\" + Regex.Replace(_contentGroup.Name, @"[?:\/*""<>|]", "") + "\\";

							DirectoryInfo _dir = Directory.CreateDirectory(_path);

							List<string> _files = new List<string>();

							foreach (IContentEntity _entity in _contentGroup.Contents)
							{
								_files.Add(_entity.Uri.LocalPath);
							}

							foreach (string _s in _files)
							{
								File.Copy(_s, Path.Combine(_path, Path.GetFileName(_s)), true);
							}

							DataObject _dragData = new DataObject();
							_dragData.SetData(DataFormats.FileDrop, new[] { _path });
							DragDrop.DoDragDrop(this, _dragData, DragDropEffects.Copy);

							_dir.Delete(true);
							Directory.Delete(_tempPathBase);
						}
						catch
						{
						}
					}
				}
			}
		}

		private void UserControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			m_startPoint = e.GetPosition(null);
		}

		private void UserControl_DragOver(object sender, DragEventArgs e)
		{
			bool _isItem0 = false;
			ListBoxItem _listBoxItem = GetNearestContainer(e.OriginalSource as UIElement);

			if (_listBoxItem != null)
			{
				if ((Items[0]) == _listBoxItem.Content)
				{
					_isItem0 = true;
				}
			}

			if (e.Data.GetDataPresent(typeof(IEnumerable<IContentEntity>)) && _isItem0)
			{
				e.Effects = DragDropEffects.Copy;
			}
			else
			{
				e.Effects = DragDropEffects.None;
			}
		}

		private ListBoxItem GetNearestContainer(UIElement element)
		{
			ListBoxItem _container = element as ListBoxItem;

			while ((_container == null) && (element != null))
			{
				element = VisualTreeHelper.GetParent(element) as UIElement;
				_container = element as ListBoxItem;
			}

			return _container;
		}
	}
}